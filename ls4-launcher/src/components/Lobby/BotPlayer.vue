<template>
  <div class="player bot" :class="{ removable: canRemove }">
    <div class="icon">
      <img :src="getChampionImageUrl()" alt="bot" @error="onIconError" />
      <div class="bot-badge">BOT</div>
    </div>
    <div class="details">
      <div class="name">{{ bot.name }}</div>
      <div class="champion">{{ bot.championDisplayName || bot.championId }}</div>
      <div class="badges">
        <div class="badge difficulty" :class="difficultyClass">
          {{ difficultyLabel }}
        </div>
        <div class="badge btn remove" v-if="canRemove" @click="$emit('remove', bot.botId)">
          <i class="fas fa-times"></i>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    bot: Object,
    canRemove: Boolean
  },
  data() {
    return {
      iconError: false
    };
  },
  computed: {
    difficultyClass() {
      switch (this.bot.difficulty) {
        case "EASY":
          return "easy";
        case "NORMAL":
          return "normal";
        case "HARD":
          return "hard";
        default:
          return "normal";
      }
    },
    difficultyLabel() {
      switch (this.bot.difficulty) {
        case "EASY":
          return this.$translate.text("BOT_DIFFICULTY_EASY");
        case "NORMAL":
          return this.$translate.text("BOT_DIFFICULTY_NORMAL");
        case "HARD":
          return this.$translate.text("BOT_DIFFICULTY_HARD");
        default:
          return this.bot.difficulty;
      }
    }
  },
  methods: {
    getChampionImageUrl() {
      if (this.iconError) {
        return "";
      }
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/champions/${this.bot.championId}.jpg`;
    },
    onIconError() {
      this.iconError = true;
    }
  }
};
</script>

<style lang="scss" scoped>
.player.bot {
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
  position: relative;
}

.player.bot.removable {
  border-color: #5a3a3a;
}

.player.bot .icon {
  margin-right: 10px;
  width: 55px;
  height: 55px;
  position: relative;
}

.player.bot .icon img {
  width: 100%;
  height: 100%;
  border-radius: 4px;
  object-fit: cover;
}

.player.bot .icon .bot-badge {
  position: absolute;
  bottom: 2px;
  right: 2px;
  background: rgba(0, 0, 0, 0.85);
  color: #c8a91a;
  font-size: 9px;
  font-family: LoLFont2;
  padding: 1px 4px;
  border-radius: 2px;
  font-weight: bold;
}

.player.bot .details {
  display: flex;
  height: 100%;
  width: 200px;
  flex-direction: column;
}

.player.bot .details .name {
  height: 35%;
  font-size: 14px;
  font-family: LoLFont2;
  color: #ffffff;
}

.player.bot .details .champion {
  height: 23%;
  font-size: 10px;
  font-family: LoLFont2;
  color: rgba(255, 255, 255, 0.7);
}

.player.bot .details .badges {
  height: 42%;
  padding: 2px 0;
  display: flex;
  align-items: center;
}

.player.bot .details .badges .badge {
  margin-right: 10px;
  height: 100%;
}

.player.bot .details .badges .badge.difficulty {
  font-size: 10px;
  font-family: LoLFont2;
  padding: 2px 6px;
  border-radius: 2px;
  height: auto;
  display: flex;
  align-items: center;
}

.player.bot .details .badges .badge.difficulty.easy {
  background: rgba(80, 160, 80, 0.6);
  color: #c0ffc0;
}

.player.bot .details .badges .badge.difficulty.normal {
  background: rgba(200, 169, 26, 0.6);
  color: #ffe070;
}

.player.bot .details .badges .badge.difficulty.hard {
  background: rgba(200, 60, 60, 0.6);
  color: #ffb0b0;
}

.player.bot .details .badges .badge.btn.remove {
  width: 15px;
  height: 15px;
  position: relative;
  cursor: pointer;
  transition: filter ease-in-out 200ms;
}

.player.bot .details .badges .badge.btn.remove:hover {
  filter: brightness(1.4);
}

.player.bot .details .badges .badge.btn.remove i {
  font-size: 11px;
  color: #ff6060;
}
</style>
