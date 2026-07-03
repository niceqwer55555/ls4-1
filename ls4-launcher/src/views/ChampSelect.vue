<template>
  <div id="champselect">
    <div class="side ally">
      <div class="head">
        {{ t("CS_ALLY_TEAM_TITLE") }}
      </div>
      <div class="control">
        <AllyPlayer
          v-for="(player, index) in champselect.team"
          :key="index"
          :player="player"
        />
      </div>
      <div class="foot">
        <div class="lolblock">
          <div class="head">
            {{ t("CS_BANNED_CHAMPS_TITLE") }}
          </div>
          <div class="inner">
            <ChampionBan
              v-for="(ban, index) in allyBans"
              :key="index"
              :ban="ban"
            />
          </div>
        </div>
      </div>
    </div>
    <div class="main">
      <div class="head">
        <div class="phases">
          <div
            class="phase"
            :class="{
              inactive: this.globalState.phase != 'BAN',
              active: this.globalState.phase == 'BAN'
            }"
          >
            {{ t("CS_PHASE_STATUS_BAN") }}
          </div>
          <div
            class="phase"
            :class="{
              inactive: !['BAN', 'PICK'].includes(this.globalState.phase),
              active: this.globalState.phase == 'PICK'
            }"
          >
            {{ t("CS_PHASE_STATUS_PICK") }}
          </div>
          <div
            class="phase"
            :class="{
              active: this.globalState.phase == 'PRE'
            }"
          >
            {{ t("CS_PHASE_STATUS_FREE") }}
          </div>
        </div>
      </div>
      <div class="control">
        <div class="status">
          <div
            class="leftArrow arrow"
            :class="{
              inactive: getTeamState('ALLY') == ''
            }"
          >
            <div class="timer" v-if="getTeamState('ALLY') != ''">
              <i class="fal fa-clock"></i>
              {{ stateTime }}
            </div>
          </div>
          <div class="info">
            <div class="infomessage" v-html="getGlobalStateMessage()"></div>
          </div>
          <div
            class="rightArrow arrow"
            :class="{
              inactive: getTeamState('ENEMY') == ''
            }"
          >
            <div class="timer" v-if="getTeamState('ENEMY') != ''">
              {{ stateTime }}
              <i class="fal fa-clock"></i>
            </div>
          </div>
        </div>
        <div class="champions">
          <div class="head">
            <div class="tabs">
              <button
                :class="{ active: selectedTab == 'CHAMPIONS' }"
                @click="selectedTab = 'CHAMPIONS'"
              >
                {{ t("CS_TAB_CHAMPIONS") }}
              </button>
              <button
                :class="{ active: selectedTab == 'SKINS' }"
                :disabled="!currentPlayer.lockedIn"
                @click="selectedTab = 'SKINS'"
              >
                {{ t("CS_TAB_SKINS") }}
              </button>

              <input
                type="text"
                class="actionInput"
                ref="actionInput"
                :placeholder="t('CS_SEARCH_CHAMPION')"
                @keyup="typeAction"
              />
            </div>
            <div class="spacer"></div>
          </div>
          <div class="list" v-if="selectedTab == 'CHAMPIONS'">
            <Champion
              v-for="(champion, index) in filteredChampions"
              :key="index"
              :champion="champion"
            />
          </div>
          <div class="list skins" v-else>
            <button @click="previousSkin" class="arrow arrowPrevious">
              <i class="fas fa-chevron-left"></i>
            </button>
            <button @click="nextSkin" class="arrow arrowNext">
              <i class="fas fa-chevron-right"></i>
            </button>
            <div id="carousel" :style="getCarouselStyle()">
              <Skin
                :style="`--offset: ${index + 1};`"
                class="item"
                :class="{
                  toRight: index + 1 > skinCarouselPosition,
                  toLeft: index + 1 < skinCarouselPosition
                }"
                v-for="(skin, index) in skins"
                :key="index"
                :skin="skin"
              />
            </div>
          </div>
          <div class="footer"></div>
        </div>
        <div class="actions">
          <div class="rmselect">
            <div class="title">
              {{ t("CS_LABEL_RUNES_MASTERIES") }}
            </div>
            <button disabled class="editBtn disabled">
              <i class="fas fa-pencil"></i>
            </button>
            <div class="selects">
              <CustomSelect
                :options="['Runepage #1', 'Runepage #2', 'Runepage #3']"
                :default="'Runepage #1'"
                class="select"
                @input="selectedRunePage"
              />
              <CustomSelect
                :options="[
                  'Masterypage #1',
                  'Masterypage #2',
                  'Masterypage #3'
                ]"
                :default="'Masterypage #1'"
                class="select"
                style="z-index: -1"
                @input="selectedMasteryPage"
              />
            </div>
          </div>
          <div class="spellselect">
            <div
              v-bind:class="{
                aram: this.champselect.lobbyType == 'ARAM_BLIND'
              }"
              class="spellmodal"
              ref="spellmodal"
            >
              <div
                class="spell"
                v-for="(spell, index) in filteredSpells"
                :key="index"
                @click="selectSpell(spell)"
              >
                <img :src="getSpell(false, spell)" :alt="spell" />
              </div>
            </div>
            <div class="title">
              {{ t("CS_LABEL_SPELLS") }}
            </div>
            <div class="spells">
              <div class="spell" @click="showSpellModal(false, 1)">
                <img :src="getSpell(0)" alt="SummonerSpell1" />
              </div>
              <div class="spell" @click="showSpellModal(false, 2)">
                <img :src="getSpell(1)" alt="SummonerSpell2" />
              </div>
            </div>
            <button @click="showSpellModal(true)">
              {{ t("CS_SPELLS_SELECTBOTH_BTN") }}
            </button>
          </div>
          <div class="lockin">
            <button
              v-if="currentPlayer.canLockIn && !currentPlayer.lockedIn"
              @mouseover="$sound.template('LOCKIN_HOVER')"
              @click="lockedInChampion"
            >
              {{ t("CS_ACTION_LOCK_IN") }}
            </button>
            <button
              class="banbtn"
              @mouseover="$sound.template('LOCKIN_HOVER')"
              @click="lockedInBan"
              v-else-if="currentPlayer.canBan"
            >
              {{ t("CS_ACTION_BAN") }}
            </button>
            <button disabled v-else>
              {{ t("CS_ACTION_LOCK_IN") }}
            </button>
          </div>
        </div>
      </div>
      <div class="foot">
        <div class="lolblock">
          <div class="head">
            {{ t("CS_TEAMCHAT_TITLE") }}
          </div>
          <div class="inner">
            <div class="messages" ref="chatMessages">
              <div
                class="message"
                v-for="(message, index) in chatMessages"
                :key="index"
              >
                <span class="sender">{{ message.from }}:</span>
                <span class="text">{{ message.data }}</span>
              </div>
            </div>
            <div class="input">
              <input
                type="text"
                v-on:keyup.enter="sendChatMessage"
                ref="chatInput"
                :placeholder="t('CS_TEAMCHAT_PLACEHOLDER')"
              />
              <button @click="sendChatMessage">
                {{ t("CS_TEAMCHAT_SEND_BTN") }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="side enemy">
      <div class="head">
        {{ t("CS_ENEMY_TEAM_TITLE") }}
      </div>
      <div class="control">
        <EnemyPlayer
          v-for="(player, index) in enemyTeam"
          :key="index"
          :player="player"
          :index="index"
        />
      </div>
      <div class="foot">
        <div class="lolblock">
          <div class="head">
            {{ t("CS_BANNED_CHAMPS_TITLE") }}
          </div>
          <div class="inner">
            <ChampionBan
              v-for="(ban, index) in enemyBans"
              :key="index"
              :ban="ban"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapState } from "vuex";
import CustomSelect from "../components/CustomSelect.vue";
import AllyPlayer from "../components/ChampSelect/AllyPlayer.vue";
import EnemyPlayer from "../components/ChampSelect/EnemyPlayer.vue";
import Champion from "../components/ChampSelect/Champion.vue";
import Skin from "../components/ChampSelect/Skin.vue";
import ChampionBan from "../components/ChampSelect/ChampionBan.vue";

export default {
  components: {
    AllyPlayer,
    EnemyPlayer,
    Champion,
    Skin,
    ChampionBan,
    CustomSelect
  },
  mounted() {
    this.$sound.template("CHAMPSELECT_INTRO");
    setTimeout(() => {
      if (this.champselect.lobbyType === "SUMMONERS_RIFT_DRAFT") {
        this.audioId = this.$sound.template("CHAMPSELECT_MUSIC_DRAFT");
      } else {
        this.audioId = this.$sound.template("CHAMPSELECT_MUSIC_BLIND");
      }
    }, 1750);
  },
  data() {
    return {
      skins: [],
      skinCarouselPosition: 1,
      stateTimer: null,
      stateTime: 0,
      selectedTab: "CHAMPIONS",
      selectableSpells: 1,
      selectedSpells: 0,
      selectingSpell: null,
      lastState: "",
      wasTrading: false,
      lastMessages: null,
      audioId: null
    };
  },
  created() {
    this.$store.dispatch("getCollectionChampions").then(champions => {
      for (let champion in champions) {
        new Audio(
          this.$sound.getPath({
            type: "SOUND",
            state: "CHAMPSELECT_GENERAL",
            fileName: "champions/" + champions[champion].id + ".mp3"
          })
        );
      }
    });
  },
  methods: {
    getCarouselStyle() {
      return `--position:${this.skinCarouselPosition};`;
    },
    nextSkin() {
      if (this.skinCarouselPosition != this.skins.length) {
        this.skinCarouselPosition += 1;
        this.selectSkin(this.skins[this.skinCarouselPosition - 1]);
      }
    },
    previousSkin() {
      if (this.skinCarouselPosition != 1) {
        this.skinCarouselPosition -= 1;
        this.selectSkin(this.skins[this.skinCarouselPosition - 1]);
      }
    },
    selectSkin(skin) {
      if (!skin.owned) return;

      this.$socket.sendChampSelectMessage(
        "CHAMPSELECT_SELECT_SKIN",
        { data: skin.name },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          this.$sound.template("CHAMPION_GRID_CLICK");
          console.log(
            "SELECTED SKIN " + skin.name + " FROM CHAMPION " + skin.championId
          );
        }
      );
    },
    showSpellModal(both, spellid = null) {
      if (both) {
        this.selectableSpells = 2;
      } else {
        this.selectableSpells = 1;
        this.selectingSpell = spellid;
      }
      this.selectedSpells = 0;

      if (!this.$refs.spellmodal.classList.contains("visible")) {
        this.$refs.spellmodal.classList.add("visible");
      } else {
        this.$refs.spellmodal.classList.remove("visible");
      }
    },
    selectSpell(spell) {
      if (this.selectableSpells == 2) {
        if (this.selectedSpells === 0) {
          // Selecting first spell, always spellid 1
          this.$socket.sendChampSelectMessage(
            "CHAMPSELECT_SELECT_SPELL_1",
            { data: this.convertSpell(spell) },
            (response, error) => {
              if (error) {
                console.log("Flyback error:");
                console.log(error);
              }
              this.selectedSpells = 1;
              console.log("Changed spell 1 to " + spell);
            }
          );
        } else {
          // Selecting second spell should close modal, always spellid 2
          this.$socket.sendChampSelectMessage(
            "CHAMPSELECT_SELECT_SPELL_2",
            { data: this.convertSpell(spell) },
            (response, error) => {
              if (error) {
                console.log("Flyback error:");
                console.log(error);
              }
              this.selectedSpells = 2;
              console.log("Changed spell 2 to" + spell);
              this.$refs.spellmodal.classList.remove("visible");
            }
          );
        }
      } else {
        // Selected spell should close now, spellid is selectingspell
        this.$refs.spellmodal.classList.remove("visible");
        this.$socket.sendChampSelectMessage(
          `CHAMPSELECT_SELECT_SPELL_${this.selectingSpell}`,
          { data: this.convertSpell(spell) },
          (response, error) => {
            if (error) {
              console.log("Flyback error:");
              console.log(error);
            }
            console.log(
              "Changed spell " + this.selectingSpell + " to " + spell
            );
            this.$refs.spellmodal.classList.remove("visible");
          }
        );
      }
    },
    convertSpell(spell, fromenum = false) {
      if (fromenum) {
        spell = spell.toLowerCase();
        const parts = spell.split("_");
        parts.forEach((part, index) => {
          parts[index] = part.charAt(0).toUpperCase() + part.slice(1);
        });
        return parts.join("");
      } else {
        const parts = spell.match(/[A-z][a-z]+/g);
        parts.forEach((part, index) => {
          parts[index] = part.toUpperCase();
        });
        return parts.join("_");
      }
    },
    selectedRunePage() {
      console.log("RUNEPAGE SELECTED");
    },
    selectedMasteryPage() {
      console.log("RUNEPAGE SELECTED");
    },
    lockedInChampion() {
      if (this.currentPlayer && !this.currentPlayer.selectedChampion) return;
      this.$socket.sendChampSelectMessage(
        "CHAMPSELECT_LOCK_CHAMPION",
        { data: this.currentPlayer.selectedChampion.id },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          this.$sound.template("LOCKIN_CLICK");
          console.log(
            "LOCKED IN CHAMPION " + this.currentPlayer.selectedChampion.id
          );

          this.skins = this.getSkins(this.currentPlayer.selectedChampion.id);
          this.selectedTab = "SKINS";

          this.$sound.play({
            type: "SOUND",
            state: "CHAMPSELECT_GENERAL",
            fileName:
              "champions/" + this.currentPlayer.selectedChampion.id + ".mp3"
          });
        }
      );
    },
    lockedInBan() {
      if (this.currentPlayer && !this.currentPlayer.selectedChampion) return;
      const bannedChampId = this.currentPlayer.selectedChampion.id;

      this.$socket.sendChampSelectMessage(
        "CHAMPSELECT_BAN_LOCK_CHAMPION",
        { data: this.currentPlayer.selectedChampion.id },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          this.$sound.template("LOCKIN_CLICK");
          console.log("BANNED CHAMPION " + bannedChampId);
        }
      );
    },
    getSpell(spellid, spell = null) {
      const { host, port } = this.$store.state.config.download;

      if (spellid === false) {
        return `${host}:${port}/summoner_spells/${spell}.jpg`;
      }

      let spells = ["SummonerHeal", "SummonerFlash"];
      if (this.currentPlayer.spell1) {
        spells = [
          this.convertSpell(this.currentPlayer.spell1, true),
          this.convertSpell(this.currentPlayer.spell2, true)
        ];
      }

      return `${host}:${port}/summoner_spells/${spells[spellid]}.jpg`;
    },
    getSkins(championId) {
      let tempSkinsOwned = this.champions.find(c => c.id === championId).skins
        .owned;
      let tempSkinsNotOwned = this.champions.find(c => c.id === championId)
        .skins.notOwned;

      tempSkinsOwned.forEach(e => {
        e.championId = championId;
        e.owned = true;
      });

      tempSkinsNotOwned.forEach(e => {
        e.championId = championId;
        e.owned = false;
      });

      return tempSkinsOwned.concat(tempSkinsNotOwned);
    },
    sendChatMessage() {
      const input = this.$refs.chatInput;
      const message = input.value;
      if (message.trim() == "") return;

      input.value = "";

      this.$socket.sendChampSelectMessage(
        "CHAMPSELECT_CHAT",
        { data: message },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }
          this.$refs.chatMessages.scrollTop = this.$refs.chatMessages.scrollHeight;
        }
      );
    },
    getTeamState(name) {
      if (!this.currentPlayer) return "UNDEFINED";

      const allyteam = this.currentPlayer.team;

      if (name == "ALLY") {
        switch (this.globalState.phase) {
          case "PICK":
            if (
              this.globalState.team == "BLIND" &&
              !this.didFullyLock("ALLY")
            ) {
              return "PICKING";
            } else if (
              this.globalState.team != "BLIND" &&
              this.globalState.team == allyteam
            ) {
              return "PICKING";
            } else {
              return "";
            }

          case "BAN":
            if (this.globalState.team == allyteam) {
              return "BANNING";
            } else {
              return "";
            }

          default:
            return "";
        }
      } else {
        switch (this.globalState.phase) {
          case "PICK":
            if (
              this.globalState.team == "BLIND" &&
              !this.didFullyLock("ENEMY")
            ) {
              return "PICKING";
            } else if (
              this.globalState.team != "BLIND" &&
              this.globalState.team != allyteam
            ) {
              return "PICKING";
            } else {
              return "";
            }

          case "BAN":
            if (this.globalState.team != allyteam) {
              return "BANNING";
            } else {
              return "";
            }

          default:
            return "";
        }
      }
    },
    didFullyLock(name) {
      if (name == "ALLY") {
        return (
          this.champselect.team.filter(player => {
            return !player.lockedIn;
          }).length == 0
        );
      } else {
        return (
          this.enemyTeam.filter(player => {
            return !player.lockedIn;
          }).length == 0
        );
      }
    },
    getGlobalStateMessage() {
      if (this.globalState.team == "BLIND") {
        switch (this.globalState.phase) {
          case "PICK":
            if (this.currentPlayer.canLockIn) {
              return this.$translate.text("CS_GLOBAL_STATE_PICK_SELF");
            } else if (!this.didFullyLock("ALLY")) {
              return this.$translate.text("CS_GLOBAL_STATE_PICK_ALLY");
            } else {
              return this.$translate.text("CS_GLOBAL_STATE_PICK_ENEMY");
            }

          case "PRE":
            return this.$translate.textWithParams(
              "CS_GLOBAL_STATE_GAME_START",
              {
                stateTime: this.stateTime
              }
            );

          default:
            return this.$translate.textWithParams(
              "CS_GLOBAL_STATE_GAME_START",
              {
                stateTime: this.stateTime
              }
            );
        }
      } else {
        switch (this.globalState.phase) {
          case "PICK":
            if (this.currentPlayer.canLockIn) {
              return this.$translate.text("CS_GLOBAL_STATE_PICK_SELF");
            } else if (this.currentPlayer.team == this.globalState.team) {
              return this.$translate.text("CS_GLOBAL_STATE_PICK_ALLY");
            } else {
              return this.$translate.text("CS_GLOBAL_STATE_PICK_ENEMY");
            }

          case "BAN":
            if (this.currentPlayer.canBan) {
              return this.$translate.text("CS_GLOBAL_STATE_BAN_SELF");
            } else if (this.currentPlayer.team == this.globalState.team) {
              return this.$translate.text("CS_GLOBAL_STATE_BAN_ALLY");
            } else {
              return this.$translate.text("CS_GLOBAL_STATE_BAN_ENEMY");
            }

          case "PRE":
            return this.$translate.textWithParams(
              "CS_GLOBAL_STATE_GAME_START",
              {
                stateTime: this.stateTime
              }
            );

          default:
            return this.$translate.textWithParams(
              "CS_GLOBAL_STATE_GAME_START",
              {
                stateTime: this.stateTime
              }
            );
        }
      }
    },
    typeAction() {
      this.$store.dispatch(
        "searchFilterChampions",
        this.$refs.actionInput.value
      );
    }
  },
  computed: mapState({
    champions: state => state.collection.champions,
    chatMessages: state => state.champselectMessages,
    champselect: state => state.champselect,
    enemyTeam: state => state.csEnemyTeam,
    allyBans: state => state.csAllyBans,
    enemyBans: state => state.csEnemyBans,
    globalState: state => state.csGlobalState,
    currentPlayer: state => state.csCurrentPlayer,
    isTradingTarget: state => {
      if (state.champselect == {}) return false;

      const myTrades = state.champselect.tradesTeam.filter(trade => {
        return (
          trade.target.user.summonerName ==
          state.csCurrentPlayer.user.summonerName
        );
      });

      if (myTrades.length == 0) {
        return false;
      } else {
        return true;
      }
    },
    isTrading: state => {
      if (state.champselect == {}) return false;

      const myTrades = state.champselect.tradesTeam.filter(trade => {
        return (
          trade.initiator.user.summonerName ==
            state.csCurrentPlayer.user.summonerName ||
          trade.target.user.summonerName ==
            state.csCurrentPlayer.user.summonerName
        );
      });

      if (myTrades.length == 0) {
        return false;
      } else {
        return true;
      }
    },
    filteredChampions: state => {
      return state.collection.champions.filter(c => c.shouldShow);
    },
    filteredSpells: state => {
      return state.availableSpells.filter(c => {
        if (state.champselect.lobbyType == "ARAM_BLIND") {
          if (
            c === "SummonerOdinGarrison" ||
            c === "SummonerClairvoyance" ||
            c === "SummonerTeleport" ||
            c === "SummonerSmite"
          ) {
            return false;
          }
        } else {
          if (c === "SummonerOdinGarrison") {
            return false;
          }
        }
        return true;
      });
    }
  }),
  updated() {
    if (this.wasTrading && !this.isTrading) {
      this.wasTrading = false;
      if (this.skins[0].championId != this.currentPlayer.selectedChampion.id) {
        this.skins = this.getSkins(this.currentPlayer.selectedChampion.id);
        this.skinCarouselPosition = 1;
      }
    } else if (!this.wasTrading && this.isTradingTarget) {
      this.$sound.template("CHAMPSELECT_TRADE");
      this.wasTrading = true;
    } else if (!this.wasTrading && this.isTrading) {
      this.wasTrading = true;
    }

    if (this.champselect.lobbyType == "ARAM_BLIND" && !this.skins.length) {
      this.skins = this.getSkins(this.currentPlayer.selectedChampion.id);
      this.skinCarouselPosition = 1;
      this.selectedTab = "SKINS";
    }

    if (this.lastMessages !== this.chatMessages.length) {
      this.lastMessages = this.chatMessages.length;
      this.$refs.chatMessages.scrollTop = this.$refs.chatMessages.scrollHeight;
    }

    if (this.lastState == "") {
      this.lastState = this.globalState ?? "NOT DEFAULT";
      this.stateTime = this.globalState.time;

      if (this.currentPlayer.canLockIn && !this.currentPlayer.lockedIn) {
        this.$sound.template("CHAMPSELECT_YOURTURN");
      }

      this.stateTimer = setInterval(() => {
        if (this.stateTime <= 0) {
          clearInterval(this.stateTimer);
        } else {
          this.stateTime--;
        }
      }, 1000);
    } else if (
      this.lastState.team != this.globalState.team ||
      this.lastState.time != this.globalState.time ||
      (this.lastState.phase == this.globalState.phase &&
        this.champselect.enemyTeam.length == 0 &&
        (this.lastState.bannedChampionsLength !=
          this.globalState.bannedChampionsLength ||
          this.lastState.pickedChampionsLength !=
            this.globalState.pickedChampionsLength)) // Kill me :) -> We're depending on different time's for the phases here.
    ) {
      clearInterval(this.stateTimer);

      if (this.lastState.phase != this.globalState.phase) {
        this.$sound.template("CHAMPSELECT_PHASECHANGE");
      }

      if (this.currentPlayer.canLockIn && !this.currentPlayer.lockedIn) {
        this.$sound.template("CHAMPSELECT_YOURTURN");
      }

      this.lastState = this.globalState;
      this.stateTime = this.globalState.time;

      this.stateTimer = setInterval(() => {
        if (this.stateTime == 1) {
          this.$sound.template("CHAMPSELECT_EXIT");
        } else if (this.stateTime == 11) {
          this.$sound.template("CHAMPSELECT_COUNTDOWN");
        }

        if (this.stateTime <= 0) {
          clearInterval(this.stateTimer);
        } else {
          this.stateTime--;
        }
      }, 1000);
    }
  },
  beforeMount() {
    this.$store.dispatch("changeBackgroundState", "CHAMPSELECT");
  },
  beforeDestroy() {
    this.$sound.stopAll("CHAMPSELECT");
    clearInterval(this.stateTimer);
  }
};
</script>

<style lang="css" scoped>
@import "../assets/css/champselect.css";
</style>
