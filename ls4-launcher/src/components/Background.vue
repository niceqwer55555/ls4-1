<template>
  <div class="background">
    <!-- SCAFFOLDS -->

    <!-- LOGIN SCAFFOLD -->
    <div
      class="scaffold login"
      v-if="['CHAMPSELECT', 'LOGIN'].includes(backgroundState)"
    ></div>

    <!-- LOGGEDIN - NOT CHAMPSELECT SCAFFOLD -->
    <div class="scaffold" v-else>
      <!-- VERSION -->
      <div class="version">{{ version }} {{ update }}</div>

      <!-- LOGO -->
      <div class="providerLogo" @click="goToRoute('/LoggedIn/home')">
        <video src="static/anims/general/l4s_logo.webm" autoplay loop></video>
      </div>

      <!-- PREVIEW -->
      <div class="alertBlock">
        <div class="alerts" :class="{ submenu: $route.name != 'Home' }">
          <div
            class="alert"
            @click="cycleNext"
            v-for="(alert, index) in alerts"
            :key="index"
            v-show="lookAtAlert == index"
            :class="{
              info: alert.alertType == 'INFO',
              warning: alert.alertType == 'WARNING',
              error: alert.alertType == 'ERROR'
            }"
          >
            <div class="title">{{ alert.title }} - {{ alert.startTime }}</div>
            <div class="text">
              {{ alert.content }}
            </div>
          </div>
        </div>
      </div>

      <!-- PLAYBTN -->
      <button
        v-if="!$store.state.lobby.owner"
        @click="goToRoute('/LoggedIn/play')"
        :disabled="$router.currentRoute.path.includes('/LoggedIn/play')"
        class="playBtn"
      >
        {{ t("BG_PLAY_BTN") }}
      </button>

      <!-- LOBBY_QUEUE_INFO -->
      <div class="queueInfo" v-if="$store.state.lobby.owner">
        <p class="info" v-if="!inQueue && !$store.state.lobby.isCustom">
          {{ t("BG_QUEUE_INFO_INLOBBY") }}
        </p>
        <p class="info" v-if="!inQueue && $store.state.lobby.isCustom">
          {{ t("BG_QUEUE_INFO_INCTLOBBY") }}
        </p>
        <p class="info" v-if="inQueue">
          {{ t("BG_QUEUE_INFO_INQUEUE") }}
        </p>
        <p class="info subinfo" v-if="inQueue">{{ getQueueTimer() }}</p>
        <button
          class="actionBtn abortBtn"
          @click="inQueue ? stopQueue() : leaveLobby()"
        ></button>
        <button
          class="actionBtn lobbyBtn"
          v-if="!['Lobby', 'LobbyCustom'].includes($route.name)"
          @click="backToLobby"
        >
          <i class="fas fa-home"></i>
        </button>
      </div>

      <!-- PLAYER INFO -->
      <div class="playerBlock">
        <div
          class="summonerIcon"
          @click="$modal.summonericon.show()"
          v-bind:data-title="user.summonerLevel"
        >
          <img :src="summonerIconUrl" alt="summonerIcon" />
        </div>
        <div class="playerData">
          <div
            class="playerName"
            :class="{ shop_colored_name: user.nameColourUnlocked }"
          >
            {{ user.summonerName }}
          </div>
          <div class="buttons">
            <button
              @click="goToRoute('/LoggedIn/store')"
              class="shopBtn"
            ></button>
            <button
              @click="goToRoute('/LoggedIn/profile')"
              class="profileBtn"
            ></button>
          </div>
        </div>
        <div class="currencyData">
          <div class="currency">
            <img src="@/assets/images/ip_logo.png" alt="Currency Logo" />
            {{ user.s4Coins }}
          </div>
        </div>
      </div>
    </div>
    <!-- LOGIN BACKPLANE -->
    <div class="backplane login" v-if="backgroundState == 'LOGIN'">
      <div class="toggleCheck">
        <label class="league-check">
          {{ t("BG_BACKPLANE_SHOWANIM") }}
          <input type="checkbox" name="remember" checked @change="togglePlay" />
          <span class="checkmark"></span>
        </label>
      </div>
      <div class="toggleMusic toggleCheck">
        <label class="league-check">
          {{ t("BG_BACKPLANE_TOGGLEMUSIC") }}
          <input
            type="checkbox"
            name="remember"
            :checked="isChecked"
            @change="toggleMusic"
          />
          <span class="checkmark"></span>
        </label>
      </div>
      <div
        class="image"
        v-bind:style="getBackgroundImageUrl(backgroundState)"
        ref="backplaneImage"
      ></div>
      <div class="animation">
        <video
          autoplay
          loop
          width="100%"
          height="100%"
          ref="backplaneVideo"
          :src="getBackgroundVideoUrl(backgroundState)"
        >
          <source
            v-bind:src="getBackgroundVideoUrl(backgroundState)"
            type="video/webm"
          />
        </video>
      </div>
    </div>
    <div
      class="backplane home"
      v-if="
        [
          'HOME',
          'MODESELECT',
          'LOBBY',
          'CUSTOM_LOBBY',
          'ERROR',
          'CHAMPSELECT'
        ].includes(backgroundState)
      "
    >
      <div
        class="image visible"
        v-bind:style="getBackgroundImageUrl(backgroundState)"
        ref="backplaneImage"
      ></div>
    </div>
  </div>
</template>

<script>
import moment from "moment";
import { mapState } from "vuex";
import { ipcRenderer } from "electron";
// import Background from "../plugins/background";
export default {
  data() {
    return {
      isChecked: true,
      lookAtAlert: 0,
      alertLoop: null,
      update: ""
    };
  },
  computed: mapState({
    inQueue: state => state.lobby.inQueue,
    user: state => state.user,
    alerts: state => {
      return state.alertList.filter(alert => {
        if (alert.endTime && new Date() > Date.parse(alert.endTime)) {
          // Check if ended
          return false;
        } else {
          return true;
        }
      });
    },
    summonerIconUrl: state => {
      const { host, port } = state.config.download;
      return `${host}:${port}/summoner_icons/${state.user.summonerIconId}.png`;
    },
    backgroundState: state => state.backgroundState,
    version: state => state.version
  }),
  updated() {
    console.log(
      "The current background state is set to: " + this.backgroundState
    );

    this.isChecked = this.$sound.musicEnabled;
  },
  mounted() {
    this.alertLoop = setInterval(() => {
      if (this.alerts.length == 0) {
        clearInterval(this.alertLoop);
      } else {
        this.cycleNext();
      }
    }, 5000);
  },
  created() {
    ipcRenderer.on("update", (event, info) => {
      this.update = "- " + info;
    });
  },
  methods: {
    cycleNext() {
      if (this.lookAtAlert < this.alerts.length - 1) {
        this.lookAtAlert++;
      } else {
        this.lookAtAlert = 0;
      }

      if (this.alertLoop) {
        clearInterval(this.alertLoop);
        this.alertLoop = null;
        this.alertLoop = setInterval(() => {
          if (this.alerts.length == 0) {
            clearInterval(this.alertLoop);
            this.alertLoop = null;
          } else {
            this.cycleNext();
          }
        }, 5000);
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
    stopQueue() {
      this.$sound.template("OVERVIEW_CLICK");
      this.$store.dispatch("clearLobbyTimers");
      this.$sound.stopAllLoops("CORE_MUSIC");

      this.$socket.sendLobbyMessage(
        "LOBBY_MATCHMAKING_STOP",
        {},
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          this.$store.dispatch("getQueueCount");
          this.$store.dispatch("getServerCount");
          this.$store.dispatch("setLobbyQueueState", false);
        }
      );
    },
    leaveLobby() {
      this.$sound.template("OVERVIEW_CLICK");
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
    backToLobby() {
      this.$sound.template("OVERVIEW_CLICK");

      if (this.$store.state.lobby.isCustom) {
        this.$router.push("/LobbyCustom");
      } else {
        this.$router.push("/lobby");
      }
    },
    goToRoute(value) {
      if (!this.$router.currentRoute.path.includes(value)) {
        if (value === "/LoggedIn/play") {
          this.$sound.template("OVERVIEW_PLAYBUTTON");
        } else if (value === "/LoggedIn/store") {
          this.$sound.template("OVERVIEW_OPENSTORE");
        } else {
          this.$sound.template("OVERVIEW_CLICK");
        }
      }

      if (!this.$router.currentRoute.path.includes(value)) {
        this.$router.push(value);
      }
    },
    getBackgroundVideoUrl(state) {
      // const { host, port } = this.$store.state.config.download;
      console.log("Retrieving bg-video: " + state);
      let test = (() => {
        switch (state) {
          case "LOGIN":
            return `static/anims/background/login.webm`;

          // case "ETC":
          //   return "static/anims/background/home.webm"; //`${host}:${port}/app/splash.webm`;

          // case "LOBBY":
          //   return "static/anims/background/lobby.webm"; //`${host}:${port}/app/lobby.webm`;

          // case "CHAMPSELECT":
          //   return "static/anims/background/champselect.webm";

          default:
            return null; //`${host}:${port}/app/splash.webm`;
        }
      })();

      return test;
    },
    getBackgroundImageUrl(state) {
      switch (state) {
        case "LOGIN":
          return `background-image: url('static/images/background/login-bg.png')`;

        case "CUSTOM_LOBBY":
        case "HOME":
          return `background-image: url('static/images/background/home-bg.png')`;

        case "ETC":
          return `background-image: url('static/images/background/splash.jpg')`;

        case "MODESELECT":
          return `background-image: url('static/images/background/play-bg.png')`;

        case "LOBBY":
          return `background-image: url('static/images/background/lobby-bg.jpg')`;

        case "ERROR":
          return `background-image: url('static/images/background/error-bg.jpg')`;

        case "CHAMPSELECT":
          return `background-image: url('static/images/background/champselect-bg.png')`;

        default:
          return `background-image: url('static/images/background/splash.jpg')`;
      }
    },
    togglePlay() {
      const image = this.$refs.backplaneImage;
      if (image.classList.contains("visible")) {
        image.classList.remove("visible");

        const video = this.$refs.backplaneVideo;
        video.currentTime = 0;
        const isPlaying =
          video.currentTime > 0 &&
          !video.paused &&
          !video.ended &&
          video.readyState > video.HAVE_CURRENT_DATA;

        if (!isPlaying) {
          var playPromise = video.play();
          if (playPromise !== undefined) {
            playPromise.then().catch(function(error) {
              console.error(error);
            });
          } else {
            console.error("Undefined Play Promise");
          }
        }
      } else {
        image.classList.add("visible");
      }
    },
    toggleMusic() {
      let boolNow = this.$sound.toggleMusic();

      if (this.$route.name === "Login" && boolNow) {
        this.$sound.template("LOGIN_MUSIC");
      }
    }
  }
};
</script>

<style lang="scss" scoped>
.background {
  width: 100%;
  height: 100%;
  position: absolute;
  left: 0;
  top: 0;
}
.background .scaffold.login {
  background-image: url("../assets/images/bg-scaffold-login.png");
}
.background .scaffold {
  background-image: url("../assets/images/bg-scaffold.png");
  position: absolute;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  background-size: 100% 100%;
  z-index: 1 !important;
}

.background .scaffold .version {
  position: absolute;
  left: 8px;
  top: 2px;
  font-size: 10px;
  color: #888888;
  font-family: LoLFont2;
}
.background .scaffold .providerLogo {
  position: absolute;
  left: 37px;
  top: 15px;
  width: 100px;
  height: 50px;
  text-align: center;
  cursor: pointer;
  transition: filter ease-in-out 200ms;
}

.background .scaffold .providerLogo:hover {
  filter: brightness(1.2);
}

.background .scaffold .providerLogo video {
  height: 100%;
  width: 100%;
}

.background .scaffold .alertBlock {
  position: absolute;
  left: 160px;
  top: 5px;
  width: 300px;
  height: 65px;
  padding: 9px 8px;
  overflow: hidden;
}
.background .scaffold .alertBlock::after {
  content: "";
  position: absolute;
  left: 0;
  top: 0;
  z-index: 5;
  width: 100%;
  height: 100%;
  background: url("../assets/images/alert_bg.png");
  background-size: 100% 100%;
  pointer-events: none;
}

.background .scaffold .alertBlock .alerts {
  overflow: hidden;
  width: 100%;
  height: 100%;
  position: relative;
  left: 2px;
  top: 3px;
  background-image: url("../assets/images/alert_default.png");
  background-size: 100% 100%;
}

.background .scaffold .alertBlock .alerts.submenu {
  background-image: url("../assets/images/alert_none_bg.png");
}

.background .scaffold .alertBlock .alerts .alert {
  width: 100%;
  height: 100%;
  position: relative;
  border-radius: 3px;
  padding: 2px 0 0 47px;
}

.background .scaffold .alertBlock .alerts .alert .title {
  font-size: 12px;
  font-family: LoLFont2;
  color: rgba(245, 245, 245, 0.9);
}

.background .scaffold .alertBlock .alerts .alert .text {
  font-size: 10px;
  font-family: LoLFont2;
  color: rgba(245, 245, 245, 0.9);
}

.background .scaffold .alertBlock .alerts .alert::before {
  position: absolute;
  left: 3px;
  top: 48%;
  transform: translateY(-50%);
  background-size: 100% 100%;
  content: "";
  width: 40px;
  height: 40px;
}

.background .scaffold .alertBlock .alerts .alert.warning,
.background .scaffold .alertBlock .alerts .alert.info {
  background-image: url("../assets/images/alert_normal_bg.png");
}

.background .scaffold .alertBlock .alerts .alert.error {
  background-image: url("../assets/images/alert_error_bg.png");
}

.background .scaffold .alertBlock .alerts .alert.info::before {
  background-image: url("../assets/images/alert_info.png");
}

.background .scaffold .alertBlock .alerts .alert.warning::before {
  background-image: url("../assets/images/alert_warning.png");
}

.background .scaffold .alertBlock .alerts .alert.error::before {
  background-image: url("../assets/images/alert_error.png");
}

.background .scaffold .playBtn::before {
  position: absolute;
  content: "";
  // border: 1px solid red;
  background-image: url("../assets/images/play-btn-bg.png");
  left: -28px;
  background-size: 100% 100%;
  top: -12px;
  width: calc(100% + 61px);
  height: calc(100% + 20px);
  opacity: 0.75;
}
.background .scaffold .playBtn {
  background-image: url("../assets/images/play-btn-default.png");
  position: absolute;
  left: 556px;
  top: 13px;
  width: 142px;
  height: 54px;
  box-shadow: inset 0 0 5px 2px rgba(0, 0, 0, 0.5),
    0 0 5px 3px rgba(0, 0, 0, 0.25);
  background-size: 108% 108%;
  background-position: center;
  border-radius: 3px;

  font-size: 32px;
  line-height: 14px;
  color: rgb(236, 227, 227);
  font-family: LoLFont2;
  text-shadow: 0 0 9px #ed8f39, 0 0 15px #ed8f39;

  outline: none;
  border: none;
  background-color: transparent;
}

.background .scaffold .playBtn:active {
  background-image: url("../assets/images/play-btn-clicked.png");
  text-shadow: 0 0 9px #c06e21, 0 0 15px #c06e21;
  color: rgba(236, 227, 227, 0.75);
}

.background .scaffold .playBtn:disabled {
  filter: grayscale(100%);
}

.background .scaffold .playBtn:not(:disabled):not(:active):hover {
  cursor: pointer;
  background-image: url("../assets/images/play-btn-active.png");
}

.background .scaffold .queueInfo {
  background-image: url("../assets/images/head_mid_empty.png");
  background-size: 100% 100%;
  position: absolute;
  // left: 630px;
  // top: 5px;
  // width: 212px;
  // height: 79px;
  left: 555px;
  top: 5px;
  width: 170px;
  height: 65px;
  box-shadow: inset 0 0 5px 2px rgba(0, 0, 0, 0.5),
    0 0 5px 3px rgba(0, 0, 0, 0.25);

  outline: none;
  background-size: 98% 98%;
  background-repeat: no-repeat;
  background-position: center center;
  border: none;
  background-color: transparent;
}

.background .scaffold .queueInfo .info {
  font-size: 10px;
  width: 100%;
  position: absolute;
  left: 0;
  top: 15px;
  margin: 0;
  height: auto;
  text-shadow: 0 0 7px #4c8fe6, 0 0 15px #002a7d;
  text-align: center;
  color: #c2ecff;
  font-family: LoLFont2;
}

.background .scaffold .queueInfo .info.subinfo {
  top: 35px;
}

.background .scaffold .queueInfo .actionBtn {
  position: absolute;
  width: 25px;
  height: 25px;
  background-size: 100% 100%;
  outline: none;
  border: none;
  background-color: transparent;
  cursor: pointer;
  transition: filter ease-in-out 200ms;
}

.background .scaffold .queueInfo .actionBtn:hover {
  filter: brightness(1.25);
}

.background .scaffold .queueInfo .abortBtn {
  bottom: 5px;
  right: 7px;
  background-image: url("../assets/images/head_mid_abort_btn.png");
}

.background .scaffold .queueInfo .lobbyBtn {
  bottom: 5px;
  left: 7px;
  color: white;
  font-size: 12px;
  background-image: url("../assets/images/lobby_small_btn.png");
}

.background .scaffold .playerBlock {
  display: flex;
  position: absolute;
  justify-content: space-between;
  left: 885px;
  top: 15px;
  // width: 230px;
  width: 310px;
  height: 51px;
}

.background .scaffold .playerBlock .summonerIcon {
  width: 50px;
  height: 50px;
  border-radius: 2px;
  overflow: hidden;
  position: relative;
  cursor: pointer;
  margin-right: 2px;
}

.background .scaffold .playerBlock .summonerIcon::after {
  position: absolute;
  bottom: 0;
  right: 0;
  width: 20px;
  height: 15px;
  border-radius: 3px;
  text-align: center;
  color: black;
  content: attr(data-title);
  font-family: LoLFont2;
  font-size: 12px;
  font-weight: bold;
  background-color: rgba(245, 245, 245, 0.5);
}

.background .scaffold .playerBlock .summonerIcon img {
  height: 100%;
  width: 100%;
}

.background .scaffold .playerBlock .playerData {
  display: flex;
  flex-direction: column;
  width: 180px;
  height: 100%;
}

.background .scaffold .playerBlock .playerData > div {
  height: 50%;
  width: 100%;
}

.background .scaffold .playerBlock .playerData .playerName {
  text-align: center;
  font-family: LoLFont2;
  color: whitesmoke;
  background-image: url("../assets/images/player-name-bg.png");
  background-size: 100% 100%;
  background-position: center;
  padding: 3px;
  font-size: 14px;
}

.background .scaffold .playerBlock .playerData .buttons {
  display: flex;
  justify-content: space-between;
  padding-top: 1px;
}

.background .scaffold .playerBlock .playerData .buttons button {
  width: 49.5%;
  height: 100%;
  outline: none;
  padding: 0;
  margin: 0;
  background-color: transparent;
  cursor: pointer;
  transition: filter ease-in-out 200ms;
  background-size: 100% 100%;
  background-position: center;
  border: none;
}

.background .scaffold .playerBlock .playerData .buttons button:hover {
  filter: brightness(1.2);
}

.background .scaffold .playerBlock .playerData .buttons button.shopBtn {
  background-image: url("../assets/images/shop-btn.png");
}

.background .scaffold .playerBlock .playerData .buttons button.profileBtn {
  background-image: url("../assets/images/profile-btn.png");
}

.background .scaffold .playerBlock .currencyData {
  width: 90px;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.background .scaffold .playerBlock .currencyData > div {
  height: 50%;
}

.background .scaffold .playerBlock .currencyData .currency {
  width: 100%;
  border: 1px solid #242b36;
  border-radius: 2px;
  font-size: 12px;
  line-height: 20px;
  display: flex;
  padding: 3px;
  font-family: LoLFont2;
  background: linear-gradient(90deg, #131214 0%, #17191d 100%);
}

.background .scaffold .playerBlock .currencyData .currency img {
  height: 18px;
  width: 18px;
}

.backplane {
  width: 100%;
  height: 100%;
  position: absolute;
  z-index: -1;
  border-radius: 25px;
  overflow: hidden;
  background-color: rgb(0, 0, 0);
}

// LOGIN
.backplane.login .toggleCheck {
  position: absolute;
  bottom: 0px;
  left: 15px;
  z-index: 10 !important;
}
.backplane.login .toggleMusic {
  left: 170px;
}

.backplane.login .toggleCheck .league-check {
  left: unset;
  transform: unset;
  width: 100%;
  opacity: 0.5;
}

.backplane .image.visible {
  display: block;
}
.backplane .image.visible ~ .animation {
  display: none;
}

.backplane .image:not(.visible) {
  display: none;
}
.backplane .image:not(.visible) ~ .animation {
  display: block;
}

.backplane .image {
  width: 100%;
  height: 100%;
  background-size: 100% 100%;
}

.backplane .animation {
  animation: loadin forwards 500ms;
}

@keyframes loadin {
  0% {
    opacity: 0;
  }
  100% {
    opacity: 1;
  }
}

// LOBBY

.backplane.lobby .image {
  background-color: rgb(0, 0, 0);
  width: 100%;
  height: 100%;
}
</style>
