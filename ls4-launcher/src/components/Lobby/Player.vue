<template>
  <div class="player" :class="{ custom: custom }">
    <div class="icon">
      <img :src="getSummonerIconUrl()" alt="summonerIcon" />
    </div>
    <div class="details">
      <div
        class="name"
        :class="{ shop_colored_name: summoner.nameColourUnlocked }"
      >
        {{ summoner.summonerName }}
      </div>
      <div class="level">Level {{ summoner.summonerLevel }}</div>
      <div class="badges">
        <div class="badge" v-if="isCaptain()">
          <img src="@/assets/images/captain_logo.png" alt="captain" />
        </div>
        <div class="badge disabled btn">
          <img
            src="@/assets/images/lobby_small_btn.png"
            alt="lobbyProfileBtn"
          />
          <i class="fas fa-user"></i>
        </div>
        <div class="badge btn" v-if="!isFriend()">
          <img
            src="@/assets/images/lobby_adduser_btn.png"
            alt="addFriendBtn"
            @click="addFriend"
          />
        </div>
        <div class="badge btn" v-if="canKick()">
          <img
            src="@/assets/images/lobby_small_btn.png"
            alt="kickUserBtn"
            @click="kickUser"
          />
          <i class="fas fa-times"></i>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    custom: Boolean,
    summoner: Object
  },
  mounted() {
    console.log(this.summoner);
  },
  methods: {
    getSummonerIconUrl() {
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/summoner_icons/${this.summoner.summonerIconId}.png`;
    },
    isCaptain() {
      return (
        this.summoner.summonerName == this.$store.state.lobby.owner.summonerName
      );
    },
    canKick() {
      return (
        this.$store.state.lobbyOwner &&
        this.summoner.summonerName != this.$store.state.user.summonerName
      );
    },
    isFriend() {
      if (this.summoner.summonerName == this.$store.state.user.summonerName) {
        return true;
      }
      return (
        this.$store.state.friendList.filter(friend => {
          return friend.summonerName == this.summoner.summonerName;
        }).length > 0
      );
    },
    addFriend() {
      this.$socket.sendSystemMessage(
        "FRIEND_OUT",
        { data: this.summoner.summonerName },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    },
    kickUser() {
      this.$socket.sendLobbyMessage(
        "LOBBY_KICK",
        { data: this.summoner.summonerName },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
        }
      );
    }
  }
};
</script>

<style lang="scss" scoped>
.player {
  width: 100%;
  height: 17%;
  background-blend-mode: multiply;
  border: 1.5px solid #474b4c;
  border-radius: 4px;
  background: url("../../assets/images/noise-mask.png"),
    linear-gradient(180deg, #363636 0%, #090b0a 100%);
  display: flex;
  margin-bottom: 5px;
  padding: 4px;
}

.player .icon {
  margin-right: 10px;
  width: 55px;
  height: 55px;
}

.player.custom .icon {
  width: 50px;
  height: 50px;
}

.player .icon img {
  width: 100%;
  height: 100%;
}

.player .details {
  display: flex;
  height: 100%;
  width: 200px;
  flex-direction: column;
}

.player .details .name {
  height: 35%;
  font-size: 14px;
  font-family: LoLFont2;
}

.player .details .level {
  height: 23%;
  font-size: 10px;
  font-family: LoLFont2;
}

.player .details .badges {
  height: 42%;
  padding: 2px 0;
  display: flex;
}

.player .details .badges .badge {
  margin-right: 10px;
  height: 100%;
  width: 15px;
  position: relative;
}

.player .details .badges .badge.btn {
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.player .details .badges .badge.btn:not(.disabled):hover {
  filter: brightness(1.25);
}

.player .details .badges .badge.disabled {
  filter: grayscale(100%);
}

.player .details .badges .badge i {
  font-size: 10px;
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
}

.player .details .badges .badge img {
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  height: 100%;
  width: auto;
  object-fit: contain;
  object-position: center;
}
</style>
