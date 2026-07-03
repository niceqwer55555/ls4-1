import Vuex from "vuex";
import Vue from "vue";
import path from "path";

// 3RD PARTY
import { ipcRenderer } from "electron";

// Exported actions
import systemActions from "./actions/systemActions";
import socketActions from "./actions/socketActions";
import collectionActions from "./actions/collectionActions";
import storeActions from "./actions/storeActions";

// Exported mutations
import systemMutations from "./mutations/systemMutations";
import socketMutations from "./mutations/socketMutations";

// USE
Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    version:
      process.env.VUE_APP_VERSION !== "undefined"
        ? `Build: ${process.env.VUE_APP_VERSION}`
        : "Development",
    token: "",
    commitList: [],
    newsList: [],
    alertList: [],
    friendList: [],
    friendOut: [],
    friendIn: [],
    sidebarComponent: "friends",
    privateChat: {
      chatMessages: [],
      openChats: []
    },
    user: {
      summonerName: "Preloaded",
      summonerIconId: 29,
      summonerMotto: "PreloadedMotto",
      summonerLevel: 30,
      email: "testmail",
      ownedIcons: [],
      roles: [],
      summonerExperienceNeeded: undefined,
      summonerStatus: "OFFLINE",
      userName: "",
      uuid: "",
      s4Coins: 0
    },
    config: {
      download: {
        host:
          process.env.VUE_APP_CDN !== "undefined" &&
          process.env.VUE_APP_CDN !== undefined
            ? `https://${process.env.VUE_APP_CDN}`
            : "http://localhost:8081/cdn", //https://cdn.leagues4.com
        port:
          process.env.VUE_APP_CDN_PORT !== "undefined" &&
          process.env.VUE_APP_CDN_PORT !== undefined
            ? 80 //process.env.VUE_APP_CDN_PORT
            : 443
      },
      api: {
        host:
          process.env.VUE_APP_API !== "undefined" &&
          process.env.VUE_APP_API !== undefined
            ? `https://${process.env.VUE_APP_API}`
            : "http://127.0.0.1:8080", //https://api.leagues4.com
        port:
          process.env.VUE_APP_API_PORT !== "undefined" &&
          process.env.VUE_APP_API_PORT !== undefined
            ? 8080 //process.env.VUE_APP_API_PORT
            : 443
      },
      path: {
        client: ""
      }
    },
    rememberToken: null,
    rememberTokenPath: new Promise(function(resolve) {
      ipcRenderer.invoke("getPath").then(result => {
        resolve(path.join(result, "rememberToken.json"));
      });
    }),
    backgroundState: "LOGIN",
    collection: {
      champions: [],
      selectedChampion: {},
      icons: []
    },
    lobby: {},
    lobbyChatMessages: [],
    lobbyOwner: false,
    lobbyInvites: [],
    lobbyQueueTimer: null,
    lobbyQueueTime: 1,
    lobbyQueueCount: "-",
    serverCount: "-",
    matchFoundState: {
      accepted: 0,
      pending: 10,
      denied: 0
    },
    champselect: {},
    champselectMessages: [],
    csEnemyTeam: [],
    csAllyBans: [],
    csEnemyBans: [],
    csGlobalState: {},
    csCurrentPlayer: {},
    availableSpells: [
      "SummonerHeal",
      "SummonerFlash",
      "SummonerBoost",
      "SummonerDot",
      "SummonerExhaust",
      "SummonerHaste",
      "SummonerMana",
      "SummonerRevive",
      "SummonerSmite",
      "SummonerTeleport",
      "SummonerBarrier",
      "SummonerOdinGarrison",
      "SummonerClairvoyance"
    ]
  },
  mutations: {
    ...systemMutations,
    ...socketMutations
  },
  actions: {
    ...systemActions,
    ...socketActions,
    ...collectionActions,
    ...storeActions
  },
  modules: {},
  getters: {}
});
