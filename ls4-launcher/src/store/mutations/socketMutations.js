// import moment from "moment";
import Vue from "vue";

export default {
  // SETTER
  socketSetFriendOut(state, data) {
    state.friendOut = data;
  },
  socketSetFriendIn(state, data) {
    state.friendIn = data;
  },
  socketSetFriendList(state, data) {
    state.friendList = data;
  },
  socketSetUserSummonerName(state, data) {
    state.friendList = state.friendList.map(friend => {
      if (friend.summonerName == data.oldSummonerName) {
        friend.summonerName = data.newSummonerName;
      }
      return friend;
    });
  },
  socketSetChatMessagesForRecipient(state, data) {
    if (data.sound) {
      this._vm.$sound.template("OVERVIEW_CHATINIT");
    }
    Vue.set(
      state.privateChat.chatMessages,
      data.recipient.summonerName,
      data.messages.data
    );
  },
  socketSetFriend(state, data) {
    state.friendList.forEach((friend, index) => {
      if (friend.summonerName == data.summonerName) {
        data.shouldShow = true;
        Vue.set(state.friendList, index, data);
      }
    });
  },
  // ADDITION
  socketAddFriendIn(state, data) {
    state.friendIn.push(data);
    this._vm.$sound.template("CORE_INVITE_RECIEVED");
  },
  socketAddFriendOut(state, data) {
    state.friendOut.push(data);
  },
  socketAddFriend(state, data) {
    data.shouldShow = true;
    state.friendList.push(data);
  },
  socketAddPrivateChatMessage(state, data) {
    if (state.privateChat.chatMessages[data.from]) {
      const chat = state.privateChat.openChats.filter(chat => {
        return chat.summonerName == data.from;
      });
      if ((chat.length > 0 && !chat[0].shows) || chat.length == 0) {
        this._vm.$sound.template("OVERVIEW_CHATINIT");
        data.read = false;
      }

      if (typeof data.read == "undefined") {
        data.read = true;
      }
      state.privateChat.chatMessages[data.from].unshift(data);
    }
  },
  socketAddChatMessageToRecipient(state, data) {
    if (state.privateChat.chatMessages[data.to]) {
      state.privateChat.chatMessages[data.to].unshift(data);
    } else {
      // NOT POSSIBLE TO SEND MESSAGE WITHOUT SELECTING...
    }
  },
  // DELETION
  socketDelFriendOut(state, removable) {
    state.friendOut.forEach((friend, index) => {
      if (friend.summonerName == removable.summonerName) {
        state.friendOut.splice(index, 1);
      }
    });
  },
  socketDelFriendIn(state, removable) {
    state.friendIn.forEach((friend, index) => {
      if (friend.summonerName == removable.summonerName) {
        state.friendIn.splice(index, 1);
      }
    });
  },
  socketDelFriend(state, removable) {
    state.friendList.forEach((friend, index) => {
      if (friend.summonerName == removable.summonerName) {
        state.friendList.splice(index, 1);
      }
    });
  },

  // LOBBY
  // ADDITION
  socketAddLobbyChatMeessage(state, data) {
    state.lobbyChatMessages.push(data);
  },
  socketAddLobbyInvite(state, data) {
    state.lobbyInvites.push(data);
  },
  // SETTERS
  socketSetUserLobby(state, data) {
    if (data.owner.summonerName == state.user.summonerName) {
      state.lobbyOwner = true;
    } else {
      state.lobbyOwner = false;
    }
    state.lobby = data;
  },
  socketClearLobbyData(state) {
    state.lobby = {};
    state.lobbyOwner = false;
    state.lobbyChatMessages = [];
    state.lobbyQueueTime = 1;
    state.matchFoundState = {
      accepted: 0,
      pending: 10,
      denied: 0
    };
    clearInterval(state.lobbyQueueTimer);
    state.lobbyQueueTimer = null;
  },
  setLobbyQueueState(state) {
    state.lobby.inQueue = false;
  },
  socketSetMatchFoundState(state) {
    state.matchFoundState = state;
  },
  // REMOVE
  socketRemoveInvite(state, lobbyId) {
    state.lobbyInvites.forEach((invite, index) => {
      if (invite.lobbyUuid === lobbyId) {
        state.lobbyInvites.splice(index, 1);
      }
    });
  },

  // CHAMPSELECT
  // ADDITION
  socketAddChampSelectChatMessage(state, data) {
    state.champselectMessages.push(data);
  },
  // SETTERS
  socketSetUserChampSelect(state, data) {
    const csData = data;

    // DETERMINE PHASE OF CHAMPSELECT
    if (csData && csData.lobbyPhase) {
      const globalState = csData.lobbyPhase.split("_");
      let lobbyState = {};

      lobbyState = {
        phase: globalState[0], // BAN, PICK, or PRE
        team: globalState[1], // TEAM1 or TEAM2
        time: csData.timer - 1,
        pickedChampionsLength: [...csData.team, ...csData.enemyTeam].filter(
          member => {
            return member.selectedChampion && member.lockedIn;
          }
        ).length,
        bannedChampionsLength: [...csData.bansTeam1, ...csData.bansTeam2].length
      };

      state.csGlobalState = lobbyState;
    }
    // DETERMINE PHASE OF CHAMPSELECT END

    // SET CHAMPSELECT TEAMDATA
    state.champselect = csData;
    // SET CHAMPSELECT TEAMDATA END

    // DETERMINE CURRENT PLAYER
    if (csData.lobbyPhase) {
      const playerIndex = csData.team
        .map(function(e) {
          return e.user.summonerName;
        })
        .indexOf(state.user.summonerName);

      state.csCurrentPlayer = csData.team[playerIndex];
    } else {
      state.csCurrentPlayer = null;
    }
    // DETERMINE CURRENT PLAYER END

    // SET ENEMY TEAM DATA
    state.csEnemyTeam = csData.enemyTeam;
    // SET ENEMY TEAM DATA END

    // DETERMINE BANS
    if (csData.lobbyPhase) {
      if (state.csCurrentPlayer.team == "TEAM1") {
        state.csAllyBans = csData.bansTeam1;
        state.csEnemyBans = csData.bansTeam2;
      } else if (state.csCurrentPlayer.team == "TEAM2") {
        state.csAllyBans = csData.bansTeam2;
        state.csEnemyBans = csData.bansTeam1;
      }
    }
    // DETERMINE BANS END

    // DETERMINE UNUSABLE CHAMPIONS
    if (state.collection.champions) {
      let unusable = [];
      const bans = csData.bansTeam1.concat(csData.bansTeam2);
      const teams = csData.team.concat(state.csEnemyTeam);

      bans.forEach(ban => {
        unusable.push(ban.id);
      });

      teams.forEach(player => {
        if (player.lockedIn && player.selectedChampion) {
          unusable.push(player.selectedChampion.id);
        }
      });

      state.collection.champions.forEach(champion => {
        if (unusable.includes(champion.id)) {
          Vue.set(champion, "unavailable", true);
        } else {
          Vue.set(champion, "unavailable", false);
        }
      });
    }
    // DETERMINE UNUSABLE CHAMPIONS END
  },
  socketClearChampSelectData(state) {
    state.champselect = {};
    state.champselectMessages = [];
    state.csEnemyTeam = [];
    state.csAllyBans = [];
    state.csEnemyBans = [];
    state.csGlobalState = {};
    state.csCurrentPlayer = {};
  }
  // REMOVE
};
