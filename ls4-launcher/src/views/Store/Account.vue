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
            style="background-image: url('static/images/general/summoner_name_change.png')"
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
    canBuy(item) {
      if (item.price > this.$store.state.user.s4Coins) {
        return true;
      } else {
        return false;
      }
    },
    buyItem(item) {
      if (item.name == "Summoner Name Change") {
        this.$parent.openSncModal(item);
        return;
      }

      this.$store
        .dispatch("postPurchaseItem", { item, category: "Account" })
        .then(response => {
          if (response.status === 200) {
            this.$sound.template("CORE_SHOP_BUY_GENERAL");
            this.$store.dispatch("getStoreItems", "account").then(items => {
              this.items = items;
            });
            this.$store.dispatch("deductUserCoins", item.price);
          }
        });
    }
  },
  beforeMount() {
    this.$store.dispatch("getStoreItems", "account").then(items => {
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
  top: 11.5%;
  width: 75%;
  height: 61%;
  background-blend-mode: difference;
  background-color: #030b19;
}
</style>
