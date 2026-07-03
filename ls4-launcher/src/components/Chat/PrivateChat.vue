<template>
  <div id="privatechat" v-if="this.$store.state.token != ''">
    <div class="chatrow">
      <ChatBlock
        v-for="(recipient, index) in recipients"
        :key="index"
        :recipient="recipient"
      />
    </div>
    <div class="actions">
      <button @click="toggleSidebar('friends')" class="friendsBtn"></button>
      <button
        @click="toggleSidebar('chatrooms')"
        disabled
        class="chatroomsBtn"
      ></button>
      <button
        :class="{ blinking: shouldBlinkNotifications }"
        @click="toggleSidebar('notifications')"
        class="notificationsBtn"
      ></button>
      <button @click="$router.push('/bugreport')" class="bugreportBtn">
        <i class="fas fa-bug"></i>
      </button>
    </div>
  </div>
</template>

<script>
import ChatBlock from "@/components/Chat/ChatBlock.vue";
import Chat from "../../plugins/chat";
// import NotImplementedError from "../../utils/errors/NotImplementedError";
// import router from "@/Router";
import { mapState } from "vuex";

export default {
  components: {
    ChatBlock
  },
  data() {
    return {
      visible: false,
      routeIsNotChampselect: true
    };
  },
  watch: {
    $route: function() {
      if (this.$route.path === "/champselect") {
        this.routeIsNotChampselect = false;
      } else {
        this.routeIsNotChampselect = true;
      }
    }
  },
  computed: {
    ...mapState({
      shouldBlinkNotifications(state) {
        if (state.sidebarComponent != "notifications") {
          if (state.lobbyInvites.length > 0 || state.friendIn.length > 0) {
            return true;
          } else {
            return false;
          }
        } else {
          return false;
        }
      },
      recipients: state => state.privateChat.openChats
      // recipients: (state) => {
      //   const { host, port } = state.config.download;
      //   state.friendList.forEach((recipient) => {
      //     recipient.summonerIconUrl = `${host}:${port}/summoner_icons/${recipient.summonerIconId}.png`;
      //   });
      //   return state.friendList;
      // },
    })
  },
  methods: {
    toggleSidebar(component) {
      this.$sound.template("OVERVIEW_CLICK");
      this.$store.commit("setSidebarComponent", component);
    },
    closeChatWindow(recipient) {
      console.log("Should close for " + recipient.summonerName);
    }
    // reportBug() {
    //   router.push("/bugreport");
    // },
  },
  beforeMount() {
    Chat.EventBus.$on("show", params => {
      // Handled via state, but kept for "reasons"
      console.log(params);
    });
  }
};
</script>

<style lang="css" scoped>
#privatechat {
  position: absolute;
  height: 40px;
  width: 100%;
  right: 0;
  bottom: 0;
  z-index: 5;
  display: flex;
}

.actions {
  display: flex;
  width: 21%;
  height: 100%;
  padding: 5px 0;
  justify-content: space-evenly;
}

.actions button {
  width: 24%;
  height: 100%;
  border-radius: 3px;
  background-size: 100% 100%;
  background-color: transparent;
  outline: none;
  border: none;
  transition: filter ease-in-out 200ms;
}

.actions button.friendsBtn {
  background-image: url("../../assets/images/sidebar-friends.png");
}

.actions button.chatroomsBtn {
  background-image: url("../../assets/images/sidebar-chatrooms.png");
}

.actions button.notificationsBtn {
  background-image: url("../../assets/images/sidebar-notifications.png");
}
.actions button.bugreportBtn {
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
  border-radius: 4px;
  text-align: center;
  border: 1px solid #304b69;
  color: white;
  font-family: LoLFont2;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.actions button.bugreportBtn:hover {
  filter: brightness(1.2);
}

.actions button:not(:disabled) {
  cursor: pointer;
}
.actions button:disabled {
  opacity: 0.5;
}
.actions button:not(:disabled):not(.bugreportBtn):hover {
  filter: brightness(1.5);
}

.chatrow {
  display: flex;
  width: 79%;
  height: 100%;
  padding: 3px 20px;
}

.blinking {
  animation: blinking linear 1s infinite;
}

@keyframes blinking {
  0% {
    filter: brightness(1);
  }
  50% {
    filter: brightness(1.25);
  }
  100% {
    filter: brightness(1);
  }
}
</style>
