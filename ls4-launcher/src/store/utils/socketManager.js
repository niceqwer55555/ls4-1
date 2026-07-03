var path = require("path");
import { Client } from "@stomp/stompjs";
import Vue from "vue";
import { uuid } from "uuidv4";
import axios from "axios";
import router from "../../Router";
import { ipcRenderer } from "electron";

class EventEmitter {
  constructor() {
    this._events = {};
  }

  on(name, listener) {
    if (!this._events[name]) {
      this._events[name] = [];
    }

    this._events[name].push(listener);
  }

  removeListener(name, listenerToRemove) {
    if (!this._events[name]) {
      throw new Error(
        `Can't remove a listener. Event "${name}" doesn't exits.`
      );
    }

    const filterListeners = listener => listener !== listenerToRemove;

    this._events[name] = this._events[name].filter(filterListeners);
  }

  emit(name, data) {
    if (!this._events[name]) {
      throw new Error(`Can't emit an event. Event "${name}" doesn't exits.`);
    }

    const fireCallbacks = callback => {
      callback(data);
    };

    this._events[name].forEach(fireCallbacks);
  }
}

export default class SocketManager {
  constructor(userObj, state, dispatch, vm) {
    const config = state.config.api;
    this.api = config;
    this.vm = vm;
    this.state = state;
    this.wasConnected = false;
    this.reconnectCount = 0;

    this.stomp = new Client({
      brokerURL: `ws://${config.host.replace(/^https?:\/\//, "")}:${
        config.port
      }/socket?token=${state.token}`,
      onConnect: connectedFrame => {
        this.emitter.emit("connected", true);
        this.log(connectedFrame);
        this.connectToSystem(); // Subscribe to System Destination
        this.afterConnect();
        this.wasConnected = true;

        if (this.reconnectCount >= 1) {
          // This means we connect after reconnect, refresh to home page
          if (router.currentRoute.name != "Home") {
            router.push("/LoggedIn/home");
          }
          this.vm.$modal.reconnect.hide();

          this.reconnectCount = 0;
        }
      },
      reconnectDelay: 5000,
      onWebSocketClose: closed => {
        if (this.wasConnected) {
          // Reset wasConnected
          this.wasConnected = false;

          if (closed.code != 1000) {
            // Connection loss happened
            this.reconnectCount++;
            this.connectionLoss();
          }
        } else {
          // Connection loss happened on reconnect, increase counter and closeApp on 1 minute
          this.reconnectCount++;

          if (this.reconnectCount >= 13) {
            ipcRenderer.send("closeApp");
          }
        }
      }
    });
    this.user = userObj;
    this.connected = false;
    this.dispatch = dispatch;
    this.auth_headers = { headers: { Authorization: `Bearer ${state.token}` } };
    this.emitter = new EventEmitter();
    this.callbacks = [];
    this.isFirstMessage = true;
    this.lobbysub = null;
    this.systemsub = null;
    this.champselectsub = null;

    Vue.prototype.$socket = this;

    this.initEvents();

    this.stomp.activate();
  }

  connectionLoss() {
    // We can keep all data except lobby and chats

    if (router.currentRoute.name != "Home") {
      router.push("/LoggedIn/home");
    }
    this.dispatch("socketExtraHandler", {
      commit: "clearLobby"
    });
    this.dispatch("socketExtraHandler", {
      commit: "clearChats"
    });
    this.dispatch("socketExtraHandler", {
      commit: "clearLobbyTimers"
    });

    this.vm.$modal.reconnect.show();
  }

  /* INITIALIZATIONS */
  initEvents() {
    this.emitter.on("connected", () => {
      this.connected = true;
      this.afterConnect();
    });
  }

  /* SUBSCRIPTIONS */
  connectToSystem() {
    this.log("Subscribed to /user/queue/system");
    this.systemsub = this.stomp.subscribe("/user/queue/system", message => {
      if (
        this.isFirstMessage &&
        this.parseMessage(message).messageType !=
          "USER_LOGGED_IN_ANOTHER_LOCATION"
      ) {
        this.isFirstMessage = false;
      }
      this.recievedSystemMessage(message);
    });
  }

  connectToLobby(lobbyId) {
    this.log(`Subscribed to /user/queue/lobby${lobbyId}`);
    this.lobbysub = this.stomp.subscribe(
      `/user/queue/lobby${lobbyId}`,
      message => {
        this.recievedLobbyMessage(message);
      }
    );
  }

  connectToChampSelect(champselectId) {
    this.log(`Subscribed to /user/queue/champselect${champselectId}`);
    this.champselectsub = this.stomp.subscribe(
      `/user/queue/champselect${champselectId}`,
      message => {
        this.recievedChampSelectMessage(message);
      }
    );
  }

  disconnectFromSystem() {
    this.log(`Unsubscribed from /user/queue/system`);
    this.systemsub.unsubscribe();
    this.stomp.deactivate();
  }

  disconnectFromLobby(lobbyId) {
    this.log(`Unsubscribed from /user/queue/lobby${lobbyId}`);
    this.lobbysub.unsubscribe();
  }

  disconnectFromChampSelect(champselectId) {
    this.log(`Unsubscribed from /user/queue/champselect${champselectId}`);
    this.champselectsub.unsubscribe();
  }

  /* EVENTS */
  afterConnect() {
    // HANDLE STUFF IF NEED BE AFTER CONNECTION
  }

  /* UTILITIES */
  parseMessage(message) {
    return JSON.parse(message.body);
  }
  generateMessageCallback(callback, message) {
    const id = uuid();
    this.callbacks[id] = { callback: callback, originalMessage: message };
    return id;
  }
  log(message, prefix = null) {
    const sender = new Error().stack
      .split("\n")[2]
      .replace(" at ", "")
      .trim();
    if (prefix) {
      console.group([`${prefix}`, `[${sender}]`]);
    } else {
      console.group(`[${sender}]`);
    }
    console.log(message);
    console.groupEnd();
  }
  recievedError(error) {
    this.log(error, "[STOMP][ERROR]");
  }

  startGame(data) {
    const { spawn } = require("child_process");
    let executablePath = this.state.config.path.client;
    // let parameters = [
    //   '"" "" "" "' +
    //     data.ip +
    //     " " +
    //     data.port +
    //     " " +
    //     data.blowfish +
    //     " " +
    //     data.playerId
    // ];

    // const sub = spawn(executablePath, parameters, {
    //   detached: true,
    //   stdio: "ignore"
    // });

    // sub.unref();

    spawn(
      "cmd",
      [
        "/c",
        "start",
        "",
        executablePath,
        "8394",
        "LoLLauncher.exe",
        "",
        data.ip + " " + data.port + " " + data.blowfish + " " + data.playerId
      ],
      {
        cwd: path.dirname(executablePath)
      }
    );
  }

  /* MESSAGES */
  recievedSystemMessage(message) {
    const data = this.parseMessage(message);
    console.group("[RECIEVER][SYSTEM]", data.messageType);
    console.log(this.stomp.subscriptions);

    if (data.id !== null) {
      const pendingMessage = this.callbacks[data.id];
      pendingMessage.callback(data.data, data.error, data.messageTimestamp);
      // TODO: This is a temporary fix to deny lobbies which don't exist anymore. We need to do proper error handling here though. See https://git.jandev.de/leagues4/ls4-launcher/-/issues/91
      if (!data.error || data.messageType === "LOBBY_DENY") {
        console.groupEnd();
        this.handleSystemMessage(data, pendingMessage.originalMessage);
      }
      delete this.callbacks[data.id];
      console.groupEnd();
      return;
    }

    switch (data.messageType) {
      case "FRIEND_LIST":
        console.log(data);
        this.dispatch("socketRecievedFriendList", data.data);
        break;

      case "FRIEND_OUT":
        console.log(data);
        this.dispatch("socketRecievedFriendOut", data.data);
        break;

      case "FRIEND_IN":
        console.log(data);
        this.dispatch("socketRecievedFriendIn", data.data);
        break;

      case "FRIEND_REMOVE":
        console.log(data);
        this.dispatch("socketExtraHandler", {
          commit: "socketDelFriend",
          data: data.data
        });
        break;

      case "FRIEND_IN_ACCEPT":
        console.log(data);
        this.dispatch("socketRecievedFriendInAccept", data.data);
        break;

      case "FRIEND_IN_DENY":
        console.log(data);
        this.dispatch("socketRecievedFriendInDeny", data.data);
        break;

      case "USER_UPDATE_SUMMONER_NAME":
        console.log(data);
        this.dispatch("socketRecievedSummonerNameUpdate", data.data);
        break;

      case "MESSAGE_PRIVATE":
        if (!this.state.privateChat.chatMessages[data.data.from]) {
          this.sendSystemMessage(
            "MESSAGE_PRIVATE_GET",
            { data: { summonerName: data.data.from }, denyHandler: true },
            (response, error) => {
              if (error) {
                console.log("Flyback error:");
                console.log(error);
              }

              response.forEach((message, index) => {
                if (index != response.length - 1) {
                  message.read = true;
                } else {
                  message.read = false;
                }
              });

              console.log(response);

              this.dispatch("socketExtraHandler", {
                commit: "socketSetChatMessagesForRecipient",
                data: {
                  messages: { data: response },
                  recipient: { summonerName: data.data.from },
                  sound: true
                }
              });
            }
          );
        } else {
          this.dispatch("socketRecievedPrivateMessage", data.data);
        }
        break;

      case "FRIEND_UPDATE":
        console.log(data);
        this.dispatch("socketRecievedFriendUpdate", data.data);
        break;

      case "LOBBY_INVITE":
        console.log(data);
        this.dispatch("socketRecievedLobbyInvite", data.data);

        this.vm.$sound.template("OVERVIEW_INVITE");
        break;

      case "LOBBY_INVITE_REVOKE":
        console.log(data.data);
        this.dispatch("socketReceivedLobbyInviteRevoke", data.data);
        break;

      case "USER_LOGGED_IN_ANOTHER_LOCATION":
        if (!this.isFirstMessage) {
          ipcRenderer.send("closeApp");
        }
        break;

      case "USER_BAN":
        ipcRenderer.send("closeApp");
        break;

      case "KILL":
        ipcRenderer.send("closeApp");
        break;

      default:
        console.log(data);
        throw new Error(
          `Could not handle unhandled message type: ${data.messageType}`
        );
    }
    console.groupEnd();
  }

  recievedLobbyMessage(message) {
    const data = this.parseMessage(message);
    console.group("[RECIEVER][LOBBY]", data.messageType);

    if (data.id !== null) {
      const pendingMessage = this.callbacks[data.id];
      pendingMessage.callback(data.data, data.error);
      if (!data.error) {
        console.groupEnd();
        this.handleLobbyMessage(data, pendingMessage.originalMessage);
      }
      delete this.callbacks[data.id];
      console.groupEnd();
      return;
    }

    switch (data.messageType) {
      case "LOBBY_CHAT":
        this.dispatch("socketRecievedLobbyChat", data.data);
        break;

      case "LOBBY_UPDATE":
        console.log(data.data);
        if (
          this.state.lobby &&
          this.state.lobby.inQueue == false &&
          data.data.inQueue == true
        ) {
          this.dispatch("setLobbyTimers");
        } else if (
          this.state.lobby &&
          this.state.lobby.inQueue == true &&
          data.data.inQueue == false
        ) {
          this.dispatch("clearLobbyTimers");
        }

        if (!this.state.lobby.isCustom && data.data.isCustom) {
          router.push("/lobbyCustom");
        } else if (this.state.lobby.isCustom && !data.data.isCustom) {
          router.push("/lobby");
        }

        this.dispatch("socketRecievedLobbyUpdate", data.data);

        break;

      case "LOBBY_KICK":
        router.push("/LoggedIn/home");
        this.disconnectFromLobby(this.state.lobby.uuid);
        this.dispatch("socketKickedFromLobby");
        break;

      case "LOBBY_MATCH_FOUND":
        console.log(data.data);
        this.vm.$modal.matchmaking.show();
        break;

      case "LOBBY_MATCH_FOUND_UPDATE":
        console.log(data.data);
        // let dummyData = {
        //   "accepted": 4,
        //   "pending": 4,
        //   "denied": 2
        // }
        this.dispatch("socketRecievedMatchFoundUpdate", data.data);
        break;

      case "LOBBY_CHAMPSELECT_SUBSCRIBE":
        console.log(data.data);
        this.connectToChampSelect(data.data.uuid);
        this.dispatch("socketSetUserChampSelect", data.data);
        router.push("/champselect");
        break;

      default:
        console.log(data);
        throw new Error(
          `Could not handle unhandled message type: ${data.messageType}`
        );
    }
    console.groupEnd();
  }

  recievedChampSelectMessage(message) {
    const data = this.parseMessage(message);
    console.group("[RECIEVER][CHAMPSELECT]", data.messageType);

    if (data.id !== null) {
      const pendingMessage = this.callbacks[data.id];
      pendingMessage.callback(data.data, data.error);
      if (!data.error) {
        console.groupEnd();
        this.handleChampSelectMessage(data, pendingMessage.originalMessage);
      }
      delete this.callbacks[data.id];
      console.groupEnd();
      return;
    }

    switch (data.messageType) {
      case "CHAMPSELECT_CHAT":
        this.dispatch("socketRecievedChampSelectChat", data.data);
        break;

      case "CHAMPSELECT_UPDATE":
        console.log(data.data);
        this.dispatch("socketSetUserChampSelect", data.data);
        break;

      case "CHAMPSELECT_ABANDON":
        this.disconnectFromChampSelect(this.state.champselect.uuid);
        this.dispatch("socketExtraHandler", {
          commit: "socketClearChampSelectData",
          data: null
        });
        if (this.state.lobby && this.state.lobby.isCustom) {
          router.push("/lobbyCustom");
        } else {
          router.push("/Lobby");
        }
        break;

      case "CHAMPSELECT_GAME_START":
        this.disconnectFromChampSelect(this.state.champselect.uuid);
        this.dispatch("socketExtraHandler", {
          commit: "socketClearChampSelectData",
          data: null
        });
        this.disconnectFromLobby(this.state.lobby.uuid);
        this.dispatch("socketExtraHandler", {
          commit: "socketClearLobbyData",
          data: null
        });
        router.push("/LoggedIn/home");

        if (data.data && data.data.ip) {
          this.startGame(data.data);
        }
        break;

      default:
        console.log(data);
        throw new Error(
          `Could not handle unhandled message type: ${data.messageType}`
        );
    }
    console.groupEnd();
  }

  /* SENDERS */
  sendSystemMessage(type, params, flyback = null) {
    console.group("[SENDER][SYSTEM]", type);
    let message = new Object();
    let callback;
    let payload;
    switch (type) {
      case "FRIEND_OUT":
        if (
          this.state.friendIn.filter(friend => {
            return friend.summonerName == params.data;
          }).length > 0
        ) {
          const userFix = this.state.friendIn.filter(friend => {
            return friend.summonerName == params.data;
          })[0];

          this.dispatch("socketExtraHandler", {
            commit: "socketAddFriend",
            data: userFix
          });
          this.dispatch("socketExtraHandler", {
            commit: "socketDelFriendIn",
            data: userFix
          });
          if (flyback) {
            callback = flyback;
            payload = params;
          } else {
            callback = () => {
              console.log("Recieved response");
            };
          }
        } else {
          if (flyback) {
            callback = flyback;
            payload = params;
          } else {
            callback = () => {
              console.log("Recieved response");
            };
          }
        }
        break;

      case "FRIEND_IN_ACCEPT":
        if (flyback) {
          callback = flyback;
          payload = { data: params.data.summonerName };
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }

        break;

      case "FRIEND_IN_DENY":
        if (flyback) {
          callback = flyback;
          payload = { data: params.data.summonerName };
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "FRIEND_REMOVE":
        if (flyback) {
          callback = flyback;
          payload = { data: params.data.summonerName };
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "FRIEND_BLOCK":
        if (flyback) {
          callback = flyback;
          payload = { data: params.data.summonerName };
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "MESSAGE_PRIVATE_GET":
        if (flyback) {
          callback = flyback;
          payload = { data: params.data.summonerName };
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "MESSAGE_PRIVATE":
        if (flyback) {
          callback = flyback;
          payload = {
            data: { to: params.data.summonerName, data: params.message }
          };
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "USER_UPDATE_ICON":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "USER_UPDATE_MOTTO":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "USER_UPDATE_STATUS":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "USER_BAN":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_CREATE":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_ACCEPT":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_DENY":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      default:
        throw new Error(`Could not send unhandled message type: ${type}`);
    }

    const messageCallbackId = this.generateMessageCallback(callback, {
      ...params,
      messageType: type
    });
    message = { ...payload, id: messageCallbackId, messageType: type };
    this.stomp.publish({
      destination: "/out/system",
      body: JSON.stringify(message)
    });
    console.groupEnd();
  }

  sendLobbyMessage(type, params, flyback = null) {
    console.group("[SENDER][LOBBY]", type);
    let message = new Object();
    let callback;
    let payload;
    switch (type) {
      case "LOBBY_CHAT":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_INVITE":
        if (flyback) {
          callback = flyback;
          let summ = params.data.summonerName;
          payload = { data: summ };
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_KICK":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_CHANGE_TYPE":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_LEAVE":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_MATCHMAKING_START":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_MATCHMAKING_STOP":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_SWITCH_TEAM":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_CHAMPSELECT_ACCEPT":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "LOBBY_CHAMPSELECT_DENY":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      default:
        throw new Error(`Could not send unhandled message type: ${type}`);
    }

    const messageCallbackId = this.generateMessageCallback(callback, {
      ...params,
      messageType: type
    });
    message = { ...payload, id: messageCallbackId, messageType: type };
    this.stomp.publish({
      destination: "/out/lobby",
      body: JSON.stringify(message)
    });
    console.groupEnd();
  }

  sendChampSelectMessage(type, params, flyback = null) {
    console.group("[SENDER][CHAMPSELECT]", type);
    let message = new Object();
    let callback;
    let payload;
    switch (type) {
      case "CHAMPSELECT_CHAT":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "CHAMPSELECT_SELECT_CHAMPION":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "CHAMPSELECT_SELECT_SKIN":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "CHAMPSELECT_LOCK_CHAMPION":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "CHAMPSELECT_BAN_SELECT_CHAMPION":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "CHAMPSELECT_BAN_LOCK_CHAMPION":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "CHAMPSELECT_SELECT_SPELL_1":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "CHAMPSELECT_SELECT_SPELL_2":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "CHAMPSELECT_TRADE_REQUEST":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "CHAMPSELECT_TRADE_ACCEPT":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      case "CHAMPSELECT_TRADE_DENY":
        if (flyback) {
          callback = flyback;
          payload = params;
        } else {
          callback = () => {
            console.log("Recieved response");
          };
        }
        break;

      default:
        throw new Error(`Could not send unhandled message type: ${type}`);
    }

    const messageCallbackId = this.generateMessageCallback(callback, {
      ...params,
      messageType: type
    });
    message = { ...payload, id: messageCallbackId, messageType: type };
    this.stomp.publish({
      destination: "/out/champselect",
      body: JSON.stringify(message)
    });
    console.groupEnd();
  }

  /* HANDLERS */
  handleSystemMessage(data, original) {
    if (original.denyHandler == true) {
      this.log("HANDLER IMPLICITLY RETURNED");
      console.groupEnd();
      return;
    }
    // gets the response as 1st param
    // gets the original message as 2nd param
    console.group("[HANDLER][SYSTEM]", data.messageType);
    switch (data.messageType) {
      case "FRIEND_OUT":
        axios
          .get(
            `${this.api.host}:${this.api.port}/users/${original.data}`,
            this.auth_headers
          )
          .then(response => {
            console.log(response.data);
            if (
              this.state.friendList.filter(friend => {
                return friend.summonerName == response.data.summonerName;
              }).length == 0
            ) {
              this.dispatch("socketExtraHandler", {
                commit: "socketAddFriendOut",
                data: response.data
              });
            }
          })
          .catch(err => {
            throw err;
          });
        break;

      case "FRIEND_IN_ACCEPT":
        console.log(original.data);
        this.dispatch("socketExtraHandler", {
          commit: "socketAddFriend",
          data: original.data
        });
        this.dispatch("socketExtraHandler", {
          commit: "socketDelFriendIn",
          data: original.data
        });
        break;

      case "FRIEND_IN_DENY":
        console.log(original.data);
        this.dispatch("socketExtraHandler", {
          commit: "socketDelFriendIn",
          data: original.data
        });
        break;

      case "FRIEND_REMOVE":
        console.log(original.data);
        this.dispatch("socketExtraHandler", {
          commit: "socketDelFriend",
          data: original.data
        });
        this.dispatch("socketExtraHandler", {
          commit: "socketDelFriendOut",
          data: original.data
        });
        break;

      case "FRIEND_BLOCK":
        console.log(original.data);
        this.dispatch("socketExtraHandler", {
          commit: "socketDelFriend",
          data: original.data
        });
        break;

      case "MESSAGE_PRIVATE_GET":
        data.data.forEach(message => {
          message.read = true;
        });
        this.dispatch("socketExtraHandler", {
          commit: "socketSetChatMessagesForRecipient",
          data: { messages: data, recipient: original.data }
        });
        break;

      case "MESSAGE_PRIVATE":
        this.dispatch("socketExtraHandler", {
          commit: "socketAddChatMessageToRecipient",
          data: {
            data: original.message,
            from: original.from,
            to: original.data.summonerName,
            messageTimestamp: data.messageTimestamp,
            read: true
          }
        });
        break;

      case "LOBBY_CREATE":
        this.connectToLobby(data.data.uuid);
        this.dispatch("socketExtraHandler", {
          commit: "socketSetUserLobby",
          data: data.data
        });
        break;

      case "LOBBY_ACCEPT":
        this.connectToLobby(data.data.uuid);
        this.dispatch("socketExtraHandler", {
          commit: "socketSetUserLobby",
          data: data.data
        });
        this.dispatch("socketExtraHandler", {
          commit: "socketRemoveInvite",
          data: original.data
        });
        break;

      case "LOBBY_DENY":
        this.dispatch("socketExtraHandler", {
          commit: "socketRemoveInvite",
          data: original.data
        });
        break;

      default:
        console.log("No extra system handlers were included");
        break;
    }
    console.groupEnd();
  }

  handleLobbyMessage(data, original) {
    if (original.denyHandler == true) {
      this.log("HANDLER IMPLICITLY RETURNED");
      console.groupEnd();
      return;
    }
    // gets the response as 1st param
    // gets the original message as 2nd param
    console.group("[HANDLER][LOBBY]", data.messageType);
    switch (data.messageType) {
      case "LOBBY_LEAVE":
        this.disconnectFromLobby(this.state.lobby.uuid);
        this.dispatch("socketExtraHandler", {
          commit: "socketClearLobbyData",
          data: null
        });
        break;

      case "LOBBY_ACCEPT":
        if (this.state.lobby.uuid) {
          this.disconnectFromLobby(this.state.lobby.uuid);
        }
        break;

      default:
        console.log("No extra lobby handlers were included");
        console.log(data);
        break;
    }
    console.groupEnd();
  }

  handleChampSelectMessage(data, original) {
    if (original.denyHandler == true) {
      this.log("HANDLER IMPLICITLY RETURNED");
      console.groupEnd();
      return;
    }
    // gets the response as 1st param
    // gets the original message as 2nd param
    console.group("[HANDLER][CHAMPSELECT]", data.messageType);
    switch (data.messageType) {
      default:
        console.log("No extra lobby handlers were included");
        console.log(data);
        break;
    }
    console.groupEnd();
  }
}
