<template>
  <div class="chatblock" :class="{ open: isOpen, unread: hasUnread }">
    <div class="head" @click="toggleOpen">
      <button class="closeChatBtn" @click.stop.prevent="closeChat">
        <i class="far fa-times"></i>
      </button>
      {{ recipient.summonerName }}
    </div>
    <div class="content">
      <div class="messages" id="messages">
        <div
          class="message"
          v-for="(message, index) in messages"
          :key="index"
          :class="{ recieved: getOrigin(message) }"
        >
          [{{ formatTime(message) }}] {{ message.from }}:
          {{ message.data }}
        </div>
      </div>
      <div class="input">
        <input
          type="text"
          ref="chatInput"
          v-on:keyup.enter="sendMessage"
          placeholder="Your message..."
        />
      </div>
    </div>
  </div>
</template>

<script>
import moment from "moment";
import { mapState } from "vuex";
export default {
  props: {
    recipient: Object
  },
  methods: {
    getOrigin(message) {
      return message.from == this.recipient.summonerName;
    },
    toggleOpen() {
      if (!this.isOpen) {
        this.$socket.sendSystemMessage(
          "MESSAGE_PRIVATE_GET",
          { data: this.recipient },
          (response, error) => {
            if (error) {
              console.log("Flyback error:");
              console.log(error);
            }
            this.$store.dispatch("setPrivateChatReadMessages", this.recipient);
          }
        );
      }

      this.$store.dispatch("setPrivateChatStatus", {
        summonerName: this.recipient.summonerName,
        change: "toggle"
      });
    },
    closeChat() {
      this.$store.dispatch("setPrivateChatClosed", this.recipient);
    },
    formatTime(message) {
      return `${moment(message.messageTimestamp).format("HH:mm")}`;
    },
    sendMessage() {
      const message = this.$refs.chatInput.value;
      if (message.trim() == "") {
        return;
      }

      this.$refs.chatInput.value = "";
      this.$socket.sendSystemMessage(
        "MESSAGE_PRIVATE",
        {
          data: this.recipient,
          message: message,
          from: this.user.summonerName
        },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    }
  },
  updated() {
    const container = this.$el.querySelector("#messages");
    container.scrollTop = container.scrollHeight;
  },
  mounted() {
    const container = this.$el.querySelector("#messages");
    container.scrollTop = container.scrollHeight;
  },
  computed: mapState({
    user: state => state.user,
    hasUnread(state) {
      const thischat =
        state.privateChat.chatMessages[this.recipient.summonerName];
      let unreadMessages = 0;
      if (thischat) {
        unreadMessages = thischat.filter(message => {
          return !message.read;
        }).length;
      }

      if (!this.isOpen && unreadMessages > 0) {
        return true;
      } else {
        return false;
      }
    },
    isOpen(state) {
      return (
        state.privateChat.openChats.filter(recipient => {
          return (
            recipient.summonerName == this.recipient.summonerName &&
            recipient.shows
          );
        }).length > 0
      );
    },
    messages(state) {
      const msgs = [
        ...state.privateChat.chatMessages[this.recipient.summonerName]
      ];

      if (msgs) {
        msgs.reverse();
      }
      return msgs;
      //   return msgs
      //     ? msgs.sort((a, b) => {
      //         if (new Date(a.messageTimestamp) > new Date(b.messageTimestamp)) {
      //           return 1;
      //         } else if (
      //           new Date(b.messageTimestamp) > new Date(a.messageTimestamp)
      //         ) {
      //           return -1;
      //         } else {
      //           return 0;
      //         }
      //       })
      //     : [];
    }
  })
};
</script>

<style lang="css" scoped>
.chatblock {
  position: relative;
  max-width: 220px;
  height: 100%;
  width: 30%;
  margin-right: 5px;
  filter: brightness(0.75);
}

.chatblock.unread {
  animation: blinking ease-in-out 1s infinite;
}

@keyframes blinking {
  0% {
    filter: brightness(1);
  }
  50% {
    filter: brightness(1.35);
  }
  100% {
    filter: brightness(1);
  }
}

.chatblock.open {
  height: 240px;
  margin-top: -200px;
}

.chatblock .content {
  display: none;
  flex-direction: column;
  height: 200px;
  width: 100%;
  border: 1px solid rgba(100, 166, 255, 0.25);
  /* background: linear-gradient(
    90deg,
    #237df7 0%,
    #1c5fba 40%,
    #0c3374 55%,
    #0a223e 100%
  ); */
  background-color: #0c3374;
  padding: 3px;
}

.chatblock .content .messages {
  display: flex;
  flex-direction: column;
  background-color: #0a223e;
  border-radius: 2px;
  width: 100%;
  height: 80%;
  overflow: auto;
}

.chatblock .content .messages .message {
  font-size: 12px;
  font-family: LoLFont2;
  padding: 5px 2px;
  line-break: anywhere;
  color: whitesmoke;
  border-bottom: 1px solid rgba(218, 165, 32, 0.45);
  user-select: text;
}

.chatblock .content .messages .message:not(.recieved) {
  /* text-align: right; */
  background-color: #0e2e53;
}

.chatblock .content .input {
  height: 20%;
  width: 100%;
}

.chatblock .content .input input {
  width: 100%;
  height: 100%;
  outline: none;
  background-color: #0a223e;
  color: whitesmoke;
  border: 1px solid rgba(100, 166, 255, 0.25);
}

.chatblock.open .content {
  display: flex;
}

.chatblock.open .head {
  border-bottom-left-radius: 0;
  border-bottom-right-radius: 0;
  border-bottom: none;
}

.chatblock .head {
  position: relative;
  background: linear-gradient(
    180deg,
    #237df7 0%,
    #1c5fba 40%,
    #0c3374 55%,
    #0a223e 100%
  );
  border-radius: 3px;
  padding: 5px;
  text-align: left;
  padding-right: 30px;
  height: 100%;
  max-height: 33px;
  width: 100%;
  font-family: LoLFont2;
  font-size: 16px;
  border: 1px solid rgba(100, 166, 255, 0.25);
  box-shadow: 0px 0px 5px 2px rgba(0, 0, 0, 0.75),
    0px 0px 5px 2px rgba(255, 255, 255, 0.12);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.chatblock .head .closeChatBtn {
  position: absolute;
  right: 5px;
  top: 45%;
  transform: translateY(-50%);
  height: 23px;
  width: 23px;
  outline: none;
  border: none;
  filter: brightness(1.25);
  border-radius: 4px;
  box-shadow: 0px 0px 2px 0px rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(100, 166, 255, 0.25);
  background: linear-gradient(
    180deg,
    #237df7 0%,
    #1c5fba 40%,
    #0c3374 55%,
    #0a223e 100%
  );
  padding: 0;
  font-size: 16px;
  color: #cbf6ff;
  transition: box-shadow ease-in-out 200ms;
  cursor: pointer;
}

.chatblock .head .closeChatBtn:hover {
  box-shadow: 0px 0px 4px 2px rgba(255, 255, 255, 0.15);
}
</style>
