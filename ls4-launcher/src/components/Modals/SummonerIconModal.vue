<template>
  <div class="modal" v-if="visible" tabindex="-1">
    <div class="head">
      {{ t("SMIMDL_TITLE") }}
    </div>
    <div class="body">
      <div class="iconlist">
        <div
          class="icon"
          v-for="(icon, index) in icons"
          :key="index"
          :class="{ selected: icon.id == selectedIconId }"
          @click="selectIcon(icon)"
        >
          <img :src="getSummonerIconUrl(icon)" />
        </div>
      </div>
      <div class="description">
        {{ t("SMIMDL_DESCRIPTION") }}
      </div>
    </div>
    <div class="foot">
      <button type="button" @click="hide">{{ t("SMIMDL_CANCEL_BTN") }}</button>
      <button type="button" @click="submitIcon">
        {{ t("SMIMDL_OK_BTN") }}
      </button>
    </div>
  </div>
</template>

<script>
import { mapState } from "vuex";
import Modal from "../../plugins/modals";

export default {
  data() {
    return {
      visible: false,
      selectedIconId: null
    };
  },
  mounted() {
    this.selectedIcon = this.$store.state.user.summonerIconId;
  },
  computed: mapState({
    user: state => state.user,
    icons: state => {
      return state.collection.icons ? state.collection.icons.owned : [];
    }
  }),
  methods: {
    selectIcon(icon) {
      this.$sound.template("OVERVIEW_CLICK");
      this.selectedIconId = icon.id;
    },
    submitIcon() {
      this.$sound.template("CORE_CLICK");
      this.$socket.sendSystemMessage(
        "USER_UPDATE_ICON",
        { data: this.selectedIconId },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          this.$store.dispatch("setSummonerIcon", this.selectedIconId);
          this.hide();
          this.selectedIconId = this.$store.state.user.summonerIconId;
        }
      );
    },
    getSummonerIconUrl(icon) {
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/summoner_icons/${icon.id}.png`;
    },
    hide() {
      this.visible = false;
    },
    show() {
      console.log("SHOWING");
      if (this.$store.state.collection.icons.length === 0) {
        this.$store.dispatch("getCollectionIcons");
      }
      this.visible = true;
    }
  },
  beforeMount() {
    Modal.EventBus.$on("summonericon-modal-show", () => {
      this.show();
    });
  }
};
</script>

<style lang="scss" scoped>
.modal {
  position: absolute;
  left: 50%;
  top: 50%;
  width: 55%;
  height: 300px;
  border-radius: 7px;
  padding: 0 10px;
  transform: translate(-50%, -50%);
  z-index: 10 !important;
  outline: none;
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(3, 39, 82, 0.75); //origcolor rgb(100, 117, 137)
  background: linear-gradient(
    90deg,
    rgba(12, 15, 19, 0.95) 100%,
    rgba(20, 24, 29, 0.95) 0%
  );
  overflow: hidden;
}

.modal .head {
  height: 30px;
  width: 100%;
  font-size: 16px;
  font-family: LoLFont2;
  padding: 10px 5px 0px 5px;
}

.modal .body {
  width: 100%;
  height: calc(100% - 60px);
  display: flex;
  padding: 10px 0;
  justify-content: space-around;
}

.modal .body > div {
  width: 50%;
}

.modal .body .description {
  padding: 5px 10px;
  font-family: LoLFont2;
  text-align: justify;
  font-size: 14px;
}

.modal .body .iconlist {
  display: flex;
  flex-wrap: wrap;
  overflow: auto;
}

.modal .body .iconlist .icon {
  width: 75px;
  height: 75px;
  border: 1px solid rgba(255, 255, 255, 0.75);
  margin-bottom: 10px;
  cursor: pointer;
  transition: filter ease-in-out 200ms;
}

.modal .body .iconlist .icon.selected {
  border: 1px solid rgba(175, 128, 0, 0.75);
}

.modal .body .iconlist .icon:hover {
  filter: brightness(1.25);
}

.modal .body .iconlist .icon:not(:nth-child(4n)) {
  margin-right: 10px;
}

.modal .body .iconlist .icon img {
  height: 100%;
  width: 100%;
}

.modal .foot {
  display: flex;
  justify-content: flex-end;
}

.modal .foot button {
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
  padding: 5px 30px;
  border: 1px solid #304b69;
  color: white;
  font-family: LoLFont2;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.modal .foot button:hover {
  filter: brightness(1.25);
}
</style>
