<template>
  <div v-if="visible">
    <div class="backdrop"></div>
    <div class="modal" ref="modal" tabindex="-1">
      <div class="head">
        {{ t("RCMDL_TITLE") }}
      </div>
      <div class="body">
        <div class="fa-3x">
          <i class="fas fa-sync fa-spin"></i>
        </div>
        <div>
          {{ t("RCMDL_RECONNECTING") }}
          <button @click="closeApp()">
            {{ t("RCMDL_EXIT_BTN") }} <i class="far fa-times-octagon"></i>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ipcRenderer } from "electron";
import Modal from "../../plugins/modals";
export default {
  data() {
    return {
      visible: false
    };
  },
  methods: {
    show() {
      this.visible = true;
    },
    hide() {
      this.visible = false;
    },
    closeApp() {
      ipcRenderer.send("closeApp");
    }
  },
  beforeMount() {
    Modal.EventBus.$on("reconnect-modal-show", () => {
      this.show();
    });
    Modal.EventBus.$on("reconnect-modal-hide", () => {
      this.hide();
    });
  }
};
</script>

<style lang="scss" scoped>
.backdrop {
  height: 95%;
  width: 100%;
  position: absolute;
  z-index: 9 !important;
  background-color: rgba(0, 0, 0, 0.5);
}

.modal {
  position: absolute;
  left: 50%;
  top: 50%;
  width: 300px;
  height: 175px;
  border-radius: 7px;
  padding: 0 10px;
  transform: translate(-50%, -50%);
  z-index: 10 !important;
  outline: none;
  display: flex;
  flex-direction: column;
  border: 1px solid rgba(3, 39, 82, 0.75);
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
  height: calc(100% - 30px);
  display: flex;
  flex-direction: column;
  padding: 10px 0;
  justify-content: center;
}

.modal .body > div {
  width: 100%;
  text-align: center;
  margin-bottom: 10px;
}

.modal .body > div:nth-child(2) {
  display: flex;
  flex-direction: column;
}

.modal .body > div button {
  margin-top: 10px;
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
  padding: 5px 30px;
  border: 1px solid #304b69;
  color: white;
  font-family: LoLFont2;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.modal .body > div button:hover {
  filter: brightness(1.25);
}
</style>
