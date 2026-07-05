<template>
  <div class="container">
    <div class="pingdisplay">PING: {{ ping }}</div>
    <div class="modeblock">
      <div class="modeselect">
        <div class="modes">
          <div
            class="mode"
            @click="selectMode(mode)"
            v-for="(mode, index) in modes"
            :key="index"
            :class="{
              inactive: mode.disabled,
              selected: mode.name == selectedMode.name
            }"
          >
            {{ mode.name }}
          </div>
        </div>
        <div class="selects">
          <!-- me neither -->
          <div class="list">
            <div class="head">
              {{ t("PLAY_GAME_MODE") }}
            </div>
            <div class="items">
              <div
                v-for="(gamemode, index) in selectedMode.gamemodes"
                :key="index"
                :class="{
                  subtexted: gamemode.sub != null,
                  selected: gamemode.id == selectedGameMode.id
                }"
                @click="selectGameMode(gamemode)"
              >
                <span>
                  {{ gamemode.name }}
                </span>
                <span v-if="gamemode.sub != null">
                  {{ gamemode.sub }}
                </span>
              </div>
            </div>
          </div>
          <div class="list">
            <div class="head">
              {{ t("PLAY_GAME_MAP") }}
            </div>
            <div class="items">
              <div
                v-for="(map, index) in selectedGameMode.maps"
                :key="index"
                :class="{
                  subtexted: map.sub != null,
                  selected: map.id == selectedGameMap.id
                }"
                @click="selectGameMap(map)"
              >
                <span>
                  {{ map.name }}
                </span>
                <span v-if="map.sub != null">
                  {{ map.sub }}
                </span>
              </div>
            </div>
          </div>
          <div class="list">
            <div class="head">
              {{ t("PLAY_GAME_TYPE") }}
            </div>
            <div class="items">
              <div
                v-for="(type, index) in selectedGameMap.types"
                :key="index"
                :class="{
                  subtexted: type.sub != null,
                  selected: type.id == selectedGameType.id
                }"
                @click="selectGameType(type)"
              >
                <span>
                  {{ type.name }}
                </span>
                <span v-if="type.sub != null">
                  {{ type.sub }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="modeinfo">
        <div class="inner" v-if="selectedGameMode" :style="getModeInfoStyles()">
          <div class="data">
            <div class="head">
              <h2>{{ selectedGameMode.name }}</h2>
              <h3>{{ selectedGameMap.name }}</h3>
              <p>{{ selectedGameType.name }} ({{ selectedGameType.sub }})</p>
            </div>
            <div class="description">
              {{ selectedGameMode.description }}
            </div>
            <button @click="createLobby" class="createLobby">
              {{
                inLobby
                  ? t("PLAY_CHANGE_LOBBY_TYPE_BTN")
                  : t("PLAY_CREATE_LOBBY_BTN")
              }}
            </button>
            <button @click="goHome" class="goHome">
              {{ t("PLAY_HOME_BTN") }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<!-- <div class="nav-view">
  <div class="page-links">
    <router-link to="/LoggedIn/play/pvp">PVP</router-link>
  </div>
  <div class="navView">
    <router-view></router-view>
  </div>
</div> -->

<script>
import { ipcRenderer } from "electron";
import { mapState } from "vuex";
export default {
  data() {
    return {
      ping: "...",
      selectedMode: null,
      selectedGameMode: null,
      selectedGameMap: null,
      selectedGameType: null,
      modes: [
        {
          name: this.$translate.text("PLAY_DATA_LOBBY_PVP"),
          custom: false,
          disabled: this.$store.state.lobby
            ? this.$store.state.lobby.isCustom &&
              this.$store.state.lobby.members.length > 5
              ? true
              : false
            : false,
          selected: false,
          gamemodes: [
            {
              id: 1,
              name: this.$translate.text("PLAY_DATA_MODE_CLASSIC"),
              selected: false,
              bgimage: "static/images/gamemodes/gamemode_bg_classic.jpg",
              maps: [
                {
                  id: 1,
                  name: this.$translate.text("PLAY_DATA_MAP_SR"),
                  sub: "5x5",
                  code: "SUMMONERS_RIFT",
                  selected: false,
                  types: [
                    {
                      id: 1,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_BLIND"),
                      selected: false,
                      code: "BLIND"
                    },
                    {
                      id: 2,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_DRAFT"),
                      selected: false,
                      code: "DRAFT"
                    }
                  ]
                },
                {
                  id: 2,
                  name: this.$translate.text("PLAY_DATA_MAP_TT"),
                  sub: "3x3",
                  code: "TWISTED_TREELINE",
                  selected: false,
                  types: [
                    {
                      id: 1,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_BLIND"),
                      selected: false,
                      code: "BLIND"
                    },
                    {
                      id: 2,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_DRAFT"),
                      selected: false,
                      code: "DRAFT"
                    }
                  ]
                },
                {
                  id: 3,
                  name: this.$translate.text("PLAY_DATA_MAP_ODIN"),
                  sub: "5x5",
                  code: "ODIN",
                  selected: false,
                  types: [
                    {
                      id: 1,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_BLIND"),
                      selected: false,
                      code: "BLIND"
                    },
                    {
                      id: 2,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_DRAFT"),
                      selected: false,
                      code: "DRAFT"
                    }
                  ]
                }
              ]
            },
            {
              id: 2,
              name: this.$translate.text("PLAY_DATA_MODE_ARAM"),
              selected: false,
              bgimage: "static/images/gamemodes/gamemode_bg_classic.jpg",
              maps: [
                {
                  id: 1,
                  name: this.$translate.text("PLAY_DATA_MAP_HA"),
                  sub: "5x5",
                  code: "ARAM",
                  selected: false,
                  types: [
                    {
                      id: 1,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_RANDOM"),
                      selected: false,
                      code: "BLIND"
                    }
                  ]
                }
              ]
            }
          ]
        },
        {
          name: this.$translate.text("PLAY_DATA_LOBBY_COOPVSAI"),
          custom: false,
          disabled: false,
          selected: false,
          gamemodes: [
            {
              id: 1,
              name: this.$translate.text("PLAY_DATA_MODE_CLASSIC"),
              description: "与AI队友一起对抗敌方AI，练习你的技能！",
              selected: false,
              bgimage: "static/images/gamemodes/gamemode_bg_classic.jpg",
              maps: [
                {
                  id: 1,
                  name: this.$translate.text("PLAY_DATA_MAP_SR"),
                  sub: "5x5",
                  code: "SUMMONERS_RIFT",
                  selected: false,
                  types: [
                    {
                      id: 1,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_BLIND"),
                      selected: false,
                      code: "BOT_BLIND"
                    },
                    {
                      id: 2,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_DRAFT"),
                      selected: false,
                      code: "BOT_DRAFT"
                    }
                  ]
                },
                {
                  id: 2,
                  name: this.$translate.text("PLAY_DATA_MAP_ODIN"),
                  sub: "5x5",
                  code: "ODIN",
                  selected: false,
                  types: [
                    {
                      id: 1,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_BLIND"),
                      selected: false,
                      code: "BOT_BLIND"
                    },
                    {
                      id: 2,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_DRAFT"),
                      selected: false,
                      code: "BOT_DRAFT"
                    }
                  ]
                }
              ]
            },
            {
              id: 2,
              name: this.$translate.text("PLAY_DATA_MODE_ARAM"),
              description: "随机英雄，与AI队友一起在嚎哭深渊战斗！",
              selected: false,
              bgimage: "static/images/gamemodes/gamemode_bg_classic.jpg",
              maps: [
                {
                  id: 1,
                  name: this.$translate.text("PLAY_DATA_MAP_HA"),
                  sub: "5x5",
                  code: "ARAM",
                  selected: false,
                  types: [
                    {
                      id: 1,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_RANDOM"),
                      selected: false,
                      code: "BOT_ARAM"
                    }
                  ]
                }
              ]
            }
          ]
        },
        {
          name: this.$translate.text("PLAY_DATA_LOBBY_CUSTOM"),
          custom: true,
          disabled: false,
          selected: false,
          gamemodes: [
            {
              id: 1,
              name: this.$translate.text("PLAY_DATA_MODE_CLASSIC"),
              selected: false,
              bgimage: "static/images/gamemodes/gamemode_bg_classic_custom.jpg",
              maps: [
                {
                  id: 1,
                  name: this.$translate.text("PLAY_DATA_MAP_SR"),
                  sub: "5x5",
                  code: "SUMMONERS_RIFT",
                  selected: false,
                  types: [
                    {
                      id: 1,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_BLIND"),
                      selected: false,
                      code: "BLIND"
                    },
                    {
                      id: 2,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_DRAFT"),
                      selected: false,
                      code: "DRAFT"
                    }
                  ]
                },
                {
                  id: 2,
                  name: this.$translate.text("PLAY_DATA_MAP_TT"),
                  sub: "3x3",
                  code: "TWISTED_TREELINE",
                  selected: false,
                  types: [
                    {
                      id: 1,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_BLIND"),
                      selected: false,
                      code: "BLIND"
                    },
                    {
                      id: 2,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_DRAFT"),
                      selected: false,
                      code: "DRAFT"
                    }
                  ]
                },
                {
                  id: 3,
                  name: this.$translate.text("PLAY_DATA_MAP_ODIN"),
                  sub: "5x5",
                  code: "ODIN",
                  selected: false,
                  types: [
                    {
                      id: 1,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_BLIND"),
                      selected: false,
                      code: "BLIND"
                    },
                    {
                      id: 2,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_DRAFT"),
                      selected: false,
                      code: "DRAFT"
                    }
                  ]
                }
              ]
            },
            {
              id: 2,
              name: this.$translate.text("PLAY_DATA_MODE_ARAM"),
              selected: false,
              bgimage: "static/images/gamemodes/gamemode_bg_classic_custom.jpg",
              maps: [
                {
                  id: 1,
                  name: this.$translate.text("PLAY_DATA_MAP_HA"),
                  sub: "5x5",
                  code: "ARAM",
                  selected: false,
                  types: [
                    {
                      id: 1,
                      name: this.$translate.text("PLAY_DATA_TYPE_NORMAL"),
                      sub: this.$translate.text("PLAY_DATA_PICK_RANDOM"),
                      selected: false,
                      code: "BLIND"
                    }
                  ]
                }
              ]
            }
          ]
        },
        {
          name: this.$translate.text("PLAY_DATA_LOBBY_TUTORIAL"),
          custom: false,
          disabled: true,
          selected: false,
          gamemodes: []
        }
      ]
    };
  },
  created() {
    const host = this.$store.state.config.api.host.includes("//")
      ? this.$store.state.config.api.host.split("//")[1]
      : null;

    if (host !== null) {
      ipcRenderer.invoke("ping", host).then(result => {
        if (result.alive) {
          this.ping = Math.round(result.avg) + " ms";
        } else {
          this.ping = "Unknown";
        }
      });
    }
  },
  computed: mapState({
    inLobby: state => {
      return state.lobby.lobbyType;
    }
  }),
  methods: {
    goHome() {
      this.$sound.template("OVERVIEW_CLICK");
      this.$router.push("/LoggedIn/home");
    },
    createLobby() {
      this.$sound.template("OVERVIEW_CLICK");
      const custom = this.selectedMode.custom;
      const lobbyType = `${this.selectedGameMap.code}_${this.selectedGameType.code}`;
      const router = this.$router;

      if (this.$store.state.lobby.owner) {
        this.$socket.sendLobbyMessage(
          "LOBBY_CHANGE_TYPE",
          { data: { lobbyType: lobbyType, isCustom: custom } },
          (response, error) => {
            if (error) {
              console.log("Flyback error:");
              console.log(error);
            }

            if (custom && this.$route.name !== "LobbyCustom") {
              router.push("/lobbyCustom");
            } else if (!custom && this.$route.name !== "Lobby") {
              router.push("/lobby");
            }
          }
        );
      } else {
        this.$socket.sendSystemMessage(
          "LOBBY_CREATE",
          { data: { lobbyType: lobbyType, isCustom: custom } },
          (response, error) => {
            if (error) {
              console.log("Flyback error:");
              console.log(error);
            }

            if (custom) {
              router.push("/lobbyCustom");
            } else {
              router.push("/lobby");
            }
          }
        );
      }
    },
    getModeInfoStyles() {
      return `background-image: url('${this.selectedGameMode.bgimage}')`;
    },
    selectMode(mode) {
      if (mode.disabled) return;
      if (this.selectedMode == mode) return;
      this.selectedMode = mode;
      this.selectedGameMode = this.selectedMode.gamemodes[0];
      this.selectedGameMap = this.selectedGameMode.maps[0];
      this.selectedGameType = this.selectedGameMap.types[0];
      this.$sound.template("CORE_CLICK");
    },
    selectGameMode(gamemode) {
      if (this.selectedGameMode == gamemode) return;
      this.selectedGameMode = gamemode;
      this.selectedGameMap = this.selectedGameMode.maps[0];
      this.selectedGameType = this.selectedGameMap.types[0];
      this.$sound.template("CORE_CLICK");
    },
    selectGameMap(map) {
      if (this.selectedGameMap == map) return;
      this.selectedGameMap = map;
      this.selectedGameType = this.selectedGameMap.types[0];
      this.$sound.template("CORE_CLICK");
    },
    selectGameType(type) {
      if (this.selectedGameType == type) return;
      this.selectedGameType = type;
      this.$sound.template("CORE_CLICK");
    }
  },
  beforeMount() {
    const DefaultIndex = this.$store.state.lobby
      ? this.$store.state.lobby.isCustom
        ? 2
        : 0
      : 0;
    this.selectedMode = this.modes[DefaultIndex];
    this.selectedGameMode = this.selectedMode.gamemodes[0];
    this.selectedGameMap = this.selectedGameMode.maps[0];
    this.selectedGameType = this.selectedGameMap.types[0];
    this.$store.dispatch("changeBackgroundState", "MODESELECT");
  }
};
</script>

<style lang="css" scoped>
.container {
  display: flex;
  justify-content: flex-end;
  position: relative;
}

.container .pingdisplay {
  position: absolute;
  left: 5px;
  bottom: 5px;
  font-size: 12px;
  font-family: LoLFont2;
}

.container .modeblock {
  width: 79%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.container .modeblock > div {
  border: 2px solid rgba(86, 86, 86, 0.75);
  border-radius: 5px;
  background-color: #030d19;
}

/*
.container .modeblock .modeselect::after {
  position: absolute;
  content: "";
  pointer-events: none;
  width: 100%;
  height: 100%;
  left: 0;
  top: 0;
  box-shadow: inset 0 0 6px 4px rgba(37, 48, 66, 1);
  z-index: 10;
}
*/

.container .modeblock .modeinfo {
  height: 67%;
  box-shadow: inset 0 0 6px 2px rgba(0, 0, 0, 0.75);
  overflow: hidden;
}

.container .modeblock .modeinfo .inner {
  background-size: 100% 100%;
  background-position: center;
  width: 100%;
  height: 100%;
  position: relative;
}

.container .modeblock .modeinfo .inner .data {
  position: absolute;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  padding: 20px;
}

.container .modeblock .modeinfo .inner .data button {
  position: absolute;
  bottom: 10px;
  width: auto;
  min-width: 100px;
  height: 25px;
  outline: none;
  border: none;
  opacity: 0.9;
  border-radius: 3px;
  cursor: pointer;
  transition: filter ease-in-out 200ms;
}

.container .modeblock .modeinfo .inner .data button:hover {
  filter: brightness(1.2);
  opacity: 1;
}

.container .modeblock .modeinfo .inner .data .createLobby {
  left: 120px;
  background-image: url("../../assets/images/button-default.png");
  background-size: 100% 100%;
  font-family: LoLFont2;
  font-size: 12px;
  color: white;
  font-weight: bold;
}

.container .modeblock .modeinfo .inner .data .goHome {
  left: 10px;
  background-image: url("../../assets/images/social-button-bg.png");
  background-size: 100% 100%;
  font-family: LoLFont2;
  font-size: 12px;
  color: white;
  font-weight: bold;
}

.container .modeblock .modeinfo .inner .data .head {
  display: flex;
  flex-direction: column;
  margin-bottom: 30px;
}

.container .modeblock .modeinfo .inner .data .head > * {
  margin: 0;
}

.container .modeblock .modeinfo .inner .data .head > h2 {
  -webkit-text-stroke: 1px black;
  font-size: 36px;
  line-height: 36px;
  color: #f2d030;
}

.container .modeblock .modeinfo .inner .data .head > h3 {
  -webkit-text-stroke: 1px black;
  font-size: 24px;
}

.container .modeblock .modeinfo .inner .data .head > p {
  font-size: 16px;
  -webkit-text-stroke: 0.5px black;
}

.container .modeblock .modeselect {
  position: relative;
  height: 33%;
  display: flex;
}

.container .modeblock .modeselect .modes {
  display: flex;
  flex-direction: column;
  width: 22%;
}

.container .modeblock .modeselect .modes .mode {
  border-radius: 4px;
  background-image: linear-gradient(
    180deg,
    rgba(255, 255, 255, 0.15) 0%,
    #1f2e43 10%,
    #0f2034 45%,
    #05162a 50%,
    #04192c 90%,
    rgba(255, 255, 255, 0.15) 100%
  );
  padding: 10px;
  text-align: center;
  height: 25%;
  border: 2px solid #223b5a;
  width: 100%;
  text-shadow: 0 0px 7px rgba(255, 255, 255, 0.55);
  font-family: LoLFont2;
  font-size: 14px;
  transition: filter ease-in-out 200ms;
}

.container .modeblock .modeselect .modes .mode.inactive {
  filter: grayscale(100%);
}

.container
  .modeblock
  .modeselect
  .modes
  .mode:not(.selected):not(.inactive):hover {
  filter: brightness(1.25);
  cursor: pointer;
}

.container .modeblock .modeselect .modes .mode.selected {
  /* border-top-right-radius: 0px;
   border-bottom-right-radius: 0px; */
  border-right: none;
  border-top: none;
  padding: 13px;
  border-bottom: none;
  background: linear-gradient(
      120deg,
      rgba(13, 71, 49, 0.75) 0%,
      transparent 90%
    ),
    linear-gradient(90deg, transparent 0%, #030d19 100%),
    linear-gradient(
      180deg,
      rgba(255, 255, 255, 0.15) 0%,
      #1f2e43 10%,
      #0f2034 45%,
      #05162a 50%,
      #04192c 90%,
      rgba(255, 255, 255, 0.15) 100%
    );
}

.container .modeblock .modeselect .selects {
  width: 78%;
  display: flex;
  justify-content: space-around;
  height: 100%;
}

.container .modeblock .modeselect .selects .list {
  height: 100%;
  width: 30%;
  display: flex;
  flex-direction: column;
  padding-bottom: 10px;
}

.container .modeblock .modeselect .selects .list .head {
  width: 100%;
  height: 20%;
  text-shadow: 0 0px 7px rgba(255, 255, 255, 0.55);
  font-family: LoLFont2;
  font-size: 14px;
  text-align: center;
  padding: 10px;
}

.container .modeblock .modeselect .selects .list .items {
  width: 100%;
  height: 80%;
  display: flex;
  flex-direction: column;
  background-color: #01060c;
  border-radius: 3px;
  border: 1px solid #0f2034;
  padding: 2px;
}

.container .modeblock .modeselect .selects .list .items div {
  text-shadow: 0 0px 7px rgba(255, 255, 255, 0.55);
  font-family: LoLFont2;
  font-size: 12px;
  height: 30px;
  width: 100%;
  padding: 5px;
  border-radius: 5px;
  cursor: pointer;
}

.container .modeblock .modeselect .selects .list .items div:hover {
  background-image: linear-gradient(
    90deg,
    rgba(13, 71, 49, 0.4) 0%,
    transparent 100%
  );
}

.container .modeblock .modeselect .selects .list .items div.selected {
  background-image: linear-gradient(
    90deg,
    rgba(13, 71, 49, 0.75) 0%,
    transparent 100%
  );
}

.container .modeblock .modeselect .selects .list .items div.subtexted {
  display: flex;
  flex-direction: column;
  height: 40px;
}

.container
  .modeblock
  .modeselect
  .selects
  .list
  .items
  div.subtexted
  span:nth-child(2) {
  font-size: 11px;
}
</style>
