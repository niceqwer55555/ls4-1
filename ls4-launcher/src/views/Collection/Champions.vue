<template>
  <div>
    <div class="end-title">
      Champions
      <span v-if="selectedChampion">- {{ selectedChampion.displayName }}</span>
    </div>
    <div class="end-view">
      <div class="wrapper">
        <div class="listing" ref="championListing">
          <Icon
            v-for="(champion, index) in champions"
            :key="index"
            v-bind:champion="champion"
          />
        </div>
        <div class="information" v-if="champions.length > 0">
          <div class="underlay" v-bind:style="championImgUrl"></div>
          <div class="display-title">
            {{ selectedChampion.displayName }},
            {{ selectedChampion.title }}
          </div>
          <div class="display-spells">
            <div
              class="spell"
              v-for="(spell, index) in selectedChampion.spells"
              :key="index"
            >
              <div class="tooltip">
                {{ spell.displayName }}
              </div>
              <img v-bind:src="getSpellUrl(spell)" alt="spell" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Icon from "@/components/Collection/ChampionIcon.vue";
import { mapState } from "vuex";

export default {
  mounted() {
    if (this.$store.state.collection.champions.length === 0) {
      this.$store.dispatch("getCollectionChampions").then(() => {
        this.$store.dispatch(
          "setSelectedChampion",
          this.$store.state.collection.champions[0]
        );
      });
    } else {
      this.$store.dispatch(
        "setSelectedChampion",
        this.$store.state.collection.champions[0]
      );
    }
  },
  components: {
    Icon
  },
  methods: {
    iconClick(champion) {
      this.$store.dispatch("setSelectedChampion", champion).then();
    },
    getSpellUrl(spell) {
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/spells/${this.selectedChampion.id}/${spell.id}.png`;
    }
  },
  computed: mapState({
    championImgUrl: state => {
      const { host, port } = state.config.download;
      return `background-image: url('${host}:${port}/skins/${state.collection.selectedChampion.id}/splash/${state.collection.selectedChampion.id}_0.jpg')`;
    },
    champions: state => state.collection.champions,
    selectedChampion: state => state.collection.selectedChampion
  })
};
</script>

<style lang="css" scoped>
.end-view .wrapper {
  display: flex;
  width: 100%;
  height: 100%;
  position: relative;
}
.end-view .wrapper .listing {
  width: 50%;
  height: 100%;
  overflow-y: auto;
  overflow-x: hidden;
  display: flex;
  /* flex-direction: column; */
  flex-wrap: wrap;
  padding: 3px 0;
  box-sizing: border-box;
  position: absolute;
  left: 0;
  top: 0;
  background-color: rgba(0, 0, 0, 0.3);
  z-index: 1 !important;
}

.end-view .wrapper .information {
  width: 80%;
  right: 0;
  height: 100%;
  box-sizing: border-box;
  padding: 10px;
  position: absolute;
}

.end-view .wrapper .information .underlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-size: cover;
  mix-blend-mode: exclusion;
  background-position: top;
  z-index: -1 !important;
  /* mask-image: linear-gradient(90deg, transparent 0%, black 33%); */
  mask-image: radial-gradient(
    ellipse 80% 100% at 70% 50%,
    black 50%,
    transparent 80%
  );
}
.end-view .wrapper .information .display-title {
  position: absolute;
  bottom: 10px;
  right: 10px;
  font-size: 24px;
  font-family: LoLFont4;
  font-style: italic;
  color: rgba(245, 245, 245, 1);
  background-image: linear-gradient(
    90deg,
    transparent 0%,
    rgba(0, 0, 0, 0.45) 20%,
    rgba(0, 0, 0, 0.45) 80%,
    transparent 100%
  );
  padding: 5px 20px;
}
.end-view .wrapper .information .display-spells {
  display: flex;
  position: absolute;
  bottom: 60px;
  right: 10px;
  background-image: linear-gradient(
    90deg,
    transparent 0%,
    rgba(0, 0, 0, 0.45) 20%,
    rgba(0, 0, 0, 0.45) 80%,
    transparent 100%
  );
}

.end-view .wrapper .information .display-spells .spell {
  margin-left: 5px;
  box-sizing: border-box;
  padding: 5px;
  position: relative;
  filter: brightness(0.5);
  cursor: pointer;
  transition: filter ease-in-out 150ms;
}

.end-view .wrapper .information .display-spells .spell:hover {
  filter: brightness(1);
}

.end-view .wrapper .information .display-spells .spell img {
  border-radius: 7px;
}

.end-view .wrapper .information .display-spells .spell .tooltip {
  display: none;
  position: absolute;
  top: -30px;
  text-align: center;
  width: 400%;
  border-radius: 7px;
  font-size: 14px;
  box-sizing: border-box;
  padding: 5px;
  background-color: rgba(0, 0, 0, 0.75);
  font-family: LoLFont4;
  color: whitesmoke;
  text-transform: uppercase;
  left: 0;
  transform: translateX(-50%);
}

.end-view .wrapper .information .display-spells .spell:nth-child(1) .tooltip {
  left: 200%;
}

.end-view .wrapper .information .display-spells .spell:nth-child(2) .tooltip {
  left: 100%;
}

.end-view .wrapper .information .display-spells .spell:nth-child(3) .tooltip {
  left: 0;
}

.end-view .wrapper .information .display-spells .spell:nth-child(4) .tooltip {
  left: -100%;
}

.end-view .wrapper .information .display-spells .spell:hover .tooltip {
  display: block;
}
</style>
