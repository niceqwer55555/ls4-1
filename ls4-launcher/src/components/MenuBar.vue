<template>
  <div class="menu-bar">
    <button id="minimizeApp" @click="minimizeApp">
      <img src="@/assets/images/min_btn.png" alt="minimize" />
    </button>
    <button id="exitApp" v-on:click="loggedin ? logoutModal() : closeApp()">
      <img src="@/assets/images/exit_btn.png" alt="close" />
    </button>
  </div>
</template>

<style lang="scss" scoped>
.menu-bar {
  display: flex;
  justify-content: flex-end;
  height: 11px;
  width: 100%;
  right: 5px;
  position: absolute;
  z-index: 3;
  box-sizing: border-box;
  -webkit-app-region: drag;
}

.menu-bar button {
  -webkit-app-region: no-drag;
  pointer-events: all;
  outline: none;
  border: none;
  width: 20px;
  height: 20px;
  background-color: transparent;
  padding: 3px 0;
  margin-left: 5px;
  box-sizing: border-box;
  cursor: pointer;
  opacity: 0.7;
  transition: opacity ease-in-out 200ms;
}

.menu-bar button#exitApp {
  background-image: url("../assets/images/exit_btn_bg.png");
}

.menu-bar button#minimizeApp {
  background-image: url("../assets/images/min_btn_bg.png");
}

.menu-bar button img {
  object-position: bottom;
}

.menu-bar button:hover {
  opacity: 1;
}
</style>

<script>
import { ipcRenderer } from "electron";
import { mapState } from "vuex";
export default {
  computed: mapState({
    loggedin: state => state.token != ""
  }),
  methods: {
    minimizeApp() {
      ipcRenderer.send("minimizeApp");
    },
    closeApp() {
      ipcRenderer.send("closeApp");
    },
    logoutModal() {
      this.$modal.logout.show();
    }
  }
};
</script>
