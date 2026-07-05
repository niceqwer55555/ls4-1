<template>
  <div class="sidebar-holder" v-show="component != 'hidden'">
    <div class="accents">
      <button class="listBtn">
        <img src="@/assets/icons/bars-social.svg" alt="sidebarList" />
      </button>
      <button class="minimize" @click="component == 'hidden'">
        <img src="@/assets/icons/minimize_social.svg" alt="sidebarMinimize" />
      </button>
    </div>
    <div class="head">
      <Profile />
    </div>
    <div class="content" v-if="component == 'friends'">
      <FriendList :lobbyState="lobbyState" />
    </div>
    <div class="content" v-else-if="component == 'chatrooms'">
      <h2>no chat rooms sry</h2>
    </div>
    <div class="content" v-else-if="component == 'notifications'">
      <NotificationList :lobbyState="lobbyState" />
    </div>
  </div>
</template>

<script>
import Profile from "@/components/SideBar/Profile.vue";
import FriendList from "@/components/SideBar/FriendList.vue";
import NotificationList from "@/components/SideBar/NotificationList.vue";
import { mapState } from "vuex";

export default {
  components: {
    Profile,
    FriendList,
    NotificationList
  },
  data() {
    return {
      lobbyState: false
    };
  },
  computed: mapState({
    component: state => {
      return state.sidebarComponent;
    }
  }),
  methods: {
    showFriendRequests() {
      this.$modal.friendrequests.show();
    }
  }
};
</script>

<style lang="scss" scoped>
.sidebar-holder {
  display: flex;
  background-color: #050a10;
  position: relative;
  flex-direction: column;
  // background: linear-gradient(
  //   180deg,
  //   rgba(39, 73, 111, 80%) 0%,
  //   rgba(12, 39, 72, 0.8) 50%,
  //   rgba(39, 73, 111, 80%) 100%
  // );
  background-image: url("../assets/images/social_scaffold.png");
  background-size: 100% 100%;
  width: 19.8%;
  height: 99.5%;
  border-radius: 5px;
  z-index: 2;
}

.sidebar-holder .accents {
  position: absolute;
  top: 2px;
  right: 0px;
  width: 69px;
  height: 30px;
  justify-content: space-around;
  padding: 0 1px;
  display: flex;
  z-index: 5 !important;
}

.sidebar-holder .accents > button {
  width: 32px;
  outline: none;
  background-size: 100% 100%;
  border: none;
  height: 29px;
  border-radius: 5px;
  background-color: transparent;
  background-image: url("../assets/images/social-button-bg.png");
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.sidebar-holder .accents > button:hover {
  filter: brightness(1.2);
}

.sidebar-holder .head {
  width: 100%;
  height: 90px;
}

.sidebar-holder .content {
  height: calc(100% - 85px);
  padding: 5px;
}

// .sidebar-holder .content .spacer {
//   margin-bottom: 10px;
// }
// .sidebar-holder .content .list-button {
//   display: flex;
//   padding: 10px 0;
//   margin: 0 1rem;
//   justify-content: space-between;
//   cursor: pointer;
// }

// .sidebar-holder .content .list-button .text {
//   font-family: LoLFont4;
//   color: rgba(245, 245, 245, 0.75);
//   transition: color ease-in-out 250ms;
//   font-size: 14px;
// }

// .sidebar-holder .content .list-button .badge {
//   border-radius: 50%;
//   background-color: #c29760;
//   width: 20px;
//   height: 20px;
//   box-sizing: border-box;
//   font-size: 14px;
//   font-family: LoLFont4;
//   color: black;
//   text-align: center;
//   opacity: 0.75;
//   transition: opacity ease-in-out 250ms;
// }

// .sidebar-holder .content .list-button:hover .text {
//   color: rgba(245, 245, 245, 1);
// }

// .sidebar-holder .content .list-button:hover .badge {
//   opacity: 1;
// }

// // .sidebar-holder::after {
// //   content: "";
// //   position: absolute;
// //   left: 0;
// //   top: 0;
// //   width: 100%;
// //   height: 100%;
// // }

// .blinking {
//   animation: blinking ease-in-out infinite 1s;
// }

// @keyframes blinking {
//   0% {
//     filter: brightness(0.7);
//   }
//   50% {
//     filter: brightness(1.2);
//   }
//   100% {
//     filter: brightness(0.7);
//   }
// }
</style>
