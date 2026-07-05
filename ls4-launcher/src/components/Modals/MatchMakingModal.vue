<template>
  <div v-if="visible">
    <div class="backdrop"></div>
    <div class="modal" ref="modal" tabindex="-1">
      <div class="title">
        {{ getMMTitle() }}
      </div>
      <div class="timer">
        <div class="pie spinner"></div>
        <div class="pie filler"></div>
        <div class="mask"></div>
      </div>
      <div class="actions" v-if="canAct">
        <button @click="acceptMatch">
          {{ t("MMMDL_ACCEPT_BTN") }}
        </button>
        <button @click="denyMatch">
          {{ t("MMMDL_DENY_BTN") }}
        </button>
      </div>
      <div class="players" v-if="!canAct">
        <div
          class="player"
          v-for="index in matchFoundState.accepted"
          :key="index"
        >
          <img src="@/assets/images/matchmaking_player_accept.png" />
        </div>
        <div
          class="player"
          v-for="index in matchFoundState.denied"
          :key="index"
        >
          <img src="@/assets/images/matchmaking_player_deny.png" />
        </div>
        <div
          class="player pending"
          v-for="index in matchFoundState.pending"
          :key="index"
        >
          <img src="@/assets/images/matchmaking_player_accept.png" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
// import { mapState } from "vuex";
import Modal from "../../plugins/modals";
import { ipcRenderer } from "electron";
import { mapState } from "vuex";

export default {
  data() {
    return {
      visible: false,
      canAct: true,
      timeout: null
    };
  },
  computed: mapState({
    matchFoundState: state => state.matchFoundState
  }),
  methods: {
    getMMTitle() {
      if (!this.canAct) {
        return this.$translate.text("MMDL_TITLE_WAIT");
      } else {
        return this.$translate.text("MMDL_TITLE_MATCH_FOUND");
      }
    },
    hide() {
      this.visible = false;
      this.$store.dispatch("clearMatchFoundState");
    },
    show() {
      this.visible = true;
    },
    acceptMatch() {
      if (!this.canAct) return;
      this.canAct = false;
      this.$socket.sendLobbyMessage(
        "LOBBY_CHAMPSELECT_ACCEPT",
        {},
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          console.log(response);
        }
      );
    },
    denyMatch() {
      if (!this.canAct) return;
      this.canAct = false;

      Modal.EventBus.$emit("matchmaking-reset-timer");
      this.$socket.sendLobbyMessage(
        "LOBBY_CHAMPSELECT_DENY",
        {},
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          console.log(response);
        }
      );
    }
  },
  beforeMount() {
    Modal.EventBus.$on("matchmaking-show", () => {
      this.show();
      ipcRenderer.send("focusApp");
      this.$sound.template("CORE_MATCH_FOUND");

      this.timeout = setTimeout(() => {
        this.hide();
        this.canAct = true;
      }, 15000);
    });

    Modal.EventBus.$on("matchmaking-hide", () => {
      this.hide();
      this.canAct = true;
    });
  },
  updated() {
    if (this.$refs.modal) {
      this.$refs.modal.focus();
    }
  }
};
</script>

<style lang="scss" scoped>
.backdrop {
  height: 100%;
  width: 100%;
  position: absolute;
  z-index: 9 !important;
  background-color: rgba(0, 0, 0, 0.5);
}

.modal {
  position: absolute;
  left: 50%;
  top: 50%;
  width: 380px;
  height: 180px;
  background-size: 100% 100%;
  background-image: url("../../assets/images/matchmaking_modal_bg.png");
  // box-shadow: 0 0 3px 1px rgba(245, 245, 245, 0.25);
  padding: 20px;
  transform: translate(-50%, -50%);
  z-index: 10 !important;
  outline: none;
}

.modal .title {
  position: absolute;
  top: 25px;
  left: 0;
  text-align: center;
  width: 100%;
  font-size: 18px;
  font-family: LoLFont2;
}

.modal .actions {
  position: absolute;
  bottom: 25px;
  left: 0;
  width: 100%;
  padding: 0 30px;
  display: flex;
  justify-content: space-between;
}

.modal .players {
  position: absolute;
  bottom: 25px;
  left: 0;
  width: 100%;
  padding: 0 30px;
  display: flex;
  justify-content: center;
}

.modal .players .player:not(:last-child) {
  margin-right: 3px;
}

.modal .players .player.pending {
  filter: brightness(0.3);
}

.modal .actions button {
  background: linear-gradient(
    180deg,
    #3c73b4 0%,
    #20477e 45%,
    #1e3e6d 50%,
    #0e284b 100%
  );
  outline: none;
  border: none;
  width: 125px;
  border-radius: 2px;
  margin-right: 10px;
  padding: 5px 0;
  text-align: center;
  border: 1px solid #304b69;
  color: white;
  font-family: LoLFont2;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.modal .actions button:hover {
  filter: brightness(1.25);
}

.modal .timer {
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  background: rgb(255, 255, 255);
  border-radius: 50%;
  overflow: hidden;
}

@mixin timer($item, $duration, $size, $color, $border, $hover: running) {
  #{$item} {
    width: $size;
    height: $size;
  }

  #{$item} .pie {
    width: 50%;
    height: 100%;
    transform-origin: 100% 50%;
    position: absolute;
    background: $color;
    border: #{$border};
  }

  #{$item} .spinner {
    border-radius: 100% 0 0 100% / 50% 0 0 50%;
    z-index: 200;
    border-right: none;
    animation: rota $duration + s linear forwards;
  }

  #{$item}:hover .spinner,
  #{$item}:hover .filler,
  #{$item}:hover .mask {
    animation-play-state: $hover;
  }

  #{$item} .filler {
    border-radius: 0 100% 100% 0 / 0 50% 50% 0;
    left: 50%;
    opacity: 0;
    z-index: 100;
    animation: opa $duration + s steps(1, end) forwards reverse;
    border-left: none;
  }

  #{$item} .mask {
    width: 50%;
    height: 100%;
    position: absolute;
    background: inherit;
    opacity: 1;
    z-index: 300;
    animation: opa $duration + s steps(1, end) forwards;
  }

  @keyframes rota {
    0% {
      transform: rotate(0deg);
    }
    100% {
      transform: rotate(360deg);
    }
  }

  @keyframes opa {
    0% {
      opacity: 1;
    }
    50%,
    100% {
      opacity: 0;
    }
  }
}

@include timer(".modal .timer", 15, 25px, rgb(0, 0, 0), "none");
// from: https://codepen.io/KittyGiraudel/pen/BHEwo
</style>
