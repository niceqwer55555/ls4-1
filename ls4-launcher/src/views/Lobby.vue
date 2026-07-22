<template>
  <div id="Lobby">
    <div class="content">
      <div class="mainblock">
        <div class="lobby">
          <div class="players lolblock">
            <div class="head">
              {{
                tWithParams("LOBBY_YOUR_TEAM_QUEUE", {
                  queueNumber,
                  serverCount
                })
              }}
            </div>
            <div class="inner">
              <Player
                v-for="(player, index) in players"
                :key="index"
                :summoner="player"
              />
              <div class="actions">
                <button class="cancelBtn" v-if="!inQueue" @click="leaveLobby">
                  {{ t("LOBBY_CANCEL_BTN") }}
                </button>
                <button
                  v-if="!inQueue && owner"
                  class="changeBtn"
                  @click="$router.push('/LoggedIn/play')"
                >
                  {{ t("LOBBY_CHANGE_LOBBY_TYPE") }}
                </button>

                <button
                  v-if="!owner && inQueue"
                  class="startBtn"
                  @click="stopQueue"
                >
                  {{ t("LOBBY_STOP_QUEUE") }}
                </button>
                <button
                  v-if="owner"
                  class="startBtn"
                  @click="inQueue ? stopQueue() : startQueue()"
                >
                  {{ inQueue ? t("LOBBY_STOP_QUEUE") : t("LOBBY_START_QUEUE") }}
                </button>
              </div>
            </div>
          </div>
          <div class="game">
            <div class="map lolblock">
              <div class="head">
                {{ t("LOBBY_MAP_OPTIONS_TITLE") }}
              </div>
              <div class="inner">
                <img :src="getMapPreview()" class="preview" alt="" />
                <div class="details">
                  <p>
                    <strong>{{ t("LOBBY_MAP_OPTIONS_LABEL_MAP") }}:</strong>
                    {{ getMapType() }}
                  </p>
                  <p>
                    <strong
                      >{{ t("LOBBY_MAP_OPTIONS_LABEL_TEAMSIZE") }}:</strong
                    >
                    {{ getTeamSize() }}
                  </p>
                  <p>
                    <strong
                      >{{ t("LOBBY_MAP_OPTIONS_LABEL_QUEUETYPE") }}:</strong
                    >
                    {{ getQueueType() }}
                  </p>
                </div>
            </div>
          </div>
          <div class="runes lolblock">
            <div class="head">
              符文 / 天赋
            </div>
            <div class="inner">
              <div class="select-row">
                <label>符文页</label>
                <select v-model="selectedRunePage" @change="onRunePageChange">
                  <option
                    v-for="(page, index) in runePages"
                    :key="'rune-' + index"
                    :value="index"
                  >
                    {{ page.name || ('符文页 ' + (index + 1)) }}
                  </option>
                </select>
              </div>
              <div class="select-row">
                <label>天赋页</label>
                <select v-model="selectedMasteryPage" @change="onMasteryPageChange">
                  <option
                    v-for="(page, index) in masteryPages"
                    :key="'mastery-' + index"
                    :value="index"
                  >
                    {{ page.name || ('天赋页 ' + (index + 1)) }}
                  </option>
                </select>
              </div>
              <div class="edit-row">
                <button class="editBtn" @click="openRuneMasteryEditor">编辑符文/天赋</button>
              </div>
            </div>
          </div>
          <div class="invites lolblock">
            <div class="head">
                {{ t("LOBBY_INVITES_TITLE") }}
              </div>
              <div class="inner">
                <div
                  class="invite"
                  v-for="(invite, index) in lobbyInvites"
                  :key="index"
                >
                  <span>{{ index }}</span>
                  <span :class="invite.toLowerCase()">
                    {{ getInviteStatus(invite) }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="chat lolblock">
          <div class="head">
            {{ t("LOBBY_ALLCHAT_TITLE") }}
          </div>
          <div class="inner">
            <div class="chat-messages" ref="chatMessages">
              <ChatMessage
                v-for="(message, index) in lobbyChatMessages"
                :key="index"
                :message="message"
              />
            </div>
            <div class="chat-input">
              <input
                type="text"
                ref="chatInput"
                v-on:keyup.enter="sendLobbyChatMessage"
              />
              <button @click="sendLobbyChatMessage">
                {{ t("LOBBY_ALLCHAT_SEND_BTN") }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
    <SideBar />
  </div>
</template>

<script>
import Player from "@/components/Lobby/Player.vue";
import SideBar from "@/components/SideBar.vue";
import ChatMessage from "@/components/Lobby/ChatMessage.vue";
import { mapState } from "vuex";
import moment from "moment";

export default {
  components: {
    Player,
    ChatMessage,
    SideBar
  },
  data() {
    return {
      lastMessages: null,
      selectedRunePage: 0,
      selectedMasteryPage: 0
    };
  },
  computed: mapState({
    players: state => state.lobby.members,
    lobbyChatMessages: state => state.lobbyChatMessages,
    lobbyInvites: state => state.lobby.invited,
    inQueue: state => state.lobby.inQueue,
    owner: state => state.lobbyOwner,
    queueNumber: state => state.lobbyQueueCount,
    serverCount: state => state.serverCount,
    runePages: state => state.runePages,
    masteryPages: state => state.masteryPages,
    currentRunePage: state => state.currentRunePage,
    currentMasteryPage: state => state.currentMasteryPage
  }),
  methods: {
    onRunePageChange() {
      this.$store.dispatch("selectRunePage", this.selectedRunePage);
    },
    onMasteryPageChange() {
      this.$store.dispatch("selectMasteryPage", this.selectedMasteryPage);
    },
    openRuneMasteryEditor() {
      this.$store.dispatch("setRuneMasteryEditorVisible", true);
    },
    getInviteStatus(status) {
      // return status.charAt(0).toUpperCase() + status.slice(1).toLowerCase();
      return this.$translate.text(`LOBBY_INVITE_STATUS_${status}`);
    },
    inLobby(invite) {
      return (
        this.$store.state.lobby.members.filter(member => {
          return member.summonerName == invite.summonerName;
        }).length > 0
      );
    },
    getQueueType() {
      switch (this.$store.state.lobby.lobbyType) {
        case "SUMMONERS_RIFT_DRAFT":
        case "TWISTED_TREELINE_DRAFT":
        case "SUMMONERS_RIFT_BOT_DRAFT":
        case "ODIN_DRAFT":
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_QT_DRAFT");
        case "SUMMONERS_RIFT_BLIND":
        case "TWISTED_TREELINE_BLIND":
        case "SUMMONERS_RIFT_BOT_BLIND":
        case "ODIN_BLIND":
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_QT_BLIND");
        case "ARAM_BLIND":
        case "ARAM_BOT_ARAM":
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_QT_ARAM");
      }
    },
    getMapPreview() {
      switch (this.$store.state.lobby.lobbyType) {
        case "SUMMONERS_RIFT_DRAFT":
        case "SUMMONERS_RIFT_BLIND":
        case "SUMMONERS_RIFT_BOT_DRAFT":
        case "SUMMONERS_RIFT_BOT_BLIND":
          return "static/images/general/sr-preview.png";
        case "TWISTED_TREELINE_DRAFT":
        case "TWISTED_TREELINE_BLIND":
          return "static/images/general/tt-preview.png";
        case "ARAM_BLIND":
        case "ARAM_BOT_ARAM":
          return "static/images/general/ha-preview.png";
        case "ODIN_BLIND":
        case "ODIN_DRAFT":
          return "static/images/general/odin-preview.png";
      }
    },
    getMapType() {
      switch (this.$store.state.lobby.lobbyType) {
        case "SUMMONERS_RIFT_DRAFT":
        case "SUMMONERS_RIFT_BLIND":
        case "SUMMONERS_RIFT_BOT_DRAFT":
        case "SUMMONERS_RIFT_BOT_BLIND":
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_SR");
        case "TWISTED_TREELINE_DRAFT":
        case "TWISTED_TREELINE_BLIND":
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_TT");
        case "ARAM_BLIND":
        case "ARAM_BOT_ARAM":
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_HA");
        case "ODIN_BLIND":
        case "ODIN_DRAFT":
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_ODIN");
      }
    },
    getTeamSize() {
      switch (this.$store.state.lobby.lobbyType) {
        case "SUMMONERS_RIFT_DRAFT":
        case "SUMMONERS_RIFT_BLIND":
        case "ARAM_BLIND":
        case "ODIN_BLIND":
        case "ODIN_DRAFT":
          return "5x5";
        case "TWISTED_TREELINE_DRAFT":
        case "TWISTED_TREELINE_BLIND":
          return "3x3";
      }
    },
    getQueueTimer() {
      let format = "";
      if (this.$store.state.lobbyQueueTime >= 3600) {
        format = "HH:mm:ss";
      } else {
        format = "mm:ss";
      }

      return moment.utc(this.$store.state.lobbyQueueTime * 1000).format(format);
    },
    sendLobbyChatMessage() {
      const message = this.$refs.chatInput.value;
      if (message.trim() == "") return;
      this.$refs.chatInput.value = "";

      this.$socket.sendLobbyMessage(
        "LOBBY_CHAT",
        { data: message },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          this.$refs.chatMessages.scrollTop = this.$refs.chatMessages.scrollHeight;
        }
      );
    },
    leaveLobby() {
      const route = this.$route.name;
      const router = this.$router;
      this.$socket.sendLobbyMessage("LOBBY_LEAVE", {}, (response, error) => {
        if (error) {
          console.log("Flyback error:");
          console.log(error);
        }
        if (route != "Home") {
          router.push("/LoggedIn/home");
        }
      });
    },
    startQueue() {
      if (this.inQueue) return;

      this.$sound.template("CORE_QUEUE_START");

      this.$socket.sendLobbyMessage(
        "LOBBY_MATCHMAKING_START",
        {
          data: {
            runePage: this.runePages[this.currentRunePage] || null,
            masteryPage: this.masteryPages[this.currentMasteryPage] || null
          }
        },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);

            if (error && error.message && error.code === 140) {
              // TODO: {SummonerName=4:53, Summoner Name2=33:34}
              const dodgersRaw = error.message.replace(/[{ }]/g, "").split(",");
              let worstDodger = null;
              dodgersRaw.forEach(dodger => {
                const timeRaw = dodger.split("=")[1];
                let splitTime = timeRaw.split(":");
                const time =
                  parseInt(splitTime[0] * 60) + parseInt(splitTime[1]);
                if (worstDodger == null || worstDodger.secnum < dodger.secnum) {
                  worstDodger = {
                    name: dodger.split("=")[0],
                    time: dodger.split("=")[1],
                    secnum: time
                  };
                }
              });

              this.$modal.setter.show({
                title: "Somebody in your team dodged!",
                body: `<center>${worstDodger.name} dodged and is banned from queueing for ${worstDodger.time}</center><br>`,
                footer: "This info panel only shows the highest timer!"
              });
            }
          } else {
            this.$sound.template("CORE_MUSIC_QUEUE");
          }
        }
      );

      this.$store.dispatch("getQueueCount");
      this.$store.dispatch("getServerCount");
    },
    stopQueue() {
      this.$sound.template("OVERVIEW_CLICK");
      this.$store.dispatch("clearLobbyTimers");

      this.$socket.sendLobbyMessage(
        "LOBBY_MATCHMAKING_STOP",
        {},
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          this.$store.dispatch("setLobbyQueueState", false);

          console.log(response);
        }
      );

      this.$sound.stopAllLoops("CORE_MUSIC");
      this.$store.dispatch("getQueueCount");
      this.$store.dispatch("getServerCount");
    }
  },
  beforeMount() {
    this.$store.dispatch("saveUserStatus", "IN_LOBBY");
    this.$store.dispatch("changeBackgroundState", "LOBBY");
  },
  updated() {
    if (this.lastMessages !== this.lobbyChatMessages.length) {
      this.lastMessages = this.lobbyChatMessages.length;
      this.$refs.chatMessages.scrollTop = this.$refs.chatMessages.scrollHeight;
    }
  },
  created() {
    this.$store.dispatch('getQueueCount');
    this.$store.dispatch('getServerCount');
    this.$store.dispatch('loadRuneMasteryPages');
  }
};
</script>

<style lang="css" scoped>
#Lobby {
  width: 100%;
  height: calc(100% - 115px);
  margin-top: 77px;
  position: relative;
  z-index: 0;
  display: flex;
}

#Lobby .content {
  width: 80%;
  position: relative;
  display: flex;
  justify-content: flex-end;
}
#Lobby .sidebar-holder {
  width: 20%;
}

.content .mainblock {
  width: 87%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

/* LOL BLOCK */

.lolblock {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
  border: 1px solid rgba(100, 117, 137, 0.75);
  border-radius: 5px;
  overflow: hidden;
}

.lolblock .head {
  height: 30px;
  width: 100%;
  font-size: 14px;
  font-family: LoLFont2;
  padding: 5px;
  background-image: linear-gradient(
    180deg,
    #192e49 0%,
    #192e49 40%,
    #172b46 50%,
    #142131 100%
  );
}

.lolblock .inner {
  background-color: rgba(5, 12, 20, 0.85);
  width: 100%;
  height: calc(100% - 30px);
}

.content .mainblock .lobby {
  width: 100%;
  height: 75%;
  display: flex;
  justify-content: space-between;
}

.content .mainblock .lobby .players {
  width: 69.5%;
  height: 100%;
}

.content .mainblock .lobby .players .inner {
  padding: 5px 7px;
  position: relative;
}

.content .mainblock .lobby .players .inner .actions {
  position: absolute;
  bottom: 0px;
  left: 0px;
  padding: 5px 10px;
  display: flex;
  height: 40px;
  width: 100%;
}

.content .mainblock .lobby .players .inner .actions button {
  background: linear-gradient(
    180deg,
    #3c73b4 0%,
    #20477e 45%,
    #1e3e6d 50%,
    #0e284b 100%
  );
  outline: none;
  border: none;
  border-radius: 2px;
  margin-right: 10px;
  padding: 5px 30px;
  border: 1px solid #304b69;
  color: white;
  font-family: LoLFont2;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.content .mainblock .lobby .players .inner .actions button:hover {
  filter: brightness(1.25);
}

.content .mainblock .lobby .players .inner .actions button.startBtn {
  background: url("../assets/images/button-default.png");
  background-size: 100% 100%;
  border: none;
  margin-left: auto;
  margin-right: 0;
}

.content .mainblock .lobby .game {
  width: 30%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.content .mainblock .lobby .game .map {
  width: 100%;
  height: 55%;
  display: flex;
  flex-direction: column;
}
.content .mainblock .lobby .game .map .inner {
  padding: 2px;
}
.content .mainblock .lobby .game .map .inner img {
  height: 155px;
  width: 100%;
  object-fit: cover;
  border: 1px solid #566676;
  object-position: center;
}

.content .mainblock .lobby .game .map .inner .details {
  height: calc(100% - 155px);
  width: 100%;
  display: flex;
  flex-direction: column;
  padding: 0 5px;
}

.content .mainblock .lobby .game .map .inner .details p {
  font-size: 12px;
  font-family: LoLFont2;
  margin: 0;
}

.content .mainblock .lobby .game .invites {
  width: 100%;
  height: 30%;
  overflow: auto;
}


.content .mainblock .lobby .game .runes {
  width: 100%;
  height: 20%;
  display: flex;
  flex-direction: column;
}

.content .mainblock .lobby .game .runes .inner {
  display: flex;
  flex-direction: column;
  padding: 5px;
  gap: 6px;
}

.content .mainblock .lobby .game .runes .inner .select-row {
  display: flex;
  flex-direction: column;
  font-family: LoLFont2;
  font-size: 12px;
}

.content .mainblock .lobby .game .runes .inner .select-row label {
  margin-bottom: 2px;
  color: rgba(255, 255, 255, 0.75);
}

.content .mainblock .lobby .game .runes .inner .select-row select {
  background: #0a1320;
  color: white;
  border: 1px solid #304b69;
  border-radius: 3px;
  padding: 3px 6px;
  font-family: LoLFont2;
  font-size: 12px;
  outline: none;
}

.content .mainblock .lobby .game .runes .inner .edit-row {
  display: flex;
  justify-content: center;
  margin-top: 2px;
}

.content .mainblock .lobby .game .runes .inner .edit-row .editBtn {
  background: linear-gradient(180deg, #3c73b4 0%, #20477e 45%, #1e3e6d 50%, #0e284b 100%);
  border: 1px solid #304b69;
  border-radius: 2px;
  color: white;
  font-family: LoLFont2;
  font-size: 11px;
  padding: 3px 12px;
  cursor: pointer;
  transition: filter ease-in-out 200ms;
}

.content .mainblock .lobby .game .runes .inner .edit-row .editBtn:hover {
  filter: brightness(1.25);
}

.content .mainblock .lobby .game .invites .inner {
  display: flex;
  flex-direction: column;
  padding: 3px;
  overflow: auto;
}

.content .mainblock .lobby .game .invites .inner .invite {
  font-size: 13px;
  font-family: LoLFont2;
  display: flex;
  justify-content: space-between;
  padding: 0 5px;
  margin-bottom: 5px;
}

.content .mainblock .lobby .game .invites .inner .invite .accepted {
  color: #306d32;
}

.content .mainblock .lobby .game .invites .inner .invite .pending {
  color: #c8a91a;
}

.content .mainblock .chat {
  width: 100%;
  height: 25%;
}

.content .mainblock .chat .inner {
  display: flex;
  flex-direction: column;
}

.content .mainblock .chat .inner .chat-messages {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 80%;
  padding: 0 3px;
  overflow: auto;
}

.content .mainblock .chat .inner .chat-input {
  width: 100%;
  height: 20%;
  display: flex;
  justify-content: space-between;
  padding: 0 2px;
}

.content .mainblock .chat .inner .chat-input input {
  border: 1px solid #404549;
  border-radius: 3px;
  box-shadow: inset 0 0 5px 1px #1c2932;
  width: 89.5%;
  outline: none;
}

.content .mainblock .chat .inner .chat-input button {
  width: 10%;
  background: linear-gradient(
    180deg,
    #3c73b4 0%,
    #20477e 45%,
    #1e3e6d 50%,
    #0e284b 100%
  );
  outline: none;
  border: none;
  border-radius: 2px;
  text-align: center;
  border: 1px solid #304b69;
  color: white;
  font-family: LoLFont2;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.content .mainblock .chat .inner .chat-input button:hover {
  filter: brightness(1.25);
}
</style>