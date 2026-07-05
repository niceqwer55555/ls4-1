<template>
  <div class="storeview">
    <div class="snc-modal" v-if="snc_visible">
      <div class="backdrop"></div>
      <div class="modal-body">
        <button class="closebtn" @click="snc_visible = false">
          <i class="fas fa-times"></i>
        </button>
        <div class="form">
          <div class="info">
            <span class="title">{{ t("SNCMDL_TITLE") }}</span>
            <p class="description" v-html="t('SNCMDL_DESCRIPTION')"></p>
          </div>
          <div class="inner">
            <label for="desiredName">{{ t("SNCMDL_DESIRED_NAME") }}</label>
            <div
              class="form-group mbo"
              id="desiredName"
              :data-validity="snc_response"
              :data-validity-color="snc_res_color"
            >
              <input
                type="text"
                v-model="desiredName"
                v-on:input="snc_available = false"
              />
              <button @click="sncCheck" :disabled="desiredName == ''">
                {{ t("SNCMDL_CHECK_NAME_BTN") }}
              </button>
            </div>
            <label for="verifyName">{{ t("SNCMDL_VERIFY_NAME") }}</label>
            <div class="form-group" id="verifyName">
              <input type="text" v-model="verifyName" />
            </div>
          </div>
        </div>
        <div class="action">
          <div class="buytab">
            <div
              class="title"
              v-html="t('SNCMDL_ACTION_BUYTAB_TITLEPRE')"
            ></div>
            <div class="listings">
              <div class="listing">
                <span>
                  {{ t("SNCMDL_ACTION_BUYTAB_LISTING_CURRENT") }}
                </span>
                <span>
                  {{ $store.state.user.s4Coins }}
                </span>
              </div>
              <div class="listing">
                <span>
                  {{ t("SNCMDL_ACTION_BUYTAB_LISTING_COST") }}
                </span>
                <span>-{{ snc_item.price }}</span>
              </div>
              <div class="listing">
                <span>
                  {{ t("SNCMDL_ACTION_BUYTAB_LISTING_BALANCE") }}
                </span>
                <span>
                  {{ $store.state.user.s4Coins - snc_item.price }}
                </span>
              </div>
              <button @click="buyNameChange" :disabled="!canBuyNameChange">
                <span> {{ t("SNCMDL_ACTION_BUYTAB_UNLOCK_BTN") }}</span>
                <span
                  ><img src="@/assets/images/ip_logo.png" alt="coinicon"
                /></span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="tabs">
      <router-link @click.native="clickSound" to="/LoggedIn/store/skins">{{
        t("STORE_SKINS_TAB")
      }}</router-link>
      <router-link @click.native="clickSound" to="/LoggedIn/store/icons">{{
        t("STORE_ICONS_TAB")
      }}</router-link>
      <router-link @click.native="clickSound" to="/LoggedIn/store/boosts">{{
        t("STORE_BOOSTS_TAB")
      }}</router-link>
      <router-link @click.native="clickSound" to="/LoggedIn/store/launcher">{{
        t("STORE_LAUNCHER_TAB")
      }}</router-link>
      <router-link @click.native="clickSound" to="/LoggedIn/store/account">{{
        t("STORE_ACCOUNT_TAB")
      }}</router-link>
    </div>
    <router-view class="content"></router-view>
  </div>
</template>

<script>
export default {
  data() {
    return {
      snc_visible: false,
      desiredName: "",
      verifyName: "",
      snc_res_color: "",
      snc_response: "",
      snc_available: false,
      snc_item: {
        price: 420
      }
    };
  },
  beforeMount() {
    this.$store.dispatch("changeBackgroundState", "HOME");
  },
  computed: {
    canBuyNameChange() {
      return (
        this.verifyName == this.desiredName &&
        this.desiredName != "" &&
        this.snc_available == true
      );
    }
  },
  methods: {
    openSncModal(item) {
      this.$sound.template("OVERVIEW_CLICK");
      this.snc_item = item;
      this.snc_visible = true;
    },
    buyNameChange() {
      if (this.canBuyNameChange) {
        this.$store
          .dispatch("postPurchaseItem", {
            item: this.snc_item,
            category: "Account",
            desiredName: this.desiredName
          })
          .then(response => {
            if (response.status === 200) {
              this.$sound.template("CORE_SHOP_BUY_GENERAL");
              this.$store.dispatch("getStoreItems", "account").then(items => {
                this.items = items;
              });
              this.$store.dispatch("deductUserCoins", this.snc_item.price);
            }
          });
      }
    },
    sncCheck() {
      if (
        this.desiredName.match(/^[a-zA-Z0-9]*$/) != null &&
        this.desiredName.length <= 12 &&
        this.desiredName.length >= 3
      ) {
        this.$store
          .dispatch("getStoreCheckName", this.desiredName)
          .then(isAvailable => {
            if (!isAvailable) {
              this.snc_response = this.$translate.text(
                "SNCMDL_CHECK_RESPONSE_INUSE"
              );
              this.snc_res_color = "red";
              this.snc_available = false;
            } else {
              this.snc_res_color = "green";
              this.snc_response = this.$translate.text(
                "SNCMDL_CHECK_RESPONSE_AVAILABLE"
              );
              this.snc_available = true;
            }
          });
      } else {
        this.snc_res_color = "red";
        this.snc_response = this.$translate.text(
          "SNCMDL_CHECK_RESPONSE_INVALID"
        );
        this.snc_available = false;
      }
    },
    clickSound() {
      this.$sound.template("OVERVIEW_CLICK");
    }
  }
};
</script>

<style lang="css">
.storeview .snc-modal .modal-body .form .info .description span {
  color: rgb(192, 18, 18);
  font-weight: bold;
}
</style>

<style lang="css" scoped>
.storeview {
  display: flex;
  width: 100%;
  height: 100%;
  padding: 10px;
}

.storeview .snc-modal {
  position: absolute;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  z-index: 9 !important;
}

.storeview .snc-modal .closebtn {
  position: absolute;
  top: 10px;
  right: 10px;
  width: 20px;
  height: 20px;
  outline: none;
  background: linear-gradient(
    180deg,
    #3c73b4 0%,
    #20477e 45%,
    #1e3e6d 50%,
    #0e284b 100%
  );
  border: none;
  border-radius: 2px;
  font-size: 12px;
  padding: 0px 1px;
  z-index: 10 !important;
  text-shadow: 0 0 4px rgba(255, 255, 255, 0.5);
  border: 1px solid #304b69;
  color: white;
  font-family: LoLFont2;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.storeview .snc-modal .closebtn:hover {
  filter: brightness(1.25);
}

.storeview .snc-modal::before {
  width: 100%;
  position: absolute;
  left: 0;
  top: 0;
  height: 100%;
  content: "";
  background-color: rgba($color: #000000, $alpha: 0.65);
}

.storeview .snc-modal .modal-body {
  width: 750px;
  height: 350px;
  display: flex;
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  border-radius: 5px;
  background-size: 100% 100%;
  padding: 20px;
  background-image: url("../../assets/images/snchange_bg.png");
}

.storeview .snc-modal .modal-body .form {
  width: 50%;
  display: flex;
  flex-direction: column;
  height: 90%;
  justify-content: space-between;
}

.storeview .snc-modal .modal-body .form .info .title {
  color: #1a67a7;
  display: block;
  font-family: LoLFont2;
  font-size: 14px;
  text-shadow: 0 0 3px rgba(51, 115, 168, 0.6);
  position: relative;
  width: 100%;
}

.storeview .snc-modal .modal-body .form .info .title::after {
  position: absolute;
  bottom: -2px;
  left: 0;
  content: "";
  height: 2px;
  width: 100%;
  background: linear-gradient(
    90deg,
    rgba(121, 120, 128, 0.5) 0%,
    rgb(121, 120, 128) 50%,
    rgba(121, 120, 128, 0.5) 100%
  );
  -webkit-mask-image: linear-gradient(
    90deg,
    black 80%,
    transparent 90%,
    transparent 100%
  );
}

.storeview .snc-modal .modal-body .form .info .description {
  font-family: LoLFont2;
  font-size: 12px;
}

.storeview .snc-modal .modal-body .form .inner {
  display: flex;
  flex-direction: column;
}

.storeview .snc-modal .modal-body .form .inner label {
  font-family: LoLFont2;
  font-size: 12px;
  margin-bottom: 5px;
}

.storeview .snc-modal .modal-body .form .inner .form-group {
  display: flex;
  justify-content: space-between;
  width: 80%;
  position: relative;
}

.storeview .snc-modal .modal-body .form .inner .form-group.mbo {
  margin-bottom: 20px;
}

[data-validity-color="green"]::after {
  color: green;
}

[data-validity-color="red"]::after {
  color: red;
}

.storeview .snc-modal .modal-body .form .inner .form-group::after {
  content: attr(data-validity);
  position: absolute;
  font-weight: bold;
  bottom: -14px;
  font-family: LoLFont2;
  font-size: 12px;
}

.storeview .snc-modal .modal-body .form .inner .form-group input {
  background-color: black;
  border-radius: 2px;
  outline: white;
  height: 27px;
  font-family: LoLFont2;
  color: white;
  box-shadow: inset 0 0 5px #132b35, 0 0 5px #132b35;
  border: 1px solid rgba(175, 192, 199, 0.75);
}

.storeview .snc-modal .modal-body .form .inner .form-group button {
  background: linear-gradient(
    180deg,
    #3c73b4 0%,
    #20477e 45%,
    #1e3e6d 50%,
    #0e284b 100%
  );
  outline: none;
  border: none;
  border-radius: 2px;
  margin-right: 10px;
  font-size: 12px;
  padding: 5px 15px;
  text-shadow: 0 0 4px rgba(255, 255, 255, 0.5);
  border: 1px solid #304b69;
  color: white;
  font-family: LoLFont2;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.storeview .snc-modal .modal-body .form .inner .form-group button:disabled {
  filter: grayscale(100%);
}

.storeview
  .snc-modal
  .modal-body
  .form
  .inner
  .form-group
  button:not(:disabled):hover {
  filter: brightness(1.25);
}

.storeview .snc-modal .modal-body .action {
  position: relative;
  width: 50%;
  height: 90%;
}

.storeview .snc-modal .modal-body .action .buytab {
  width: 199px;
  height: 206px;
  border-radius: 3px;
  background-image: url("../../assets/images/purchase_coins_bg.png");
  position: absolute;
  bottom: 0;
  right: 0;
  border: 1px solid #222840;
}

.storeview .snc-modal .modal-body .action .buytab .title {
  background-color: #222840;
  height: 17%;
  width: 100%;
  font-family: LoLFont2;
  text-align: center;
  padding: 0px;
  display: flex;
  flex-direction: column;
  font-size: 13px;
}

.storeview .snc-modal .modal-body .action .buytab .listings {
  height: 83%;
  width: 100%;
  display: flex;
  padding: 5px;
  padding-top: 20px;
  flex-direction: column;
}

.storeview .snc-modal .modal-body .action .buytab .listings .listing {
  display: flex;
  justify-content: space-between;
  font-family: LoLFont2;
  font-size: 13px;
  padding: 5px;
  margin-bottom: 10px;
}

.storeview
  .snc-modal
  .modal-body
  .action
  .buytab
  .listings
  .listing:nth-child(2) {
  border-bottom: 3px solid rgba(44, 53, 60, 0.75);
  margin-bottom: 0;
}

.storeview
  .snc-modal
  .modal-body
  .action
  .buytab
  .listings
  .listing
  span:first-child {
  font-weight: bold;
}

.storeview .snc-modal .modal-body .action .buytab .listings button {
  display: flex;
  justify-content: space-between;
  height: 40px;
  width: 70%;
  margin: 0 auto;
  color: white;
  font-size: 14px;
  font-family: LoLFont2;
  border: none;
  padding: 7px 10px;
  outline: none;
  line-height: 24px;
  background: linear-gradient(
    180deg,
    #1f2e43 0%,
    #0f2034 45%,
    #05162a 50%,
    #04192c 100%
  );
  border-radius: 4px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.35);
}

.storeview .snc-modal .modal-body .action .buytab .listings button:disabled {
  filter: grayscale(100%);
}

.storeview
  .snc-modal
  .modal-body
  .action
  .buytab
  .listings
  button:not(:disabled):hover {
  filter: brightness(1.25);
  cursor: pointer;
}

.storeview .snc-modal .modal-body .action .buytab .listings button img {
  width: 25px;
  height: 25px;
}

.storeview .tabs {
  width: 15%;
  height: 100%;
  display: flex;
  flex-direction: column;
  position: relative;
  z-index: 3 !important;
}

.storeview .tabs a {
  border-radius: 6px;
  border-top-right-radius: 0px;
  border-bottom-right-radius: 0px;
  padding: 10px;
  text-align: center;
  height: 50px;
  margin-bottom: 2px;
  border: 2px solid rgba(34, 44, 58, 0.6);
  border-right: none;
  border-bottom: 1px solid rgba(34, 44, 58, 0.6);
  width: calc(100% - 2px);
  text-shadow: 0 0px 7px rgba(255, 255, 255, 0.55);
  font-family: LoLFont2;
  font-size: 14px;
  transition: filter ease-in-out 200ms;
  position: relative;
  background-color: #06101e;
}
.storeview .tabs a::after {
  content: "";
  border-radius: 4px;
  width: 100%;
  height: 100%;
  position: absolute;
  left: 0;
  top: 0;
  background-image: linear-gradient(
    180deg,
    #1f2e43 0%,
    #0f2034 45%,
    #05162a 50%,
    #04192c 100%
  );
  z-index: -1;
  border-top-right-radius: 0px;
  border-bottom-right-radius: 0px;
  -webkit-mask-image: linear-gradient(90deg, black 80%, transparent 100%);
}

.storeview .tabs a:not(.router-link-exact-active) {
  padding: 12px;
}
.storeview .tabs a.router-link-exact-active {
  left: 3px;
  width: 100%;
  border: none;
  padding: 13px;
  background: linear-gradient(
      120deg,
      rgba(21, 66, 109, 0.75) 0%,
      transparent 90%
    ),
    linear-gradient(90deg, transparent 0%, rgb(7, 11, 21) 100%),
    linear-gradient(
      180deg,
      rgba(255, 255, 255, 0.15) 0%,
      #1f2e43 10%,
      #0f2034 45%,
      #05162a 50%,
      #04192c 90%,
      rgba(255, 255, 255, 0.15) 100%
    );
  box-shadow: 0 0 10px rgba(21, 66, 109, 0.75);
  -webkit-mask-image: linear-gradient(90deg, black 98%, transparent 100%);
}

.storeview .tabs a:not(.router-link-exact-active):hover {
  filter: brightness(1.25);
}

.storeview .tabs a.router-link-exact-active::after {
  box-shadow: inset 0 0 10px rgba(58, 148, 233, 0.75);
  -webkit-mask-image: linear-gradient(90deg, black 70%, transparent 100%);
}

.storeview .content {
  width: 85%;
  height: 100%;
  background: linear-gradient(180deg, rgb(7, 11, 21) 0%, rgb(7, 16, 31) 100%);
  padding: 10px;
  border: 1px solid rgb(23, 31, 43);
  position: relative;
}
</style>
