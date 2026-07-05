<template>
  <div class="outer">
    <div class="inner">
      <div class="titlebar">
        <div id="title">
          {{ t("FRLIST_TITLE") }} ({{ onlineFriends }}/{{
            friends ? friends.length : 0
          }})
        </div>
      </div>
      <div class="friendlist" v-if="friends && friends.length > 0">
        <Friend
          v-for="(friend, index) in friends"
          :key="index"
          :summoner="friend"
        />
      </div>
      <div v-else class="empty-friendlist">
        <img
          src="@/assets/icons/comment-slash-duotone.svg"
          alt="No friends, and ur pc is shit"
        />
        <p>{{ t("FRLIST_NOFRIENDS") }}</p>
      </div>
    </div>
    <div class="actions">
      <input
        type="text"
        class="actionInput"
        ref="actionInput"
        :placeholder="getPlaceHolderForType()"
        v-show="actionInputState != null"
        @keyup="typeAction"
        v-on:keyup.enter="submitAction"
      />
      <button class="addFriendBtn" @click="toggleAction('adduser')">
        <i class="fas fa-user-plus"></i>
      </button>
      <button class="addFriendBtn" @click="toggleAction('search')">
        <i class="fas fa-search"></i>
      </button>
      <button
        class="addFriendBtn"
        v-if="isAdmin()"
        @click="toggleAction('banuser')"
      >
        <i class="fas fa-user-slash"></i>
      </button>
    </div>
  </div>
</template>

<script>
import Friend from "@/components/SideBar/FriendList/Friend.vue";
import { mapState } from "vuex";
export default {
  data() {
    return {
      actionInputState: null
    };
  },
  components: {
    Friend
  },
  props: {
    lobbyState: Boolean
  },
  mounted() {
    this.$store.dispatch("searchFilterFriendlist", "");
  },
  methods: {
    typeAction() {
      switch (this.actionInputState) {
        case "search":
          this.$store.dispatch(
            "searchFilterFriendlist",
            this.$refs.actionInput.value
          );
          break;

        default:
          break;
      }
    },
    submitAction() {
      switch (this.actionInputState) {
        case "adduser":
          this.$socket.sendSystemMessage(
            "FRIEND_OUT",
            { data: this.$refs.actionInput.value },
            (response, error) => {
              if (error) {
                console.log("Flyback error:");
                console.log(error);
                this.$refs.actionInput.classList.add("shake");
                setTimeout(() => {
                  if (this.$refs.actionInput) {
                    this.$refs.actionInput.classList.remove("shake");
                  }
                }, 2000);
              } else {
                this.$refs.actionInput.value = null;
                this.actionInputState = null;
              }
            }
          );
          break;

        case "banuser":
          this.$socket.sendSystemMessage(
            "USER_BAN",
            { data: this.$refs.actionInput.value },
            (response, error) => {
              if (error) {
                console.log("Flyback error:");
                console.log(error);
                this.$refs.actionInput.classList.add("shake");
                setTimeout(() => {
                  if (this.$refs.actionInput) {
                    this.$refs.actionInput.classList.remove("shake");
                  }
                }, 2000);
              } else {
                this.$refs.actionInput.value = null;
                this.actionInputState = null;
              }
            }
          );
          break;

        default:
          break;
      }
    },
    getPlaceHolderForType() {
      switch (this.actionInputState) {
        case "adduser":
          return this.$translate.text("FRLIST_ACTION_PLACEHOLDER_ADDUSER");

        case "search":
          return this.$translate.text("FRLIST_ACTION_PLACEHOLDER_SEARCH");

        case "banuser":
          return this.$translate.text("FRLIST_ACTION_PLACEHOLDER_BANUSER");

        default:
          return this.$translate.text("FRLIST_ACTION_PLACEHOLDER_DEFAULT");
      }
    },
    toggleAction(type) {
      if (this.actionInputState == type) {
        this.$refs.actionInput.value = null;
        this.typeAction();
        this.actionInputState = null;
      } else {
        this.actionInputState = type;

        this.$nextTick(() => {
          this.$refs.actionInput.focus();
        });
      }
    },
    isAdmin() {
      return (
        this.$store.state.user.roles &&
        this.$store.state.user.roles.includes("ADMIN")
      );
    }
  },
  created() {},
  computed: mapState({
    friends(state) {
      return state.friendList.sort((a, b) => {
        const statusMap = {
          ONLINE: 5,
          IN_LOBBY: 4,
          IN_CHAMPSELECT: 3,
          IN_GAME: 2,
          AWAY: 1,
          OFFLINE: 0
        };

        const a_statusScore = statusMap[a.summonerStatus];
        const b_statusScore = statusMap[b.summonerStatus];

        if (a_statusScore > b_statusScore) {
          return -1;
        } else if (a_statusScore < b_statusScore) {
          return 1;
        } else {
          return 0;
        }
      });
    },
    onlineFriends: state => {
      let onlineCount = 0;
      state.friendList.forEach(friend => {
        const { host, port } = state.config.download;
        friend.summonerIconUrl = `${host}:${port}/summoner_icons/${friend.summonerIconId}.png`;
        if (friend.summonerStatus !== "OFFLINE") {
          onlineCount++;
        }
      });
      return onlineCount;
    }
  })
};
</script>

<style lang="scss" scoped>
.outer {
  width: 100%;
  height: 100%;
}

.outer .actions {
  position: relative;
  width: 100%;
  height: 30px;
  display: flex;
  justify-content: center;
}

.outer .actions .actionInput {
  position: absolute;
  top: -30px;
  height: 30px;
  width: 100%;
  outline: none;
  border: none;
  border-radius: 3px;
  left: 0;
  background-color: rgba(255, 255, 255, 0.75);
}

.outer .actions .actionInput.shake {
  border: 1px solid rgba(255, 0, 0, 0.75);
  animation: shake 100ms linear;
  animation-iteration-count: 3;
}

@keyframes shake {
  0% {
    left: -5px;
  }
  100% {
    right: -5px;
  }
}

.outer .actions .addFriendBtn {
  width: 30px;
  height: 100%;
  color: white;
  background-color: transparent;
  background-size: 100% 100%;
  border-radius: 3px;
  outline: none;
  border: none;
  cursor: pointer;
}

.outer .actions .addFriendBtn i {
  font-size: 12px;
  transition: font-size ease-in-out 200ms;
}

.outer .actions .addFriendBtn:hover i {
  font-size: 14px;
}

.outer .inner {
  border-radius: 3px;
  background-image: linear-gradient(180deg, #07111d 0%, #051121 100%);
  border: 1px solid #0c212c;
  height: calc(100% - 30px);
}

.titlebar {
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
.titlebar #title {
  color: whitesmoke;
  font-family: LoLFont2;
  font-size: 14px;
  letter-spacing: 0.6px;
  text-transform: uppercase;
}
.empty-friendlist {
  width: 100%;
  height: 345px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  text-align: center;
}

.empty-friendlist img {
  width: 33%;
  height: 33%;
  object-fit: contain;
  left: 50%;
  position: relative;
  transform: translateX(-50%);
  opacity: 0.5;
}
.empty-friendlist p {
  font-family: LoLFont4;
  text-transform: uppercase;
  color: rgba(245, 245, 245, 0.25);
}
.friendlist {
  display: flex;
  flex-direction: column;
  height: calc(100% - 40px);
  padding: 0 5px;
  overflow: auto;
  transition: height ease-in-out 150ms;
}
.friendlist::-webkit-scrollbar {
  width: 7px;
}
.friendlist::-webkit-scrollbar-thumb {
  background-size: 100% 100%;
  background-color: transparent;
  background-image: url("../../assets/images/scroll-thumb.png");
}
.friendlist::-webkit-scrollbar-button {
  display: none;
}
.friendlist::-webkit-scrollbar-track {
  display: none;
}
.friendlist::-webkit-scrollbar-track-piece {
  display: none;
}
</style>
