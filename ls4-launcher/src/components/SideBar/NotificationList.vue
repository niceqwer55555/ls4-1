<template>
  <div class="inner">
    <div class="block" v-if="lobbyInvites.length > 0">
      <div class="titlebar">
        <div id="title">
          {{ t("NTFLIST_LOBBY_INVS_TITLE") }} ({{ lobbyInvites.length }})
        </div>
      </div>
      <div class="list">
        <div
          class="request"
          v-for="(invite, index) in lobbyInvites"
          :key="index"
        >
          <span>
            {{ invite.inviter.summonerName }}
          </span>
          <button class="acceptButton actionbtn" @click="acceptInvite(invite)">
            <i class="fas fa-check"></i>
          </button>
          <button class="denyButton actionbtn" @click="denyInvite(invite)">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>
    </div>
    <div class="block" v-if="friendRequests.length > 0">
      <div class="titlebar">
        <div id="title">
          {{ t("NTFLIST_FR_REQS_TITLE") }} ({{ friendRequests.length }})
        </div>
      </div>
      <div class="list">
        <div
          class="request"
          v-for="(request, index) in friendRequests"
          :key="index"
        >
          <span>
            {{ request.summonerName }}
          </span>
          <button
            class="acceptButton actionbtn"
            @click="acceptRequest(request)"
          >
            <i class="fas fa-check"></i>
          </button>
          <button class="denyButton actionbtn" @click="denyRequest(request)">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>
    </div>
    <div class="block" v-if="sentRequests.length > 0">
      <div class="titlebar">
        <div id="title">
          {{ t("NTFLIST_SENT_REQS_TITLE") }} ({{ sentRequests.length }})
        </div>
      </div>
      <div class="list">
        <div
          class="request"
          v-for="(request, index) in sentRequests"
          :key="index"
        >
          <span>
            {{ request.summonerName }}
          </span>
          <button class="denyButton actionbtn" @click="removeRequest(request)">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>
    </div>
    <div
      class="block"
      :class="{ empty: friendRequests.length + lobbyInvites.length == 0 }"
      v-if="friendRequests.length + lobbyInvites.length == 0"
    >
      <div class="no-notifications">
        {{ t("NTFLIST_NONTFS") }}
      </div>
    </div>
  </div>
</template>

<script>
import { mapState } from "vuex";

export default {
  data() {
    return {};
  },
  methods: {
    acceptInvite(invite) {
      const router = this.$router;
      this.$socket.sendSystemMessage(
        "LOBBY_ACCEPT",
        { data: invite.lobbyUuid },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }

          if (response.isCustom) {
            router.push("/lobbyCustom");
          } else {
            router.push("/lobby");
          }
        }
      );
    },
    denyInvite(invite) {
      this.$socket.sendSystemMessage(
        "LOBBY_DENY",
        { data: invite.lobbyUuid },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    },
    acceptRequest(request) {
      this.$socket.sendSystemMessage(
        "FRIEND_IN_ACCEPT",
        { data: request },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    },
    denyRequest(request) {
      this.$socket.sendSystemMessage(
        "FRIEND_IN_DENY",
        { data: request },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    },
    removeRequest(request) {
      this.$socket.sendSystemMessage(
        "FRIEND_REMOVE",
        { data: request },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    }
  },
  computed: mapState({
    friendRequests: state => state.friendIn,
    sentRequests: state => state.friendOut,
    lobbyInvites: state => state.lobbyInvites
  })
};
</script>

<style lang="scss" scoped>
.inner {
  border-radius: 3px;
  background-image: linear-gradient(180deg, #07111d 0%, #051121 100%);
  border: 1px solid #0c212c;
  height: 100%;
  overflow: auto;
}

.inner .block {
  width: 100%;
  height: auto;
  margin-bottom: 10px;
  position: relative;
}

.inner .block.empty {
  height: 95%;
}

.inner .block .no-notifications {
  font-size: 14px;
  position: absolute;
  top: 50%;
  width: 100%;
  left: 50%;
  transform: translate(-50%, -50%);
  text-align: center;
  font-style: italic;
  opacity: 0.75;
  font-family: LoLFont2;
}

.inner .block .titlebar {
  display: flex;
  justify-content: space-between;
  margin-bottom: 10px;
  // box-shadow: inset 0px 0px 5px 2px rgba(0, 0, 0, 0.75),
  //   0px 0px 5px 2px rgba(255, 255, 255, 0.12);
  // background-color: #223b5a;
  background: linear-gradient(
      90deg,
      rgba(0, 0, 0, 0.75) 0%,
      transparent 10%,
      transparent 90%,
      rgba(0, 0, 0, 0.75) 100%
    ),
    linear-gradient(
      180deg,
      rgba(255, 255, 255, 0.15) 0%,
      #223b5a 10%,
      #223b5a 45%,
      #0d2846 50%,
      #0d2846 90%,
      rgba(255, 255, 255, 0.15) 100%
    );
  // background-color: rgba(4,9,16, 0.75);
  padding: 5px 10px;
  height: 30px;
}

.inner .block .titlebar #title {
  color: whitesmoke;
  font-family: LoLFont2;
  font-size: 14px;
  letter-spacing: 0.6px;
  text-transform: uppercase;
}

.inner .block .list {
  display: flex;
  flex-direction: column;
  padding: 0 5px;
}

.inner .block .list .request {
  width: 100%;
  height: 30px;
  background-image: linear-gradient(
    180deg,
    rgba(255, 255, 255, 0.15) 0%,
    #223b5a 10%,
    #223b5a 45%,
    #0d2846 50%,
    #0d2846 90%,
    rgba(255, 255, 255, 0.15) 100%
  );
  margin-bottom: 5px;
  border-radius: 3px;
  padding: 5px;
  font-size: 14px;
  font-family: LoLFont2;
  position: relative;
}

.inner .block .list .request .actionbtn {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  border: 1px;
  border-radius: 5px;
  background-size: 100% 100%;
  background-color: transparent;
  background-image: url("../../assets/images/social-button-bg.png");
  height: 20px;
  width: 20px;
  padding: 2px;
  text-align: center;
  opacity: 0.9;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
  outline: none;
}

.inner .block .list .request .actionbtn:hover {
  filter: brightness(1.25);
}

.inner .block .list .request .actionbtn.acceptButton {
  color: green;
  right: 35px;
}

.inner .block .list .request .actionbtn.denyButton {
  color: red;
  right: 10px;
}
</style>
