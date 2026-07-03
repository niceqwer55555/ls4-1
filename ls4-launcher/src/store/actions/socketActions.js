export default {
  socketExtraHandler({ commit }, handle) {
    commit(handle.commit, handle.data);
  },
  socketRecievedFriendList({ commit }, data) {
    data.forEach(friend => {
      friend.shouldShow = true;
    });
    commit("socketSetFriendList", data);
  },
  socketRecievedFriendIn({ commit }, data) {
    if (Array.isArray(data)) {
      commit("socketSetFriendIn", data);
    } else {
      commit("socketAddFriendIn", data);
    }
  },
  socketRecievedFriendOut({ commit }, data) {
    if (Array.isArray(data)) {
      commit("socketSetFriendOut", data);
    } else {
      commit("socketAddFriendOut", data);
    }
  },
  socketRecievedFriendInAccept({ commit }, friend) {
    friend.shouldShow = true;
    commit("socketAddFriend", friend);
    commit("socketDelFriendOut", friend);
  },
  socketRecievedFriendInDeny({ commit }, friend) {
    commit("socketDelFriendOut", friend);
  },
  socketRecievedSummonerNameUpdate({ commit }, data) {
    commit("socketSetUserSummonerName", data);
  },
  socketRecievedPrivateMessage({ commit }, data) {
    commit("socketAddPrivateChatMessage", data);
  },
  socketRecievedFriendUpdate({ commit }, data) {
    commit("socketSetFriend", data);
  },
  socketRecievedLobbyUpdate({ commit }, data) {
    commit("socketSetUserLobby", data);
  },
  socketRecievedMatchFoundUpdate({ commit }, data) {
    commit("socketSetMatchFoundState", data);
  },
  socketRecievedLobbyInvite({ commit }, data) {
    commit("socketAddLobbyInvite", data);
  },
  socketReceivedLobbyInviteRevoke({ commit }, data) {
    commit("socketRemoveInvite", data.lobbyUuid);
  },
  socketRecievedLobbyChat({ commit }, data) {
    commit("socketAddLobbyChatMeessage", data);
  },
  socketKickedFromLobby({ commit }) {
    commit("socketClearLobbyData");
    commit("clearLobbyTimers");
  },
  setLobbyQueueState({ commit }, state) {
    commit("setLobbyQueueState", state);
  },
  socketRecievedChampSelectChat({ commit }, data) {
    commit("socketAddChampSelectChatMessage", data);
  },
  socketSetUserChampSelect({ commit }, data) {
    commit("socketSetUserChampSelect", data);
  }
};

/*
this._vm = vue instance
router -> can be imported
*/
