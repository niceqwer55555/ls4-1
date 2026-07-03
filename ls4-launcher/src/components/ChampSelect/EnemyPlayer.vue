<template>
  <div class="player">
    <div class="champion">
      <img
        :src="
          player.selectedChampion
            ? getChampionImgUrl(player.selectedChampion.id)
            : 'static/images/general/not-selected.png'
        "
        :alt="player.selectedChampion ? player.selectedChampion.id : 'NONE'"
      />
      <div class="spells">
        <div class="blankspell"></div>
        <div class="blankspell"></div>
      </div>
    </div>
    <div class="details">
      <div class="name" v-if="champselect.lobbyType != 'CUSTOM'">
        {{ `Summoner ${index + 1}` }}
      </div>
      <div class="name" v-else-if="champselect.lobbyType == 'CUSTOM'">
        {{ `${player.user.summonerName}` }}
      </div>
    </div>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  props: {
    player: Object,
    index: Number
  },
  methods: {
    getPlayerStateAnim() {
      if (this.player.lockedIn) {
        return "static/anims/accent/champselect-player-idle.webm";
      } else if (this.selectedChampion) {
        return "static/anims/accent/champselect-player-active.webm";
      } else {
        return "static/anims/accent/champselect-player-idle.webm";
      }
    },
    getChampionImgUrl(champId) {
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/champions/${champId}.jpg`;
    }
  },
  computed: mapState({
    champselect: state => state.champselect
  })
};
</script>

<style lang="css" scoped>
.player {
  width: 100%;
  height: 19%;
  background-blend-mode: multiply;
  background: linear-gradient(180deg, #363636 0%, #090b0a 100%);
  border: 2px solid #5f5f5f;
  border-radius: 3px;
  display: flex;
  flex-direction: column;
}

.player .champion {
  padding: 5px;
  display: flex;
  width: 50%;
  height: 75%;
  position: relative;
  left: 50%;
  transform: translateX(-50%);
}

.player .champion img {
  width: 50px;
  height: 50px;
  margin-right: 5px;
  border: 1.5px solid #5f5f5f;
  border-radius: 3px;
}

.player .champion .spells {
  display: flex;
  flex-direction: column;
}

.player .champion .spells .blankspell {
  width: 25px;
  height: 25px;
  border: 1px solid #5f5f5f;
  border-radius: 3px;
  background-color: black;
}

.player .details {
  width: 100%;
  height: 25%;
  text-align: center;
  font-family: LoLFont2;
  font-size: 12px;
}
</style>
