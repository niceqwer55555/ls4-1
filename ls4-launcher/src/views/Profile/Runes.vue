<template>
  <div id="RuneMasteryPage">
    <div class="runeMasteryContainer">
      <div class="pageHeader">
        <h2>{{ t("RUNE_PAGE_TITLE") }}</h2>
        <p>{{ t("RUNE_PAGE_DESC") }}</p>
      </div>
      
      <div class="runeMasteryContent">
        <div class="runePagesList">
          <div class="listHeader">
            {{ t("RUNE_PAGE_LIST_TITLE") }}
          </div>
          <div class="pageList">
            <div
              v-for="(page, index) in runePages"
              :key="index"
              :class="{ selected: currentRunePage === index }"
              @click="selectRunePage(index)"
              class="pageItem"
            >
              <span class="pageName">{{ page.name || `Rune Page ${index + 1}` }}</span>
              <span v-if="index === currentRunePage" class="currentIndicator">✓</span>
            </div>
          </div>
          <div class="pageActions">
            <button @click="addRunePage" class="actionBtn addBtn">
              + {{ t("RUNE_PAGE_ADD_BTN") }}
            </button>
            <button
              @click="deleteRunePage"
              :disabled="runePages.length <= 1"
              class="actionBtn deleteBtn"
            >
              - {{ t("RUNE_PAGE_DELETE_BTN") }}
            </button>
          </div>
        </div>

        <div class="runeEditor">
          <div class="editorHeader">
            <input
              v-model="currentPageName"
              @blur="saveCurrentPageName"
              class="pageNameInput"
              :placeholder="t('RUNE_PAGE_NAME_PLACEHOLDER')"
            />
          </div>
          <div class="runeSlots">
            <div class="runeRow">
              <div class="rowLabel">Marks</div>
              <div class="slotGroup">
                <div 
                  v-for="i in 9" 
                  :key="i"
                  @click="openRuneSelector(i, 'mark')"
                  :class="{ selected: selectedSlot === i }"
                  class="runeSlot markSlot"
                >
                  <img :src="getRuneImage(getRuneAtSlot(i))" :alt="getRuneName(getRuneAtSlot(i))" />
                  <span class="slotNumber">{{ i }}</span>
                </div>
              </div>
            </div>
            <div class="runeRow">
              <div class="rowLabel">Seals</div>
              <div class="slotGroup">
                <div 
                  v-for="i in 9" 
                  :key="'seal-' + i"
                  @click="openRuneSelector(i + 9, 'seal')"
                  :class="{ selected: selectedSlot === (i + 9) }"
                  class="runeSlot sealSlot"
                >
                  <img :src="getRuneImage(getRuneAtSlot(i + 9))" :alt="getRuneName(getRuneAtSlot(i + 9))" />
                  <span class="slotNumber">{{ i + 9 }}</span>
                </div>
              </div>
            </div>
            <div class="runeRow">
              <div class="rowLabel">Glyphs</div>
              <div class="slotGroup">
                <div 
                  v-for="i in 9" 
                  :key="'glyph-' + i"
                  @click="openRuneSelector(i + 18, 'glyph')"
                  :class="{ selected: selectedSlot === (i + 18) }"
                  class="runeSlot glyphSlot"
                >
                  <img :src="getRuneImage(getRuneAtSlot(i + 18))" :alt="getRuneName(getRuneAtSlot(i + 18))" />
                  <span class="slotNumber">{{ i + 18 }}</span>
                </div>
              </div>
            </div>
            <div class="runeRow">
              <div class="rowLabel">Quintessences</div>
              <div class="slotGroup">
                <div 
                  v-for="i in 3" 
                  :key="'quint-' + i"
                  @click="openRuneSelector(i + 27, 'quint')"
                  :class="{ selected: selectedSlot === (i + 27) }"
                  class="runeSlot quintSlot"
                >
                  <img :src="getRuneImage(getRuneAtSlot(i + 27))" :alt="getRuneName(getRuneAtSlot(i + 27))" />
                  <span class="slotNumber">{{ i + 27 }}</span>
                </div>
              </div>
            </div>
          </div>
          <div class="runeEffects">
            <div class="effectsHeader">{{ t("RUNE_PAGE_EFFECTS_TITLE") }}</div>
            <div class="effectsList">
              <div v-for="(effect, key) in runeEffects" :key="key" class="effectItem">
                <span class="effectName">{{ key }}</span>
                <span class="effectValue">{{ effect }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showRuneSelector" class="runeSelectorModal" @click.self="closeRuneSelector">
      <div class="modalContent">
        <div class="modalHeader">
          <span>{{ t("RM_EDIT_RUNE") }}</span>
          <button @click="closeRuneSelector" class="closeBtn">✕</button>
        </div>
        <div class="runeList">
          <div 
            v-for="rune in availableRunes" 
            :key="rune.id"
            @click="selectRune(rune.id)"
            :class="{ selected: getRuneAtSlot(selectedSlot) === rune.id }"
            class="runeOption"
          >
            <img :src="getRuneImage(rune.id)" :alt="rune.name" />
            <div class="runeInfo">
              <div class="runeName">{{ rune.name }}</div>
              <div class="runeEffect">{{ rune.effect }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapState, mapActions } from "vuex";
import { RUNE_DATA, STAT_NAMES } from "@/utils/runeData";

export default {
  data() {
    return {
      currentPageName: "",
      showRuneSelector: false,
      selectedSlot: null,
      selectedSlotType: null
    };
  },
  computed: {
    ...mapState({
      runePages: state => state.runePages,
      currentRunePage: state => state.currentRunePage
    }),
    availableRunes() {
      if (!this.selectedSlotType) return [];
      const typeMap = { mark: "mark", seal: "seal", glyph: "glyph", quint: "quintessence" };
      const runeType = typeMap[this.selectedSlotType] || this.selectedSlotType;
      return Object.entries(RUNE_DATA)
        .filter(([, r]) => r.type === runeType && r.tier === 3)
        .map(([id, r]) => ({
          id: parseInt(id),
          name: r.name,
          effect: this.formatRuneEffect(r.stats)
        }));
    },
    runeEffects() {
      const currentPage = this.runePages[this.currentRunePage];
      if (!currentPage || !currentPage.runes) return {};

      const aggregated = {};
      Object.keys(currentPage.runes).forEach(slot => {
        const runeId = currentPage.runes[slot];
        const runeInfo = RUNE_DATA[runeId];
        if (runeInfo && runeInfo.stats) {
          Object.keys(runeInfo.stats).forEach(statKey => {
            const statName = STAT_NAMES[statKey] || statKey;
            if (!aggregated[statName]) aggregated[statName] = 0;
            aggregated[statName] += Math.abs(runeInfo.stats[statKey]);
          });
        }
      });

      const result = {};
      Object.keys(aggregated).forEach(key => {
        const val = aggregated[key];
        const isPercent = key.includes("%") || key.includes("几率") || key.includes("缩减") || key.includes("穿透");
        result[key] = isPercent ? `+${(val * 100).toFixed(2)}%` : `+${val.toFixed(2)}`;
      });
      return result;
    }
  },
  methods: {
    ...mapActions(["selectRunePage", "addRunePage", "deleteRunePage"]),
    getRuneAtSlot(slot) {
      const currentPage = this.runePages[this.currentRunePage];
      if (!currentPage || !currentPage.runes) return null;
      return currentPage.runes[slot] || null;
    },
    getRuneImage(runeId) {
      if (!runeId) return "";
      const runeInfo = RUNE_DATA[runeId];
      if (runeInfo && runeInfo.icon) {
        const { host, port } = this.$store.state.config.download;
        return `${host}:${port}/rune/${runeInfo.icon}.png`;
      }
      return "";
    },
    getRuneName(runeId) {
      if (!runeId) return "Empty";
      const runeInfo = RUNE_DATA[runeId];
      return runeInfo ? runeInfo.name : "未知符文";
    },
    formatRuneEffect(stats) {
      if (!stats) return "";
      return Object.entries(stats)
        .map(([key, val]) => {
          const name = STAT_NAMES[key] || key;
          const isPercent = key.includes("percent") || key.includes("chance") || key.includes("reduction") || key.includes("penetration") || (key.includes("speed") && key.includes("percent")) || key.includes("vamp") || key.includes("steal");
          const absVal = Math.abs(val);
          return `${name} +${isPercent ? (absVal * 100).toFixed(2) + "%" : absVal.toFixed(2)}`;
        })
        .join(", ");
    },
    saveCurrentPageName() {
      if (this.currentPageName.trim()) {
        const updatedPages = [...this.runePages];
        updatedPages[this.currentRunePage] = {
          ...updatedPages[this.currentRunePage],
          name: this.currentPageName.trim()
        };
        this.$store.commit("setRunePages", updatedPages);
        this.$store.dispatch("saveRuneMasteryPages");
      }
    },
    openRuneSelector(slot, type) {
      this.selectedSlot = slot;
      this.selectedSlotType = type === "quint" ? "quintessence" : type;
      this.showRuneSelector = true;
    },
    closeRuneSelector() {
      this.showRuneSelector = false;
      this.selectedSlot = null;
      this.selectedSlotType = null;
    },
    selectRune(runeId) {
      const updatedPages = [...this.runePages];
      updatedPages[this.currentRunePage] = {
        ...updatedPages[this.currentRunePage],
        runes: {
          ...updatedPages[this.currentRunePage].runes,
          [this.selectedSlot]: runeId
        }
      };
      this.$store.commit("setRunePages", updatedPages);
      this.$store.dispatch("saveRuneMasteryPages");
      this.closeRuneSelector();
    }
  },
  mounted() {
    this.$store.dispatch("loadRuneMasteryPages");
    const currentPage = this.runePages[this.currentRunePage];
    if (currentPage) {
      this.currentPageName = currentPage.name || "";
    }
  },
  beforeMount() {
    this.$store.dispatch("changeBackgroundState", "PROFILE");
  }
}
</script>

<style lang="css" scoped>
#RuneMasteryPage {
  width: 100%;
  height: 100%;
  position: relative;
  display: flex;
  overflow: hidden;
}

.runeMasteryContainer {
  width: 100%;
  padding: 6px 8px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  box-sizing: border-box;
}

.pageHeader {
  margin-bottom: 8px;
  flex-shrink: 0;
}

.pageHeader h2 {
  font-family: LoLFont2;
  font-size: clamp(18px, 2.5vw, 26px);
  color: #f2d030;
  margin: 0;
  -webkit-text-stroke: 1px black;
}

.pageHeader p {
  font-family: LoLFont2;
  font-size: clamp(10px, 1.2vw, 13px);
  color: rgba(255, 255, 255, 0.7);
  margin: 3px 0 0 0;
}

.runeMasteryContent {
  display: flex;
  gap: 8px;
  flex: 1;
  min-height: 0;
  overflow: hidden;
}

.runePagesList {
  width: clamp(120px, 14vw, 180px);
  min-width: 110px;
  background-color: rgba(5, 12, 20, 0.9);
  border: 1px solid rgba(100, 117, 137, 0.75);
  border-radius: 5px;
  display: flex;
  flex-direction: column;
  flex-shrink: 0;
}

.listHeader {
  padding: 6px 10px;
  font-family: LoLFont2;
  font-size: 12px;
  background-image: linear-gradient(180deg, #192e49 0%, #192e49 40%, #172b46 50%, #142131 100%);
  border-bottom: 1px solid rgba(100, 117, 137, 0.75);
  flex-shrink: 0;
}

.pageList {
  flex: 1;
  overflow-y: auto;
}

.pageList::-webkit-scrollbar {
  width: 4px;
}

.pageList::-webkit-scrollbar-thumb {
  background: rgba(100, 117, 137, 0.5);
  border-radius: 2px;
}

.pageItem {
  padding: 6px 10px;
  font-family: LoLFont2;
  font-size: 11px;
  cursor: pointer;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid rgba(100, 117, 137, 0.2);
  transition: background-color 0.2s;
}

.pageItem:hover {
  background-color: rgba(13, 71, 49, 0.3);
}

.pageItem.selected {
  background-color: rgba(13, 71, 49, 0.6);
}

.currentIndicator {
  color: #306d32;
}

.pageActions {
  padding: 6px 10px;
  display: flex;
  gap: 6px;
  border-top: 1px solid rgba(100, 117, 137, 0.75);
  flex-shrink: 0;
}

.actionBtn {
  flex: 1;
  padding: 5px 8px;
  font-family: LoLFont2;
  font-size: 10px;
  border: none;
  border-radius: 3px;
  cursor: pointer;
  transition: filter ease-in-out 200ms;
}

.actionBtn:hover:not(:disabled) {
  filter: brightness(1.25);
}

.actionBtn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.addBtn {
  background: linear-gradient(180deg, #2a6b2a 0%, #1a4a1a 45%, #143a14 50%, #081808 100%);
  border: 1px solid #50a850;
  color: #b0ffb0;
}

.deleteBtn {
  background: linear-gradient(180deg, #8b2a2a 0%, #5a1a1a 45%, #4a1414 50%, #200808 100%);
  border: 1px solid #a85050;
  color: #ffb0b0;
}

.runeEditor {
  flex: 1;
  background-color: rgba(5, 12, 20, 0.9);
  border: 1px solid rgba(100, 117, 137, 0.75);
  border-radius: 5px;
  display: flex;
  flex-direction: column;
  min-width: 0;
  overflow: hidden;
}

.editorHeader {
  padding: 8px 12px;
  background-image: linear-gradient(180deg, #192e49 0%, #192e49 40%, #172b46 50%, #142131 100%);
  border-bottom: 1px solid rgba(100, 117, 137, 0.75);
  flex-shrink: 0;
}

.pageNameInput {
  width: 100%;
  padding: 5px 8px;
  background-color: #0a1320;
  border: 1px solid #304b69;
  border-radius: 3px;
  color: white;
  font-family: LoLFont2;
  font-size: 13px;
  outline: none;
  box-sizing: border-box;
}

.pageNameInput:focus {
  border-color: #f2d030;
}

.runeSlots {
  flex: 1;
  padding: 10px 14px;
  overflow-y: auto;
  overflow-x: hidden;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.runeSlots::-webkit-scrollbar {
  width: 4px;
}

.runeSlots::-webkit-scrollbar-thumb {
  background: rgba(100, 117, 137, 0.5);
  border-radius: 2px;
}

.runeRow {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: nowrap;
}

.rowLabel {
  width: clamp(60px, 8vw, 90px);
  min-width: 55px;
  font-family: LoLFont2;
  font-size: clamp(10px, 1.1vw, 12px);
  color: rgba(255, 255, 255, 0.7);
  text-align: right;
  flex-shrink: 0;
}

.slotGroup {
  display: flex;
  gap: 4px;
  flex-wrap: wrap;
}

.runeSlot {
  width: clamp(32px, 4.5vw, 44px);
  height: clamp(32px, 4.5vw, 44px);
  border-radius: 50%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  cursor: pointer;
  transition: all ease-in-out 200ms;
  border: 2px solid transparent;
  flex-shrink: 0;
}

.runeSlot:hover {
  transform: scale(1.1);
  border-color: rgba(255, 255, 255, 0.5);
}

.runeSlot.selected {
  border-color: #f2d030;
  box-shadow: 0 0 10px rgba(242, 208, 48, 0.5);
}

.runeSlot img {
  width: 80%;
  height: 80%;
  object-fit: contain;
}

.markSlot {
  border-color: rgba(200, 70, 48, 0.5);
}

.sealSlot {
  border-color: rgba(60, 115, 180, 0.5);
}

.glyphSlot {
  border-color: rgba(242, 208, 48, 0.5);
}

.quintSlot {
  width: clamp(38px, 5vw, 52px);
  height: clamp(38px, 5vw, 52px);
  border-color: rgba(139, 92, 246, 0.5);
}

.slotNumber {
  position: absolute;
  bottom: -2px;
  right: -2px;
  width: 12px;
  height: 12px;
  background-color: #0a1320;
  border-radius: 50%;
  font-size: 7px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid rgba(100, 117, 137, 0.75);
  font-family: LoLFont2;
  color: rgba(255, 255, 255, 0.6);
}

.runeEffects {
  padding: 6px 12px;
  border-top: 1px solid rgba(100, 117, 137, 0.75);
  flex-shrink: 0;
  max-height: 100px;
  overflow-y: auto;
}

.runeEffects::-webkit-scrollbar {
  width: 3px;
}

.runeEffects::-webkit-scrollbar-thumb {
  background: rgba(100, 117, 137, 0.5);
  border-radius: 2px;
}

.effectsHeader {
  font-family: LoLFont2;
  font-size: 11px;
  margin-bottom: 4px;
  color: rgba(255, 255, 255, 0.7);
}

.effectsList {
  display: flex;
  flex-wrap: wrap;
  gap: 6px 14px;
}

.effectItem {
  display: flex;
  gap: 6px;
  font-family: LoLFont2;
  font-size: 10px;
}

.effectName {
  color: rgba(255, 255, 255, 0.7);
}

.effectValue {
  color: #f2d030;
}

.runeSelectorModal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modalContent {
  background-color: rgba(5, 12, 20, 0.95);
  border: 1px solid rgba(100, 117, 137, 0.75);
  border-radius: 5px;
  width: clamp(320px, 50vw, 480px);
  max-height: 70vh;
  display: flex;
  flex-direction: column;
}

.modalHeader {
  padding: 10px 14px;
  background-image: linear-gradient(180deg, #192e49 0%, #192e49 40%, #172b46 50%, #142131 100%);
  border-bottom: 1px solid rgba(100, 117, 137, 0.75);
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-family: LoLFont2;
  font-size: 14px;
  flex-shrink: 0;
}

.closeBtn {
  background: none;
  border: none;
  color: rgba(255, 255, 255, 0.7);
  font-size: 16px;
  cursor: pointer;
  padding: 0 5px;
}

.closeBtn:hover {
  color: white;
}

.runeList {
  flex: 1;
  overflow-y: auto;
  padding: 6px 8px;
}

.runeList::-webkit-scrollbar {
  width: 4px;
}

.runeList::-webkit-scrollbar-thumb {
  background: rgba(100, 117, 137, 0.5);
  border-radius: 2px;
}

.runeOption {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 6px 8px;
  cursor: pointer;
  border-radius: 3px;
  transition: all ease-in-out 200ms;
}

.runeOption:hover {
  background-color: rgba(13, 71, 49, 0.3);
}

.runeOption.selected {
  background-color: rgba(13, 71, 49, 0.6);
  border: 1px solid #306d32;
}

.runeOption img {
  width: 28px;
  height: 28px;
  object-fit: contain;
  flex-shrink: 0;
}

.runeInfo {
  flex: 1;
  min-width: 0;
}

.runeName {
  font-family: LoLFont2;
  font-size: 12px;
  color: white;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.runeEffect {
  font-family: LoLFont2;
  font-size: 10px;
  color: #f2d030;
}

/* Responsive: narrow screens */
@media (max-width: 900px) {
  .runeMasteryContent {
    flex-direction: column;
  }
  .runePagesList {
    width: 100%;
    min-width: 0;
    max-height: 100px;
  }
  .pageList {
    display: flex;
    overflow-x: auto;
    overflow-y: hidden;
    gap: 4px;
    padding: 4px;
  }
  .pageItem {
    flex-shrink: 0;
    border-bottom: none;
    border-right: 1px solid rgba(100, 117, 137, 0.2);
  }
  .runeRow {
    flex-wrap: wrap;
  }
  .slotGroup {
    flex-wrap: wrap;
  }
}

@media (max-height: 700px) {
  .runeMasteryContainer {
    padding: 6px 10px;
  }
  .pageHeader {
    margin-bottom: 4px;
  }
  .pageHeader h2 {
    font-size: 16px;
  }
  .runeSlots {
    padding: 6px 10px;
    gap: 4px;
  }
  .runeEffects {
    max-height: 60px;
    padding: 4px 8px;
  }
}
</style>
