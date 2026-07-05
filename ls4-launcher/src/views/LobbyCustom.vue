<template>
  <div id="LobbyCustom">
    <div class="content">
      <div class="mainblock">
        <div class="lobby">
          <div class="players">
            <div class="team lolblock">
              <div class="head">
                {{
                  tWithParams("CTLOBBY_TEAM1_TITLE", {
                    serverCount
                  })
                }}
              </div>
              <div class="inner">
                <Player
                  v-for="(player, index) in players1"
                  :key="index"
                  :summoner="player"
                  :custom="true"
                />
                <BotPlayer
                  v-for="bot in bots1"
                  :key="'bot1-' + bot.botId"
                  :bot="bot"
                  :canRemove="owner"
                  @remove="removeBot"
                />
                <div class="addBot" v-if="canAddBot(1)">
                  <button @click="openBotModal(1)">
                    + {{ t("CTLOBBY_ADD_BOT_BTN") }}
                  </button>
                  <button class="fillBtn" @click="fillTeamWithBots(1)">
                    {{ t("CTLOBBY_FILL_TEAM_BTN") }}
                  </button>
                </div>
                <div v-if="canJoinTeam(1)" class="joinTeam">
                  <button @click="switchTeam">
                    {{ t("CTLOBBY_JOIN_TEAM_BTN") }}
                  </button>
                </div>
              </div>
            </div>
            <div class="team lolblock">
              <div class="head">
                {{ t("CTLOBBY_TEAM2_TITLE") }}
              </div>
              <div class="inner">
                <Player
                  v-for="(player, index) in players2"
                  :key="index"
                  :summoner="player"
                  :custom="true"
                />
                <BotPlayer
                  v-for="bot in bots2"
                  :key="'bot2-' + bot.botId"
                  :bot="bot"
                  :canRemove="owner"
                  @remove="removeBot"
                />
                <div class="addBot" v-if="canAddBot(2)">
                  <button @click="openBotModal(2)">
                    + {{ t("CTLOBBY_ADD_BOT_BTN") }}
                  </button>
                  <button class="fillBtn" @click="fillTeamWithBots(2)">
                    {{ t("CTLOBBY_FILL_TEAM_BTN") }}
                  </button>
                </div>
                <div v-if="canJoinTeam(2)" class="joinTeam">
                  <button @click="switchTeam">
                    {{ t("CTLOBBY_JOIN_TEAM_BTN") }}
                  </button>
                </div>
              </div>
            </div>
            <div class="actions">
              <button class="quitBtn" @click="leaveLobby">
                {{ t("CTLOBBY_QUIT_GAME_BTN") }}
              </button>
              <button class="changeBtn" @click="$router.push('/LoggedIn/play')">
                {{ t("CTLOBBY_CHANGE_LOBBY_TYPE_BTN") }}
              </button>
              <button class="startBtn" v-if="owner" @click="startQueue">
                {{ t("CTLOBBY_START_GAME_BTN") }}
              </button>
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
                  <strong>{{ t("LOBBY_MAP_OPTIONS_LABEL_TEAMSIZE") }}:</strong>
                  {{ getTeamSize() }}
                </p>
                <p>
                  <strong>{{ t("LOBBY_MAP_OPTIONS_LABEL_QUEUETYPE") }}:</strong>
                  {{ getQueueType() }}
                </p>
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
    </div>
    <SideBar />
    <div class="botModal" v-if="botModalVisible" tabindex="-1">
      <div class="head">
        {{ t("BOT_MODAL_TITLE") }}
      </div>
      <div class="body">
        <div class="section">
          <div class="section-title">{{ t("BOT_MODAL_SELECT_CHAMPION") }}</div>
          <div class="iconlist">
            <div
              class="icon"
              v-for="champ in champions"
              :key="champ.id"
              :class="{ selected: isChampionSelected(champ.id) }"
              @click="selectChampion(champ.id)"
            >
              <img :src="getChampionImgUrl(champ.id)" :alt="champ.displayName" />
            </div>
          </div>
        </div>
        <div class="section selects">
          <div class="select-row">
            <label>{{ t("BOT_MODAL_SELECT_ROLE") }}</label>
            <select v-model="botForm.role">
              <option
                v-for="role in botRoles"
                :key="role"
                :value="role"
                :disabled="isRoleTaken(role)"
              >
                {{ t("BOT_ROLE_" + role.toUpperCase()) }}
              </option>
            </select>
          </div>
          <div class="select-row">
            <label>{{ t("BOT_MODAL_SELECT_DIFFICULTY") }}</label>
            <select v-model="botForm.difficulty">
              <option value="EASY">{{ t("BOT_DIFFICULTY_EASY") }}</option>
              <option value="NORMAL">{{ t("BOT_DIFFICULTY_NORMAL") }}</option>
              <option value="HARD">{{ t("BOT_DIFFICULTY_HARD") }}</option>
            </select>
          </div>
        </div>
      </div>
      <div class="foot">
        <button type="button" @click="closeBotModal">
          {{ t("BOT_MODAL_CANCEL_BTN") }}
        </button>
        <button
          type="button"
          @click="confirmAddBot"
          :disabled="!botForm.championId"
        >
          {{ t("BOT_MODAL_CONFIRM_BTN") }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import Player from "@/components/Lobby/Player.vue";
import BotPlayer from "@/components/Lobby/BotPlayer.vue";
import SideBar from "@/components/SideBar.vue";
import ChatMessage from "@/components/Lobby/ChatMessage.vue";
import { mapState } from "vuex";

export default {
  components: {
    Player,
    BotPlayer,
    ChatMessage,
    SideBar
  },
  data() {
    return {
      lastMessages: null,
      queueNumber: "-",
      botModalVisible: false,
      botRoles: ["Top", "Mid", "Jungle", "ADC", "Support"],
      botForm: {
        team: null,
        championId: null,
        role: "Top",
        difficulty: "HARD"
      }
    };
  },
  computed: {
    isTeamFull() {
      if (this.players1 && this.players2) {
        let user = this.$store.state.user.summonerName;
        let team1 = this.players1.some(c => c.summonerName == user);
        let team2 = this.players2.some(c => c.summonerName == user);

        if (team1 && this.players2.length <= 5) {
          return false;
        } else if (team2 && this.players1.length <= 5) {
          return false;
        }
      }
      return true;
    },
    ...mapState({
      players1: state =>
        state.lobby.members
          ? state.lobby.members.filter(c => c.lobbyTeam === "TEAM1")
          : undefined,
      players2: state =>
        state.lobby.members
          ? state.lobby.members.filter(c => c.lobbyTeam === "TEAM2")
          : undefined,
      bots1: state =>
        state.lobby.bots
          ? state.lobby.bots.filter(b => b.team === "TEAM1")
          : [],
      bots2: state =>
        state.lobby.bots
          ? state.lobby.bots.filter(b => b.team === "TEAM2")
          : [],
      champions: state => state.collection.champions || [],
      lobbyChatMessages: state => state.lobbyChatMessages,
      lobbyInvites: state => state.lobby.invited,
      inQueue: state => state.lobby.inQueue,
      owner: state => state.lobbyOwner,
      serverCount: state => state.serverCount,
      currentLobbyPlayer: state => {
        return state.lobby.members.filter(member => {
          return member.summonerName == state.user.summonerName;
        })[0];
      }
    })
  },
  methods: {
    canJoinTeam(team) {
      if (this.players1 && this.players2) {
        if (team == 2) {
          return (
            this.players2.length + this.bots2.length < 5 &&
            this.currentLobbyPlayer.lobbyTeam == "TEAM1" &&
            !this.isTeamFull
          );
        } else {
          return (
            this.players1.length + this.bots1.length < 5 &&
            this.currentLobbyPlayer.lobbyTeam == "TEAM2" &&
            !this.isTeamFull
          );
        }
      } else {
        return false;
      }
    },
    canAddBot(team) {
      if (!this.owner || this.inQueue) return false;
      const humans = team === 1 ? this.players1.length : this.players2.length;
      const bots = team === 1 ? this.bots1.length : this.bots2.length;
      return humans + bots < 5;
    },
    openBotModal(team) {
      this.botForm.team = team;
      this.botForm.championId = null;
      this.botForm.difficulty = "HARD";
      const teamBots = team === 1 ? this.bots1 : this.bots2;
      const takenRoles = teamBots.map(b => b.role);
      const firstAvailable = this.botRoles.find(r => !takenRoles.includes(r));
      this.botForm.role = firstAvailable || "Top";
      if (!this.champions || this.champions.length === 0) {
        this.$store.dispatch("getCollectionChampions");
      }
      this.botModalVisible = true;
    },
    fillTeamWithBots(team) {
      const teamBots = team === 1 ? this.bots1 : this.bots2;
      const humans = team === 1 ? this.players1.length : this.players2.length;
      const needed = 5 - humans - teamBots.length;
      if (needed <= 0) return;
      if (!this.champions || this.champions.length === 0) {
        this.$store.dispatch("getCollectionChampions");
        return;
      }

      const takenChampIds = new Set();
      this.bots1.forEach(b => takenChampIds.add(b.championId));
      this.bots2.forEach(b => takenChampIds.add(b.championId));
      const available = this.champions.filter(c => !takenChampIds.has(c.id));
      if (available.length === 0) return;

      const takenRoles = teamBots.map(b => b.role);
      const priorityOrder = ["Top", "Mid", "Jungle", "ADC", "Support"];
      const availableRoles = priorityOrder.filter(r => !takenRoles.includes(r));

      for (let i = 0; i < needed && i < availableRoles.length && i < available.length; i++) {
        const role = availableRoles[i];
        const randomChamp = available[Math.floor(Math.random() * available.length)];
        takenChampIds.add(randomChamp.id);
        const idx = available.indexOf(randomChamp);
        if (idx > -1) available.splice(idx, 1);

        const payload = {
          data: {
            championId: randomChamp.id,
            difficulty: "HARD",
            role: role,
            team: team === 1 ? "TEAM1" : "TEAM2"
          }
        };
        this.$socket.sendLobbyMessage("LOBBY_ADD_BOT", payload, () => {});
      }
    },
    isRoleTaken(role) {
      const teamBots = this.botForm.team === 1 ? this.bots1 : this.bots2;
      return teamBots.some(b => b.role === role);
    },
    closeBotModal() {
      this.botModalVisible = false;
    },
    confirmAddBot() {
      if (!this.botForm.championId) return;
      const payload = {
        data: {
          championId: this.botForm.championId,
          difficulty: this.botForm.difficulty,
          role: this.botForm.role,
          team: this.botForm.team === 1 ? "TEAM1" : "TEAM2"
        }
      };
      this.$socket.sendLobbyMessage(
        "LOBBY_ADD_BOT",
        payload,
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
            return;
          }
          this.closeBotModal();
        }
      );
    },
    removeBot(botId) {
      this.$socket.sendLobbyMessage(
        "LOBBY_REMOVE_BOT",
        { data: botId },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    },
    isChampionSelected(champId) {
      return this.botForm.championId === champId;
    },
    selectChampion(champId) {
      this.botForm.championId = champId;
    },
    getChampionImgUrl(champId) {
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/champions/${champId}.jpg`;
    },
    getInviteStatus(status) {
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
        case "ODIN_DRAFT":
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_QT_DRAFT");
        case "SUMMONERS_RIFT_BLIND":
        case "TWISTED_TREELINE_BLIND":
        case "ODIN_BLIND":
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_QT_BLIND");
        case "ARAM_BLIND":
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_QT_ARAM");
      }
    },
    getMapPreview() {
      switch (this.$store.state.lobby.lobbyType) {
        case "SUMMONERS_RIFT_DRAFT":
        case "SUMMONERS_RIFT_BLIND":
          return "static/images/general/sr-preview.png";
        case "TWISTED_TREELINE_DRAFT":
        case "TWISTED_TREELINE_BLIND":
          return "static/images/general/tt-preview.png";
        case "ARAM_BLIND":
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
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_SR");
        case "TWISTED_TREELINE_DRAFT":
        case "TWISTED_TREELINE_BLIND":
          return this.$translate.text("LOBBY_MAP_OPTIONS_VALUE_TT");
        case "ARAM_BLIND":
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

      this.$sound.template("OVERVIEW_PLAYBUTTON");

      this.$socket.sendLobbyMessage(
        "LOBBY_MATCHMAKING_START",
        {},
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);

            if (error && error.message && error.code === 140) {
              this.$store.dispatch("socketRecievedLobbyChat", {
                from: "SYSTEM",
                to: null,
                data: error.message
              });
            }
          }

          console.log(response);
        }
      );
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
    },
    switchTeam() {
      if (this.inQueue) return;

      this.$socket.sendLobbyMessage(
        "LOBBY_SWITCH_TEAM",
        {},
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    }
  },
  beforeMount() {
    this.$store.dispatch("saveUserStatus", "IN_LOBBY");
    this.$store.dispatch("changeBackgroundState", "CUSTOM_LOBBY");
  },
  updated() {
    if (this.lastMessages !== this.lobbyChatMessages.length) {
      this.lastMessages = this.lobbyChatMessages.length;
      this.$refs.chatMessages.scrollTop = this.$refs.chatMessages.scrollHeight;
    }
  },
  created() {
    this.$store.dispatch("getServerCount");
  }
};
</script>

<style lang="css" scoped>
#LobbyCustom {
  width: 100%;
  height: calc(100% - 115px);
  margin-top: 77px;
  position: relative;
  z-index: 0;
  display: flex;
}

#LobbyCustom .content {
  width: 100%;
  position: relative;
  display: flex;
  justify-content: flex-end;
}
#LobbyCustom .sidebar-holder {
  width: 20%;
  position: absolute;
  right: 0;
  top: 0;
}

.content .mainblock {
  width: 100%;
  height: 100%;
  display: flex;
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
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.content .mainblock .lobby .players {
  width: 100%;
  height: 70%;
  display: flex;
  position: relative;
}

.content .mainblock .lobby .players .team {
  width: 50%;
  height: 100%;
}

.content .mainblock .lobby .players .team .inner {
  padding: 5px;
  position: relative;
}

.content .mainblock .lobby .players .team .inner .joinTeam {
  height: 17%;
  width: 100%;
  position: relative;
}

.content .mainblock .lobby .players .team .inner .joinTeam button {
  position: absolute;
  left: 50%;
  top: 50%;
  padding: 5px 40px;
  transform: translate(-50%, -50%);
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

.content .mainblock .lobby .players .team .inner .joinTeam button:hover {
  filter: brightness(1.25);
}

.content .mainblock .lobby .players .actions {
  position: absolute;
  bottom: 0px;
  left: 50%;
  transform: translateX(-50%);
  padding: 5px 0;
  display: flex;
  height: 40px;
  width: 98%;
  border-top: 1px solid #304b69;
  background-color: rgba(5, 12, 20, 1);
}

.content .mainblock .lobby .players .actions button {
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
  padding: 5px 40px;
  border: 1px solid #304b69;
  color: white;
  font-family: LoLFont2;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.content .mainblock .lobby .players .actions button:hover {
  filter: brightness(1.25);
}

.content .mainblock .lobby .players .actions button.startBtn {
  background: url("../assets/images/button-default.png");
  background-size: 100% 100%;
  border: none;
  margin-left: auto;
  margin-right: 0;
}

.content .mainblock .lobby .chat {
  width: 100%;
  height: 30%;
}

.content .mainblock .lobby .chat .inner {
  display: flex;
  flex-direction: column;
}

.content .mainblock .lobby .chat .inner .chat-messages {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 80%;
  padding: 0 3px;
  overflow: auto;
}

.content .mainblock .lobby .chat .inner .chat-input {
  width: 100%;
  height: 20%;
  display: flex;
  justify-content: space-between;
  padding: 0 2px;
}

.content .mainblock .lobby .chat .inner .chat-input input {
  border: 1px solid #404549;
  border-radius: 3px;
  box-shadow: inset 0 0 5px 1px #1c2932;
  width: 89.5%;
  outline: none;
}

.content .mainblock .lobby .chat .inner .chat-input button {
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

.content .mainblock .lobby .chat .inner .chat-input button:hover {
  filter: brightness(1.25);
}

.content .mainblock .game {
  width: 30%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.content .mainblock .game .map {
  width: 100%;
  height: 55%;
  display: flex;
  flex-direction: column;
}
.content .mainblock .game .map .inner {
  padding: 2px;
}
.content .mainblock .game .map .inner img {
  height: 155px;
  width: 100%;
  object-fit: cover;
  border: 1px solid #566676;
  object-position: center;
}

.content .mainblock .game .map .inner .details {
  height: calc(100% - 155px);
  width: 100%;
  display: flex;
  flex-direction: column;
  padding: 0 5px;
}

.content .mainblock .game .map .inner .details p {
  font-size: 12px;
  font-family: LoLFont2;
  margin: 0;
}

.content .mainblock .game .invites {
  width: 100%;
  height: 45%;
  overflow: auto;
}

.content .mainblock .game .invites .inner {
  display: flex;
  flex-direction: column;
  padding: 3px;
  overflow: auto;
}

.content .mainblock .game .invites .inner .invite {
  font-size: 13px;
  font-family: LoLFont2;
  display: flex;
  justify-content: space-between;
  padding: 0 5px;
  margin-bottom: 5px;
}

.content .mainblock .game .invites .inner .invite .accepted {
  color: #306d32;
}

.content .mainblock .game .invites .inner .invite .pending {
  color: #c8a91a;
}

/* ADD BOT BUTTON */

.content .mainblock .lobby .players .team .inner .addBot {
  height: 17%;
  width: 100%;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  flex-wrap: wrap;
}

.content .mainblock .lobby .players .team .inner .addBot button {
  padding: 5px 18px;
  background: linear-gradient(
    180deg,
    #6b5a1e 0%,
    #4a3f16 45%,
    #3a3211 50%,
    #1a1408 100%
  );
  outline: none;
  border: none;
  border-radius: 2px;
  text-align: center;
  border: 1px solid #785f1e;
  color: #f0d070;
  font-family: LoLFont2;
  font-size: 12px;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.content .mainblock .lobby .players .team .inner .addBot button.quickAddBtn {
  background: linear-gradient(
    180deg,
    #8b2a2a 0%,
    #5a1a1a 45%,
    #4a1414 50%,
    #200808 100%
  );
  border: 1px solid #a85050;
  color: #ffb0b0;
}

.content .mainblock .lobby .players .team .inner .addBot button.fillBtn {
  background: linear-gradient(
    180deg,
    #2a6b2a 0%,
    #1a4a1a 45%,
    #143a14 50%,
    #081808 100%
  );
  border: 1px solid #50a850;
  color: #b0ffb0;
}

.content .mainblock .lobby .players .team .inner .addBot button:hover {
  filter: brightness(1.25);
}

/* BOT MODAL */

.botModal {
  position: absolute;
  left: 50%;
  top: 50%;
  width: 60%;
  height: 480px;
  border-radius: 7px;
  padding: 0 10px;
  transform: translate(-50%, -50%);
  z-index: 10 !important;
  outline: none;
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(3, 39, 82, 0.75);
  background: linear-gradient(
    90deg,
    rgba(12, 15, 19, 0.95) 100%,
    rgba(20, 24, 29, 0.95) 0%
  );
  overflow: hidden;
}

.botModal .head {
  height: 30px;
  width: 100%;
  font-size: 16px;
  font-family: LoLFont2;
  padding: 10px 5px 0px 5px;
}

.botModal .body {
  width: 100%;
  height: calc(100% - 70px);
  display: flex;
  flex-direction: column;
  padding: 10px 0;
}

.botModal .body .section {
  width: 100%;
}

.botModal .body .section.section-title,
.botModal .body .section .section-title {
  font-family: LoLFont2;
  font-size: 13px;
  color: rgba(255, 255, 255, 0.75);
  padding: 0 5px 5px 5px;
}

.botModal .body .section .iconlist {
  display: flex;
  flex-wrap: wrap;
  overflow: auto;
  max-height: 280px;
  padding: 0 5px;
}

.botModal .body .section .iconlist .icon {
  width: 60px;
  height: 60px;
  border: 1px solid rgba(255, 255, 255, 0.4);
  margin: 0 4px 8px 4px;
  cursor: pointer;
  transition: filter ease-in-out 200ms;
}

.botModal .body .section .iconlist .icon.selected {
  border: 1px solid rgba(175, 128, 0, 0.95);
  filter: brightness(1.2);
}

.botModal .body .section .iconlist .icon:hover {
  filter: brightness(1.25);
}

.botModal .body .section .iconlist .icon img {
  width: 100%;
  height: 100%;
  object-fit: contain;
}

.botModal .body .section.selects {
  display: flex;
  justify-content: space-around;
  padding: 10px 5px 0 5px;
}

.botModal .body .section.selects .select-row {
  display: flex;
  flex-direction: column;
  font-family: LoLFont2;
  font-size: 13px;
}

.botModal .body .section.selects .select-row label {
  margin-bottom: 4px;
  color: rgba(255, 255, 255, 0.75);
}

.botModal .body .section.selects .select-row select {
  background: #0a1320;
  color: white;
  border: 1px solid #304b69;
  border-radius: 3px;
  padding: 4px 8px;
  font-family: LoLFont2;
  outline: none;
  min-width: 120px;
}

.botModal .foot {
  display: flex;
  justify-content: flex-end;
  height: 40px;
  align-items: center;
}

.botModal .foot button {
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

.botModal .foot button:hover:not(:disabled) {
  filter: brightness(1.25);
}

.botModal .foot button:disabled {
  filter: grayscale(70%);
  cursor: not-allowed;
}
</style>
