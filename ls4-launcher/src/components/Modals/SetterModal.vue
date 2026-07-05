<template>
  <div class="modal" v-if="visible">
    <div class="modal-body lolblock">
      <div class="head">
        {{ title }}
      </div>
      <div class="inner">
        <div class="body" v-html="body"></div>
        <div class="buttons">
          <button v-if="onConfirm" @click="confirm">
            {{ confirmText ? confirmText : "Confirm" }}
          </button>
          <button @click="hide">Close</button>
        </div>
        <div class="footer" v-if="footer">{{ footer }}</div>
      </div>
    </div>
  </div>
</template>

<script>
import Modal from "../../plugins/modals";

export default {
  data() {
    return {
      visible: false,
      title: null,
      body: null,
      footer: null,
      onConfirm: null,
      confirmText: null
    };
  },
  methods: {
    hide() {
      this.visible = false;
    },
    confirm() {
      if (typeof this.onConfirm === "function") {
        this.onConfirm();
        this.hide();
      } else {
        this.hide();
      }
    },
    show(params) {
      this.visible = true;
      this.title = params.title;
      this.body = params.body;
      this.footer = params.footer;
      this.onConfirm = params.onConfirm;
      this.confirmText = params.confirmText;
    }
  },
  beforeMount() {
    Modal.EventBus.$on("setter-show", params => {
      this.show(params);
    });
  }
};
</script>

<style lang="scss" scoped>
.modal {
  position: absolute;
  left: 50%;
  top: 50%;
  width: 300px;
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
  background-color: rgba(5, 12, 20, 1);
}

.lolblock .head {
  height: 30px;
  width: 100%;
  text-align: center;
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
  width: 100%;
  height: calc(100% - 30px);
  display: flex;
  flex-direction: column;
  padding: 10px 0;
}

.lolblock .inner .body {
  width: 100%;
  font-size: 12px;
  font-family: LoLFont2;
  height: 75%;
}

.lolblock .inner .buttons {
  height: 20%;
  width: 100%;
  display: flex;
  justify-content: space-around;
}
.lolblock .inner .footer {
  width: 100%;
  font-size: 10px;
  font-family: LoLFont2;
  height: 5%;
  text-align: center;
  padding: 3px 5px;
}

.lolblock .inner .buttons button {
  width: 33%;
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
