<template>
  <div class="storecontent">
    <div class="itemlist">
      <div v-if="loaded && !items.length" class="noItems">
        {{ t("STORE_NO_ITEMS") }}
      </div>
      <div class="item" v-for="(item, index) in items" :key="index">
        <div class="title">
          {{ item.name ? item.name : `Boost [${item.id}]` }}
        </div>
        <div class="content">
          <div
            class="background"
            style="background-image: url('static/images/general/store_xpboost_icon.png')"
          ></div>
          <div class="foot">
            <span><i class="fas fa-coins"></i> {{ item.price }}</span>
            <button @click="buyItem(item)" :disabled="canBuy(item)">
              {{ t("STORE_UNLOCK_BTN") }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      loaded: false,
      items: []
    };
  },
  methods: {
    getStyleProp(item) {
      console.log(item);
      const { host, port } = this.$store.state.config.download;
      return `background-image: url("${host}:${port}/summoner_icons/1.png")`;
    },
    canBuy(item) {
      if (item.price > this.$store.state.user.s4Coins) {
        return true;
      } else {
        return false;
      }
    },
    buyItem(item) {
      this.$store
        .dispatch("postPurchaseItem", { item, category: "Boosts" })
        .then(response => {
          if (response.status === 200) {
            this.$sound.template("CORE_SHOP_BUY_GENERAL");
            this.$store.dispatch("getStoreItems", "boosts").then(items => {
              this.items = items;
            });
            this.$store.dispatch("deductUserCoins", item.price);
          }
        });
    }
  },
  beforeMount() {
    this.$store.dispatch("getStoreItems", "boosts").then(items => {
      this.items = items;
      this.loaded = true;
    });
  }
};
</script>

<style lang="css" scoped>
@import "../../assets/css/store.css";

.storecontent .itemlist .item .background {
  left: 50%;
  transform: translateX(-50%);
  top: 7%;
  width: 81%;
  height: 65%;
}
</style>
