import Vue from "vue";

const httpPrefix = "http://";
const httpsPrefix = "https://";

export default {
  clearUser(state) {
    state.user = {};
  },
  clearToken(state) {
    state.token = "";
  },
  clearFriends(state) {
    state.friendList = [];
    state.friendOut = [];
    state.friendIn = [];
  },
  clearChats(state) {
    state.privateChat = {
      chatMessages: [],
      openChats: []
    };
  },
  clearLobby(state) {
    state.lobby = {};
    state.lobbyChatMessages = [];
    state.lobbyOwner = false;
    state.lobbyInvites = [];
  },
  setUser(state, userObj) {
    state.user = userObj;
  },
  setUserIcon(state, iconId) {
    state.user.summonerIconId = iconId;
  },
  setUserMotto(state, motto) {
    state.user.summonerMotto = motto;
  },
  setUserStatus(state, status) {
    state.user.summonerStatus = status;
  },
  setToken(state, token) {
    state.token = token;
  },
  setAPIServerHost(state, apiServerHost) {
    if (apiServerHost.includes(":")) {
      let host = apiServerHost.split(":");
      state.config.api.host = httpPrefix + host[0];
      state.config.api.port = host[1];
    } else {
      state.config.api.host = httpsPrefix + apiServerHost;
      state.config.api.port = 443;
    }
  },
  setCDNServerHost(state, cdnServerHost) {
    if (cdnServerHost.includes(":")) {
      let host = cdnServerHost.split(":");
      state.config.download.host = httpPrefix + host[0];
      state.config.download.port = host[1];
    } else {
      state.config.download.host = httpsPrefix + cdnServerHost;
      state.config.download.port = 443;
    }
  },
  setClientPath(state, clientPath) {
    state.config.path.client = clientPath;
  },
  setRememberToken(state, token) {
    state.rememberToken = token;
  },
  setFilterShowFriend(state, obj) {
    state.friendList[obj.index].shouldShow = obj.state;
  },
  setFilterShowChampion(state, obj) {
    state.collection.champions[obj.index].shouldShow = obj.state;
  },
  setSocialAction(state, type) {
    state.socialAction = type;
  },
  setSocketManager(state, manager) {
    state.socketManager = manager;
  },
  setCollectionChampions(state, champions) {
    champions.forEach(c => (c.shouldShow = true));
    state.collection.champions = champions;
  },
  setSelectedChampion(state, champion) {
    state.collection.selectedChampion = champion;
  },
  setCollectionIcons(state, icons) {
    state.collection.icons = icons;
  },
  setPrivateChatClosed(state, recipient) {
    state.privateChat.openChats = state.privateChat.openChats.filter(chat => {
      return chat.summonerName != recipient.summonerName;
    });
  },
  setPrivateChatOpen(state, recipient) {
    state.privateChat.openChats = [
      ...state.privateChat.openChats,
      { ...recipient, shows: true }
    ];
  },
  setPrivateChatStatus(state, change) {
    state.privateChat.openChats = state.privateChat.openChats.map(chat => {
      if (chat.summonerName == change.summonerName) {
        switch (change.change) {
          case "toggle":
            chat.shows = !chat.shows;
            break;

          case "close":
            chat.shows = false;
            break;

          case "open":
            chat.shows = true;
            break;

          default:
            console.warn("Unknown chat change.");
            break;
        }
      }

      return chat;
    });
  },
  setPrivateChatReadMessages(state, recipient) {
    let msgs = state.privateChat.chatMessages[recipient.summonerName];
    if (msgs) {
      msgs.forEach(message => {
        message.read = true;
      });

      Vue.set(state.privateChat.chatMessages, recipient.summonerName, msgs);
    }
  },
  clearLobbyTimers(state) {
    state.lobbyQueueTime = 1;
    clearInterval(state.lobbyQueueTimer);
    state.lobbyQueueTimer = null;
  },
  clearMatchFoundState(state) {
    state.matchFoundState = {
      accepted: 0,
      pending: 10,
      denied: 0
    };
  },
  setLobbyTimers(state) {
    state.lobbyQueueTimer = setInterval(() => {
      state.lobbyQueueTime += 1;
    }, 1000);
  },
  setQueueCount(state, queueCount) {
    state.lobbyQueueCount = queueCount;
  },
  setServerCount(state, serverCount) {
    state.serverCount = serverCount;
  },
  changeBackgroundState(state, bgstate) {
    state.backgroundState = bgstate;
  },
  deductUserCoins(state, deductable) {
    state.user.s4Coins -= deductable;
  },
  setSidebarComponent(state, component) {
    if (state.sidebarComponent == component) {
      state.sidebarComponent = "hidden";
    } else {
      state.sidebarComponent = component;
    }
  }
};
