<template>
  <div id="app">
    <private-chat />
    <friend-contextmenu />
    <logout />
    <setter-modal />
    <matchmaking />
    <summonericonmodal />
    <reconnectmodal />
    <Menu />
    <div id="content-holder">
      <Background />
      <router-view class="router-content" />
    </div>
  </div>
</template>

<script>
import Menu from "@/components/MenuBar.vue";
import { ipcRenderer, shell } from "electron";
import Background from "@/components/Background.vue";
export default {
  components: {
    Menu,
    Background
  },
  methods: {
    closeApp() {
      ipcRenderer.send("closeApp");
    },
    minimizeApp() {
      ipcRenderer.send("minimizeApp");
    },
    openExternal() {
      shell.openExternal("URL");
    }
  }
};
</script>

<style src="@/assets/css/global.css"></style>

<style lang="css">
@import "./assets/css/fonts.css";
* {
  box-sizing: border-box;
}
body {
  position: relative;
  margin: 0;
  padding: 0;
  overflow: hidden;
  color: white;
}
#app {
  -webkit-app-region: no-drag;
  height: 720px;
  width: 1280px;
  overflow: hidden;
  font-family: LoLFont4;
}
#content-holder {
  width: 100%;
  height: 100%;
}

#content-holder .router-content {
  position: relative;
  z-index: 1 !important;
  padding: 0 10px;
}

#content-holder * {
  z-index: inherit;
}
a,
a:visited,
a:hover,
a:link {
  text-decoration: none;
  color: unset;
}
::-webkit-scrollbar {
  width: 7px;
}
::-webkit-scrollbar-thumb {
  background-size: 100% 100%;
  background-color: transparent;
  border-radius: 7px;
  background-image: url("./assets/images/scroll-thumb.png");
}
::-webkit-scrollbar-button {
  display: none;
}
::-webkit-scrollbar-track {
  display: none;
}
::-webkit-scrollbar-track-piece {
  display: none;
}
* {
  user-select: none;
  -webkit-user-drag: none;
}
</style>
