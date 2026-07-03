<template>
  <div
    class="champion"
    :class="{
      disabled: champion.unavailable || currentPlayer.lockedIn,
      banned: isBanned
    }"
  >
    <img
      :disabled="champion.unavailable || currentPlayer.lockedIn"
      :src="getChampionImgUrl(champion.id)"
      @click="
        currentPlayer.canBan
          ? selectedBan(champion)
          : selectedChampion(champion)
      "
      @mouseover="$sound.template('CHAMPION_GRID_HOVER')"
    />
    <p class="name">{{ champion.displayName }}</p>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  props: {
    champion: Object
  },
  methods: {
    selectedChampion() {
      this.$socket.sendChampSelectMessage(
        "CHAMPSELECT_SELECT_CHAMPION",
        { data: this.champion.id },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          this.$sound.template("CHAMPION_GRID_CLICK");
          console.log("SELECTED CHAMPION " + this.champion.id);
        }
      );
    },
    selectedBan() {
      this.$socket.sendChampSelectMessage(
        "CHAMPSELECT_BAN_SELECT_CHAMPION",
        { data: this.champion.id },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          this.$sound.template("CHAMPION_GRID_CLICK");
          console.log("SELECTED BAN " + this.champion.id);
        }
      );
    },
    getChampionImgUrl() {
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/champions/${this.champion.id}.jpg`;
    }
  },
  computed: mapState({
    currentPlayer: state => state.csCurrentPlayer,
    globalState: state => state.csGlobalState,
    isBanned(state) {
      if (
        [...state.csAllyBans, ...state.csEnemyBans].filter(ban => {
          return ban.id == this.champion.id;
        }).length > 0
      ) {
        return true;
      } else {
        return false;
      }
    }
  })
};
</script>

<style lang="css" scoped>
.champion {
  width: 60px;
  height: 60px;
  margin-right: 1px;
  margin-bottom: 30px;
  border-radius: 5px;
  box-shadow: inset 0 0 7px 3px rgba(5, 12, 20, 0.55);
  border: 2px solid rgba(95, 95, 95, 1);
  position: relative;
}

.champion.banned::after {
  content: "";
  position: absolute;
  z-index: 1;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  filter: brightness(0.5);
  background-size: 100% 100%;
  background-image: url("../../assets/images/champ-banned.png");
}

.champion.disabled img {
  filter: grayscale(100%);
}
.champion img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  object-position: center;
  transition: filter ease-in-out 200ms;
}

.champion .name {
  margin-top: 3px;
  text-align: center;
  font-size: 10px;
  font-family: LoLFont2;
}

.champion:not(.disabled):hover {
  cursor: pointer;
}

.champion:not(.disabled):hover img {
  filter: brightness(1.25);
}
</style>
