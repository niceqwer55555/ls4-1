<template>
  <div
    class="modal"
    ref="modal"
    tabindex="-1"
    v-if="visible"
    v-on:focusout="hide($event)"
  >
    <div class="modal-body lolblock">
      <div class="head">
        {{ t("LGMDL_TITLE") }}
      </div>
      <div class="inner">
        <button @click="closeApp">
          <i class="far fa-times-octagon"></i>
          {{ t("LGMDL_EXIT_BTN") }}
        </button>
        <button @click="logout">
          <i class="far fa-door-open"></i>
          {{ t("LGMDL_LOGOUT_BTN") }}
        </button>
      </div>
    </div>
  </div>
</template>

<script>
// import { mapState } from "vuex";
import Modal from "../../plugins/modals";
import { ipcRenderer } from "electron";

export default {
  data() {
    return {
      visible: false
    };
  },
  methods: {
    hide(event = false) {
      if (event) {
        if (event.target.contains(event.relatedTarget)) {
          event.relatedTarget.click();
          this.visible = false;
        }
      }
      this.visible = false;
    },
    show() {
      this.visible = true;
    },
    logout() {
      const router = this.$router;
      this.$store
        .dispatch("logUserOut")
        .then(() => {
          this.$store.dispatch("changeBackgroundState", "HOME");
          router.push("/");
        })
        .catch(err => {
          console.error(err);
        });
    },
    closeApp() {
      ipcRenderer.send("closeApp");
    }
  },
  beforeMount() {
    Modal.EventBus.$on("logout-show", () => {
      this.show();
    });
  },
  updated() {
    if (this.$refs.modal) {
      this.$refs.modal.focus();
    }
  }
};
</script>

<style lang="scss" scoped>
.modal {
  position: absolute;
  left: 50%;
  top: 50%;
  width: 300px;
  height: 100px;
  border-radius: 7px;
  padding: 10px;
  box-sizing: border-box;
  transform: translate(-50%, -50%);
  z-index: 10 !important;
  outline: none;
}

.lolblock {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100%;
  border: 1px solid rgba(3, 39, 82, 0.75); //origcolor rgb(100, 117, 137)
  border-radius: 5px;
  overflow: hidden;
}

.lolblock .head {
  height: 30px;
  width: 100%;
  font-size: 14px;
  font-family: LoLFont2;
  padding: 5px;
  background-image: linear-gradient(
    180deg,
    #192e49 0%,
    #192e49 40%,
    #172b46 50%,
    #142131 100%
  );
}

.lolblock .inner {
  background-color: rgba(5, 12, 20, 1);
  width: 100%;
  height: calc(100% - 30px);
  display: flex;
  padding: 10px 0;
  justify-content: space-around;
}

.lolblock .inner button {
  width: 47%;
  height: 30px;
  background: linear-gradient(
    180deg,
    #3c73b4 0%,
    #20477e 45%,
    #1e3e6d 50%,
    #0e284b 100%
  );
  outline: none;
  border: none;
  font-size: 12px;
  border-radius: 2px;
  text-align: center;
  border: 1px solid #304b69;
  color: white;
  font-family: LoLFont2;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

.lolblock .inner button:hover {
  filter: brightness(1.25);
}
</style>
