<template>
  <div
    class="contextmenu"
    v-if="visible"
    v-bind:style="positioning"
    v-bind:class="this.$store.state.lobby.owner ? 'inlobby' : ''"
    tabindex="-1"
    v-on:focusout="hide($event)"
    ref="contextmenu"
  >
    <div class="options">
      <button
        v-if="
          this.$store.state.lobby.owner && friend.summonerStatus != 'OFFLINE'
        "
        @click="inviteFriend"
      >
        <i class="far fa-reply"></i>
        <span class="text">{{ t("FRCM_ACTION_INVITE") }}</span>
      </button>
      <button @click="removeFriend">
        <i class="far fa-user-minus"></i>
        <span class="text">{{ t("FRCM_ACTION_REMOVE") }}</span>
      </button>
      <button @click="blockFriend">
        <i class="far fa-ban"></i>
        <span class="text">{{ t("FRCM_ACTION_BLOCK") }}</span>
      </button>
      <button v-if="isAdmin()" @click="banUser">
        <i class="far fa-user-slash"></i>
        <span class="text">{{ t("FRCM_ACTION_BAN") }}</span>
      </button>
    </div>
  </div>
</template>

<script>
import Modal from "../../plugins/modals";

export default {
  data() {
    return {
      visible: false,
      positioning: "",
      friend: {}
    };
  },
  methods: {
    hide(event = null) {
      if (event) {
        if (event.target.contains(event.relatedTarget)) {
          event.relatedTarget.click();
          this.visible = false;
        }
      }
      this.visible = false;
    },
    show(params) {
      this.visible = true;
      this.friend = params.friend;
      this.positioning = params.positioning;
    },
    inviteFriend() {
      this.$socket.sendLobbyMessage(
        "LOBBY_INVITE",
        { data: this.friend },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    },
    removeFriend() {
      this.$socket.sendSystemMessage(
        "FRIEND_REMOVE",
        { data: this.friend },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    },
    blockFriend() {
      this.$socket.sendSystemMessage(
        "FRIEND_BLOCK",
        { data: this.friend },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    },
    banUser() {
      this.$socket.sendSystemMessage(
        "USER_BAN",
        { data: this.friend.summonerName },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    },
    isAdmin() {
      return this.$store.state.user.roles.includes("ADMIN");
    }
  },
  beforeMount() {
    Modal.EventBus.$on("friend-contextmenu-show", params => {
      this.show(params);
    });
  },
  updated() {
    if (this.$refs.contextmenu) {
      this.$refs.contextmenu.focus();
    }
  }
};
</script>

<style lang="css" scoped>
.contextmenu {
  position: absolute;
  left: 0;
  top: 0;
  width: 155px;
  background-color: rgba(0, 0, 0, 0.75);
  background-image: url("../../assets/images/social-button-bg.png");
  background-size: 100% 100%;
  z-index: 15 !important;
  outline: none;
  border-radius: 7px;
  /* transition: height ease-in-out 150ms; */
  overflow: hidden;
}

/* .contextmenu .title {
  font-size: 14px;
  font-family: LoLFont4;
  color: whitesmoke;
  text-align: center;
  height: 10%;
} */
.contextmenu .options {
  display: flex;
  flex-direction: column;
  height: 100%;
}
.contextmenu .options button {
  width: 100%;
  height: 50%;
  box-sizing: border-box;
  background-color: transparent;
  color: whitesmoke;
  outline: none;
  border: none;
  display: flex;
  justify-content: flex-start;
  font-size: 12px;
  transition: background-color ease-in-out 150ms;
  padding: 5px;
  cursor: pointer;
}
.contextmenu.inlobby .options button {
  height: 33.33%;
}
.contextmenu .options button i {
  width: 30px;
  margin-right: 5px;
}
.contextmenu .options button .text {
  font-family: LoLFont2;
  line-height: 16px;
}
.contextmenu .options button:hover {
  background-color: rgba(245, 245, 245, 0.15);
}
</style>
