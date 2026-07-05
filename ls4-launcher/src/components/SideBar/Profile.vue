<template>
  <div id="profile">
    <div class="upper">
      <a @click="$modal.summonericon.show()">
        <div class="summoner-icon">
          <!--TODO: Make an exp progress bar-->
          <img v-bind:src="summonerIconUrl" alt="summonerIcon" />
        </div>
      </a>
      <div class="summoner-details" v-bind:class="userStatus">
        <div class="inner">
          <div
            class="name"
            :class="{ shop_colored_name: user.nameColourUnlocked }"
          >
            {{ user.summonerName }}
          </div>
          <div class="blob" @click="changeUserStatus"></div>
        </div>
      </div>
    </div>
    <div class="lower">
      <div class="status" v-bind:class="userStatus">
        <input
          type="text"
          placeholder="Motto..."
          ref="profileMottoInput"
          tabindex="-1"
          v-bind:value="user.summonerMotto ? user.summonerMotto : 'Online'"
          v-on:keyup.enter="saveUserMotto"
        />
      </div>
    </div>
  </div>
</template>

<script>
import { mapState } from "vuex";
import NotImplementedError from "../../utils/errors/NotImplementedError";
export default {
  computed: mapState({
    user: state => state.user,
    summonerIconUrl: state => {
      const { host, port } = state.config.download;
      return `${host}:${port}/summoner_icons/${state.user.summonerIconId}.png`;
    },
    userStatus: state => {
      switch (state.user.summonerStatus) {
        case "ONLINE":
          return "online";

        case "AWAY":
          return "away";

        case "OFFLINE":
          return "offline";

        case "IN_LOBBY":
          return "lobby";

        case "IN_CHAMP_SELECT":
          return "champselect";

        default:
          return "offline";
      }
    }
  }),
  methods: {
    saveUserMotto() {
      const motto = this.$refs.profileMottoInput.value;
      if (motto.length > 30) {
        throw new NotImplementedError(
          "Should show error message for user, or play error sound"
        );
      }

      this.$socket.sendSystemMessage(
        "USER_UPDATE_MOTTO",
        { data: motto },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          this.$store.dispatch("saveUserMotto", motto).then(() => {
            this.$refs.profileMottoInput.blur();
          });
        }
      );
    },
    changeUserStatus() {
      let changeto;

      switch (this.$store.state.user.summonerStatus) {
        case "ONLINE":
          changeto = "AWAY";
          break;

        case "AWAY":
          changeto = "ONLINE";
          break;

        default:
      }

      if (changeto) {
        this.$socket.sendSystemMessage(
          "USER_UPDATE_STATUS",
          { data: changeto },
          (response, error) => {
            if (error) {
              console.log("Flyback error:");
              console.log(error);
            }
            this.$store.dispatch("saveUserStatus", changeto);
          }
        );
      }
    }
  }
};
</script>

<style lang="scss" scoped>
#profile {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

#profile .upper {
  display: flex;
  padding: 6px;
}

#profile .upper .summoner-icon {
  width: 50px;
  height: 50px;
  padding: 1.5px;
  border-radius: 3px;
  overflow: none;
  box-sizing: border-box;
  position: relative;
  // box-shadow: inset 0px 0px 3px 0px goldenrod;
}

#profile .upper .summoner-icon img {
  width: 100%;
  height: 100%;
  border-radius: 3px;
  box-sizing: border-box;
  box-shadow: inset 0px 0px 3px 0px black;
}

#profile .upper .summoner-details {
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
  padding-left: 10px;
  width: 100%;
  height: 100%;
}

#profile .upper .summoner-details .inner {
  display: flex;
  justify-content: space-between;
}

#profile .upper .summoner-details .inner .name {
  margin: 0;
  font-family: LoLFont2;
  color: rgba(245, 245, 245, 0.9);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

#profile .lower {
  padding: 0 10px;
}

#profile .lower .status {
  display: flex;
  height: 20px;
}

#profile .lower .status .unfocused {
  font-size: 13px;
  font-family: LolFont4;
  line-height: 22px;
  display: none;
  padding-left: 2px;
}

#profile .lower .status .unfocused.visible {
  display: block;
}

#profile .upper .summoner-details .blob {
  width: 15px;
  height: 15px;
  // border-width: 1px;
  // border-style: solid;
  margin-right: 5px;
  position: relative;
  transform: translateY(10%);
  transition: border-color ease-in-out 150ms;
  background-size: 100% 100%;
  background-image: url("../../assets/images/status-blob-offline.png");
  cursor: pointer;
  box-shadow: 1px 1px 3px 2px rgba(0, 0, 0, 0.25);
  border-radius: 50%;
}

#profile .upper .summoner-details .blob:hover {
  border-color: rgba(245, 245, 245, 0.7);
}

#profile .upper .summoner-details.online .blob {
  background-image: url("../../assets/images/status-blob-online.png");
}

#profile .upper .summoner-details.away .blob {
  background-image: url("../../assets/images/status-blob-away.png");
}

#profile .upper .summoner-details.lobby .blob,
#profile .upper .summoner-details.champselect .blob {
  background-image: url("../../assets/images/status-blob-busy.png");
}

#profile .lower .status input {
  font-size: 13px;
  font-family: LoLFont4;
  outline: none;
  border: none;
  background: linear-gradient(0deg, #27496f 0%, #113158 100%);
  box-shadow: inset 0px 0px 5px 1px rgba(0, 0, 0, 0.75), 0px 0px 5px 0px #464444;
  color: rgba(245, 245, 245, 0.5);
  width: 100%;
  height: 25px;
  padding: 0 10px;
  box-sizing: border-box;
  margin: 0;
  outline-offset: 0;
}

// #profile .lower .status.online input {
//   color: green;
// }

// #profile .lower .status.away input {
//   color: red;
// }

// #profile .lower .status.lobby input,
// #profile .lower .status.champselect input {
//   color: rgb(0, 153, 255);
// }
</style>
