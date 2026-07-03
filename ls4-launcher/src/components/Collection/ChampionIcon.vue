<template>
  <div
    class="icon-wrap"
    v-bind:class="selectedChampion.id == champion.id ? 'active' : ''"
  >
    <div class="icon" @click="$parent.iconClick(champion)">
      <img v-bind:src="getChampionImgUrl(champion)" />
    </div>
    <div class="icon-title">{{ champion.displayName }}</div>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  props: {
    champion: Object
  },
  methods: {
    getChampionImgUrl(champion) {
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/champions/${champion.id}.jpg`;
    }
  },
  computed: mapState({
    selectedChampion: state => state.collection.selectedChampion
  })
};
</script>

<style lang="scss" scoped>
.icon-wrap {
  width: 16.66%;
  height: 105px;
  display: flex;
  flex-direction: column;
  margin-bottom: 5px;
  opacity: 0.8;
  transition: opacity ease-in-out 200ms;
  cursor: pointer;
}
.icon-wrap.active {
  opacity: 1;
}
.icon-wrap:hover {
  opacity: 1;
}
.icon-wrap .icon-title {
  text-align: center;
  font-size: 14px;
  font-family: LoLFont4;
  color: rgba(255, 255, 255, 0.75);
  background-image: linear-gradient(
    90deg,
    transparent 0%,
    rgba(0, 0, 0, 0.85) 20%,
    rgba(0, 0, 0, 0.85) 80%,
    transparent 100%
  );
}
.icon-wrap .icon {
  width: 100%;
  height: 80%;
  // mask-image: radial-gradient(
  //   ellipse 35% 50% at 50% 50%,
  //   black 70%,
  //   transparent 85%
  // );
  cursor: pointer;
  transition: filter ease-in-out 200ms;
  padding: 5px;
  box-sizing: border-box;
}
.icon-wrap .icon img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  object-position: center;
  border-radius: 7px;
  box-sizing: border-box;
}

.icon-wrap.active .icon img {
  border: 1px solid rgba(119, 87, 11, 0.55);
}
</style>
