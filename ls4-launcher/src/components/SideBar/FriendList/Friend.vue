<template>
  <div
    class="friend"
    v-if="summoner.shouldShow"
    v-bind:class="summoner.summonerStatus.toLowerCase()"
    @contextmenu="showContextMenu($event)"
    @click="openChatWindow()"
  >
    <div class="icon">
      <img v-bind:src="summoner.summonerIconUrl" alt="summonerIcon" />
    </div>
    <div class="details">
      <div class="blob"></div>
      <div class="name">{{ summoner.summonerName }}</div>
      <div class="motto">
        {{ formatStatus(summoner.summonerStatus, summoner.summonerMotto) }}
      </div>
    </div>
    <div class="unread-messages" v-if="unreadChatMessages > 0">
      {{ unreadChatMessages }}
    </div>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  props: {
    summoner: Object
  },
  methods: {
    openChatWindow() {
      this.$store
        .dispatch("setPrivateChatOpen", this.summoner)
        .then(didOpen => {
          this.$sound.template("OVERVIEW_CLICK");
          if (didOpen) {
            this.$socket.sendSystemMessage(
              "MESSAGE_PRIVATE_GET",
              { data: this.summoner },
              (response, error) => {
                if (error) {
                  console.log("Flyback error:");
                  console.log(error);
                } else {
                  this.$store.dispatch(
                    "setPrivateChatReadMessages",
                    this.summoner
                  );
                }
              }
            );
          }
        });
    },
    showContextMenu(event) {
      const positioning = `top: ${event.clientY}px;left: ${event.clientX -
        150}px`;
      const params = {
        friend: this.summoner,
        positioning: positioning
      };
      this.$modal.friendcontextmenu.show(params);
    },
    formatStatus(status, motto) {
      switch (status) {
        case "ONLINE":
          return motto ? motto : this.$translate.text(`FRO_STATUS_${status}`);

        case "AWAY":
          return motto ? motto : this.$translate.text(`FRO_STATUS_${status}`);

        default:
          return this.$translate.text(`FRO_STATUS_${status}`);
      }
    }
  },
  computed: mapState({
    unreadChatMessages(state) {
      if (state.privateChat.chatMessages[this.summoner.summonerName]) {
        let unread = 0;
        state.privateChat.chatMessages[this.summoner.summonerName].forEach(
          message => {
            if (message.read === false) {
              unread++;
            }
          }
        );
        return unread;
      } else {
        return 0;
      }
    }
  })
};
</script>

<style lang="scss" scoped>
.friend::after {
  content: "";
  position: absolute;
  opacity: 0;
  left: -10px;
  top: 0;
  width: 100%;
  height: 100%;
  background-repeat: no-repeat;
  background-size: 100% auto;
  background-position: left;
  background-image: url("../../../assets/images/friendglow.png");
  z-index: 5;
  transition: opacity ease-in-out 200ms;
}

.friend {
  display: flex;
  height: 50px;
  position: relative;
  padding: 5px;
  box-sizing: border-box;
  margin-bottom: 5px;
  background-image: linear-gradient(
    180deg,
    #181f27 0%,
    #121318 15%,
    #020609 50%,
    #030a14 85%,
    #181f27 100%
  );
  cursor: pointer;
  transition: filter ease-in-out 200ms;
}
.friend:hover {
  filter: brightness(1.25);
}

.friend:hover::after {
  opacity: 0.7;
}

.friend.offline {
  opacity: 0.5;
}

.friend .icon {
  height: 100%;
  margin-right: 5px;
  box-sizing: border-box;
  // background-color: #0d2846;
  position: relative;
}
.friend .icon img {
  width: 40px;
  height: 40px;
  // box-shadow: 0 0 3px 1px rgba(0, 0, 0, 0.75);
  object-fit: contain;
  object-position: center;
  // border-radius: 50%;
}

.friend .details {
  display: flex;
  position: relative;
  width: calc(100% - 45px);
  flex-direction: column;
  justify-content: space-between;
}

.friend .details .blob {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  background-image: url("../../../assets/images/status-blob-offline.png");
  background-size: 100% 100%;
  box-shadow: 1px 1px 3px 2px rgba(0, 0, 0, 0.25);
  position: absolute;
  right: 3px;
  bottom: 3px;
}

.friend.online .details .blob {
  background-image: url("../../../assets/images/status-blob-online.png");
}

.friend.away .details .blob {
  background-image: url("../../../assets/images/status-blob-away.png");
}

.friend.in_champ_select .details .blob,
.friend.in_lobby .details .blob {
  background-image: url("../../../assets/images/status-blob-busy.png");
}

.friend .details .name {
  color: rgba(245, 245, 245, 1);
  font-family: LoLFont2;
  font-size: 14px;
}

.friend .details .motto {
  color: rgba(245, 245, 245, 0.25);
  font-family: LoLFont2;
  font-size: 12px;
}

.friend.online .details .motto {
  color: rgb(78, 160, 86);
  text-shadow: 0 1px 3px rgba(78, 160, 86, 0.75);
}

.friend.away .details .motto {
  color: rgb(119, 29, 38);
  text-shadow: 0 1px 3px rgba(119, 29, 38, 0.75);
}

.friend.in_lobby .details .motto,
.friend.in_champ_select .details .motto {
  color: rgb(208, 170, 27);
  text-shadow: 0 1px 3px rgba(208, 170, 27, 0.75);
}

.friend .unread-messages {
  position: absolute;
  right: 5px;
  top: 5px;
  border-radius: 7px;
  width: 20px;
  height: 15px;
  box-sizing: border-box;
  padding: 0px;
  font-size: 12px;
  text-align: center;
  color: black;
  font-family: LoLFont4;
  background-color: rgba(218, 165, 32, 0.75);
  animation: blink infinite 1s;
}

@keyframes blink {
  0% {
    filter: brightness(0.7);
  }
  50% {
    filter: brightness(1.2);
  }
  100% {
    filter: brightness(0.7);
  }
}

// #text {
//   margin-top: 5px;
// }
// #motto {
//   font-size: 0.8rem;
//   color: #6b6b6b;
// }
// #status {
//   font-size: 0.5rem;
//   color: green;
// }
</style>
