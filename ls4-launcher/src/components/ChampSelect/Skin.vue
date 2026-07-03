<template>
  <div class="skin" :class="{ selected: isSelected }">
    <img
      v-if="skin.owned"
      :src="getSkinUrl(skin.championId, skin.pictureId)"
      @mouseover="$sound.template('CHAMPION_GRID_HOVER')"
    />
    <img
      v-else
      class="disabled"
      :src="getSkinUrl(skin.championId, skin.pictureId)"
    />
    <div class="banner" v-if="isSelected">
      <span class="name">{{
        skin.name == "default" ? "Classic" : skin.name
      }}</span>
      <span class="info">{{ t("CS_CUR_SELECTED_SKIN") }}</span>
    </div>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  props: {
    skin: Object
  },
  methods: {
    getSkinUrl(championId, index) {
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/skins/${championId}/loading/${championId}_${index}.jpg`;
    }
  },
  computed: mapState({
    isSelected(state) {
      return (
        (state.csCurrentPlayer.selectedSkin &&
          state.csCurrentPlayer.selectedSkin.name === this.skin.name) ||
        (!state.csCurrentPlayer.selectedSkin && this.skin.name === "default")
      );
    }
  })
};
</script>

<style lang="css" scoped>
.skin {
  position: relative;
}
.skin img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  object-position: center;
}

.skin.selected img {
  border: 2px solid #b46f38;
}

.skin:not(.selected) img {
  border: 2px solid rgba(103, 105, 104, 0.75);
}
.skin .banner {
  position: absolute;
  left: 0;
  bottom: 0;
  height: 25%;
  width: 100%;
  background-color: rgba(0, 0, 0, 0.6);
}

.skin .banner .info {
  bottom: 15px;
  position: absolute;
  width: 100%;
  text-align: center;
  left: 0;
  font-size: 13px;
  font-family: LoLFont1;
  color: #974b10;
  font-weight: bold;
}

.skin .banner .name {
  top: 10px;
  text-transform: capitalize;
  position: absolute;
  width: 100%;
  text-align: center;
  left: 0;
  font-size: 13px;
  font-family: LoLFont1;
  color: white;
}
</style>
