import fs from "fs";
import axios from "axios";
import SocketManager from "../utils/socketManager";

export default {
  registerUser({ commit, state }, formData) {
    const { email, username, password, summonername, apiserverhost } = formData;
    commit("setAPIServerHost", apiserverhost);
    const { host, port } = state.config.api;

    return axios
      .put(`${host}:${port}/users/register`, {
        email: email,
        userName: username,
        password: password,
        summonerName: summonername
      })
      .then(function(response) {
        return response;
      })
      .catch(function(error) {
        if (
          error.response &&
          error.response.data &&
          error.response.data !== ""
        ) {
          console.error(error);
          throw new Error(JSON.stringify(error.response.data.message));
        } else {
          throw new Error(error);
        }
      });
  },
  logUserIn({ commit, state, dispatch }, formData) {
    const {
      username,
      password,
      apiserverhost,
      cdnserverhost,
      rememberme,
      clientpath,
      langcode
    } = formData;
    commit("setAPIServerHost", apiserverhost);
    commit("setCDNServerHost", cdnserverhost);
    commit("setClientPath", clientpath);
    const { host, port } = state.config.api;
    const vm = this._vm;

    if (state.token !== "") {
      console.log("tried to log in again(?)");
      return;
    }

    return axios
      .post(`${host}:${port}/users/login`, {
        userName: username,
        password: password
      })
      .then(function(response) {
        response.data.summonerStatus = "ONLINE";
        const user = response.data;
        const token = user.token;
        commit("setUser", user);
        commit("setToken", token);
        if (rememberme) {
          state.rememberTokenPath.then(result => {
            fs.writeFileSync(
              result,
              JSON.stringify({
                token: token,
                clientpath: clientpath,
                cdnserverhost: cdnserverhost,
                apiserverhost: apiserverhost,
                langcode: langcode
              })
            );
          });
        }

        console.log("new socket manager");
        let socketManager = new SocketManager(user, state, dispatch, vm);
        commit("setSocketManager", socketManager);
        return response;
      })
      .catch(function(error) {
        if (
          error.response &&
          error.response.data &&
          error.response.data !== ""
        ) {
          console.error(error);
          throw new Error(JSON.stringify(error.response.data.message));
        } else {
          throw new Error(error);
        }
      });
  },
  sendBugReport({ state }, formData) {
    const { description, text } = formData;

    const { host, port } = state.config.api;
    const config = {
      headers: { Authorization: `Bearer ${state.token}` }
    };

    return axios
      .put(
        `${host}:${port}/bugs`,
        {
          description: description,
          text: text
        },
        config
      )
      .then(function(response) {
        return response.data;
      })
      .catch(err => {
        throw err;
      });
  },
  getLeagueSandboxCommits({ state }) {
    const url = `https://api.github.com/repos/LeagueSandbox/GameServer/commits?per_page=5`;

    return axios
      .get(url)
      .then(response => {
        state.commitList = response.data;
        return response.data;
      })
      .catch(err => {
        throw err;
      });
  },
  getNews({ state }) {
    const { host, port } = state.config.api;
    const config = {
      headers: { Authorization: `Bearer ${state.token}` }
    };

    return axios
      .get(`${host}:${port}/news`, config)
      .then(function(response) {
        state.newsList = response.data;
        return response.data;
      })
      .catch(err => {
        throw err;
      });
  },
  getAlerts({ state }) {
    const { host, port } = state.config.api;
    const config = {
      headers: { Authorization: `Bearer ${state.token}` }
    };

    return axios
      .get(`${host}:${port}/alerts`, config)
      .then(function(response) {
        state.alertList = response.data;
        console.log(state.alertList);
        return response.data;
      })
      .catch(err => {
        throw err;
      });
  },
  getQueueCount({ commit, state }) {
    const { host, port } = state.config.api;
    const config = {
      headers: { Authorization: `Bearer ${state.token}` }
    };

    return axios
      .get(`${host}:${port}/metrics/queue/count`, config)
      .then(function(response) {
        commit("setQueueCount", response.data);
        return response.data;
      })
      .catch(err => {
        throw err;
      });
  },
  getServerCount({ commit, state }) {
    const { host, port } = state.config.api;
    const config = {
      headers: { Authorization: `Bearer ${state.token}` }
    };

    return axios
      .get(`${host}:${port}/metrics/queue/availableGameserver`, config)
      .then(function(response) {
        commit("setServerCount", response.data);
        return response.data;
      })
      .catch(err => {
        throw err;
      });
  },
  logUserOut({ commit, state }) {
    return new Promise((resolve, reject) => {
      try {
        state.rememberTokenPath.then(result => {
          if (fs.existsSync(result)) {
            const currentlyPersisted = JSON.parse(
              fs.readFileSync(result, "utf-8")
            );
            currentlyPersisted.token = "";
            fs.writeFileSync(result, JSON.stringify(currentlyPersisted));
          }
        });
        this._vm.$socket.disconnectFromSystem();
        commit("changeBackgroundState", { newState: "HOME" });
        resolve(true);
        commit("clearUser");
        commit("clearToken");
        commit("clearFriends");
        commit("clearChats");
        commit("clearLobby");
        commit("clearLobbyTimers");
      } catch (error) {
        reject(error);
      }
    });
  },
  validateToken({ commit, state, dispatch }, obj) {
    const { host, port } = state.config.api;
    const config = {
      headers: { Authorization: `Bearer ${obj.token}` }
    };
    const vm = this._vm;

    return axios
      .get(`${host}:${port}/users`, config)
      .then(function(response) {
        let user = response.data;
        user.summonerStatus = "ONLINE";
        commit("setUser", user);
        commit("setToken", obj.token);

        console.log("new socket manager 2");
        let socketManager = new SocketManager(user, state, dispatch, vm);
        commit("setSocketManager", socketManager);
        return user;
      })
      .catch(err => {
        throw new Error(err);
      });
  },
  setUser({ commit }, user) {
    commit("setUser", user);
  },
  readRememberToken({ commit, state }) {
    return state.rememberTokenPath.then(result => {
      let data = null;

      if (fs.existsSync(result)) {
        data = JSON.parse(fs.readFileSync(result, "utf8"));
        const didReLoad = this._vm.$translate.safeLoadLanguages(
          null,
          data.langcode
        );
        if (!didReLoad) {
          this._vm.$translate.setLang(data.langcode);
        }
        commit("setRememberToken", data.token);
        commit("setAPIServerHost", data.apiserverhost);
        commit("setCDNServerHost", data.cdnserverhost);
        commit("setClientPath", data.clientpath);
        return data;
      }
      const didReLoad = this._vm.$translate.safeLoadLanguages(null, "en_US");
      if (!didReLoad) {
        this._vm.$translate.setLang("en_US");
      }

      return data;
    });
  },
  saveUserMotto({ commit }, motto) {
    commit("setUserMotto", motto);
  },
  saveUserStatus({ commit }, changeto) {
    commit("setUserStatus", changeto);
  },
  searchFilterFriendlist({ commit, state }, word) {
    let friendlist = state.friendList;
    if (word.length === 0) {
      friendlist.forEach((friend, index) => {
        commit("setFilterShowFriend", { index: index, state: true });
      });
    }

    friendlist.forEach((friend, index) => {
      if (friend.summonerName.toLowerCase().includes(word.toLowerCase())) {
        commit("setFilterShowFriend", { index: index, state: true });
      } else {
        commit("setFilterShowFriend", { index: index, state: false });
      }
    });
  },
  searchFilterChampions({ commit, state }, word) {
    let champions = state.collection.champions;
    if (word.length === 0) {
      champions.forEach((champion, index) => {
        commit("setFilterShowChampion", { index: index, state: true });
      });
    }

    champions.forEach((champion, index) => {
      if (champion.displayName.toLowerCase().startsWith(word.toLowerCase())) {
        commit("setFilterShowChampion", { index: index, state: true });
      } else {
        commit("setFilterShowChampion", { index: index, state: false });
      }
    });
  },
  setSocialActionType({ commit }, type) {
    commit("setSocialAction", type);
  },
  setSummonerIcon({ commit }, iconId) {
    commit("setUserIcon", iconId);
  },
  setPrivateChatOpen({ commit, state }, recipient) {
    if (
      state.privateChat.openChats.filter(chat => {
        return chat.summonerName == recipient.summonerName;
      }).length > 0
    ) {
      commit("setPrivateChatStatus", {
        summonerName: recipient.summonerName,
        change: "open"
      });
      return false;
    } else {
      if (state.privateChat.openChats.length >= 4) {
        commit("setPrivateChatClosed", state.privateChat.openChats[3]);
      }
      commit("setPrivateChatOpen", recipient);
      return true;
    }
  },
  setPrivateChatClosed({ commit }, recipient) {
    commit("setPrivateChatClosed", recipient);
  },
  setPrivateChatStatus({ commit }, change) {
    commit("setPrivateChatStatus", change);
  },
  setPrivateChatReadMessages({ commit }, recipient) {
    commit("setPrivateChatReadMessages", recipient);
  },
  clearLobbyTimers({ commit }) {
    commit("clearLobbyTimers");
  },
  clearMatchFoundState({ commit }) {
    commit("clearMatchFoundState");
  },
  setLobbyTimers({ commit }) {
    commit("setLobbyTimers");
  },
  changeBackgroundState({ commit }, bgstate) {
    commit("changeBackgroundState", bgstate);
  },
  deductUserCoins({ commit }, deductable) {
    commit("deductUserCoins", deductable);
  }
};
