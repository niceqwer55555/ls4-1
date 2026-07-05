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

export default {
  data() {
    return {
      currentPageName: "",
      showRuneSelector: false,
      selectedSlot: null,
      selectedSlotType: null,
      runeData: {
        mark: [
          { id: 5245, name: "Mark of Attack Damage", effect: "+0.95 AD" },
          { id: 5246, name: "Greater Mark of Attack Damage", effect: "+1.89 AD" },
          { id: 5247, name: "Lesser Mark of Attack Damage", effect: "+2.84 AD" },
          { id: 5273, name: "Mark of Attack Speed", effect: "+1.08% AS" },
          { id: 5274, name: "Greater Mark of Attack Speed", effect: "+2.16% AS" },
          { id: 5275, name: "Lesser Mark of Attack Speed", effect: "+3.23% AS" },
          { id: 5320, name: "Mark of Critical Strike", effect: "+1.86% Crit" },
          { id: 5321, name: "Greater Mark of Critical Strike", effect: "+3.73% Crit" },
          { id: 5322, name: "Lesser Mark of Critical Strike", effect: "+5.59% Crit" },
          { id: 5265, name: "Mark of Gold per 10", effect: "+0.35 Gp10" },
          { id: 5266, name: "Greater Mark of Gold per 10", effect: "+0.7 Gp10" },
          { id: 5267, name: "Lesser Mark of Gold per 10", effect: "+1.05 Gp10" },
          { id: 5311, name: "Mark of Spell Vamp", effect: "+0.5% Spell Vamp" },
          { id: 5312, name: "Greater Mark of Spell Vamp", effect: "+1.0% Spell Vamp" },
          { id: 5313, name: "Lesser Mark of Spell Vamp", effect: "+1.5% Spell Vamp" },
          { id: 5302, name: "Mark of Cooldown Reduction", effect: "+0.83% CDR" },
          { id: 5303, name: "Greater Mark of Cooldown Reduction", effect: "+1.66% CDR" },
          { id: 5304, name: "Lesser Mark of Cooldown Reduction", effect: "+2.5% CDR" },
          { id: 5286, name: "Mark of Mana Regeneration", effect: "+0.26 MP5" },
          { id: 5287, name: "Greater Mark of Mana Regeneration", effect: "+0.51 MP5" },
          { id: 5288, name: "Lesser Mark of Mana Regeneration", effect: "+0.77 MP5" },
        ],
        seal: [
          { id: 5289, name: "Seal of Armor", effect: "+1.41 Armor" },
          { id: 5290, name: "Greater Seal of Armor", effect: "+2.81 Armor" },
          { id: 5291, name: "Lesser Seal of Armor", effect: "+4.21 Armor" },
          { id: 5277, name: "Seal of Health", effect: "+5.35 HP" },
          { id: 5278, name: "Greater Seal of Health", effect: "+10.7 HP" },
          { id: 5279, name: "Lesser Seal of Health", effect: "+16.05 HP" },
          { id: 5280, name: "Seal of Health Regeneration", effect: "+0.41 HP5" },
          { id: 5281, name: "Greater Seal of Health Regeneration", effect: "+0.81 HP5" },
          { id: 5282, name: "Lesser Seal of Health Regeneration", effect: "+1.22 HP5" },
          { id: 5296, name: "Seal of Movement Speed", effect: "+0.45% MS" },
          { id: 5297, name: "Greater Seal of Movement Speed", effect: "+0.9% MS" },
          { id: 5298, name: "Lesser Seal of Movement Speed", effect: "+1.35% MS" },
          { id: 5302, name: "Seal of Cooldown Reduction", effect: "+0.83% CDR" },
          { id: 5303, name: "Greater Seal of Cooldown Reduction", effect: "+1.66% CDR" },
          { id: 5304, name: "Lesser Seal of Cooldown Reduction", effect: "+2.5% CDR" },
        ],
        glyph: [
          { id: 5335, name: "Glyph of Magic Resist", effect: "+1.41 MR" },
          { id: 5336, name: "Greater Glyph of Magic Resist", effect: "+2.81 MR" },
          { id: 5337, name: "Lesser Glyph of Magic Resist", effect: "+4.21 MR" },
          { id: 5317, name: "Glyph of Ability Power", effect: "+1.19 AP" },
          { id: 5318, name: "Greater Glyph of Ability Power", effect: "+2.38 AP" },
          { id: 5319, name: "Lesser Glyph of Ability Power", effect: "+3.57 AP" },
          { id: 5286, name: "Glyph of Mana Regeneration", effect: "+0.26 MP5" },
          { id: 5287, name: "Greater Glyph of Mana Regeneration", effect: "+0.51 MP5" },
          { id: 5288, name: "Lesser Glyph of Mana Regeneration", effect: "+0.77 MP5" },
          { id: 5283, name: "Glyph of Mana", effect: "+8.18 Mana" },
          { id: 5284, name: "Greater Glyph of Mana", effect: "+16.36 Mana" },
          { id: 5285, name: "Lesser Glyph of Mana", effect: "+24.54 Mana" },
          { id: 5302, name: "Glyph of Cooldown Reduction", effect: "+0.83% CDR" },
          { id: 5303, name: "Greater Glyph of Cooldown Reduction", effect: "+1.66% CDR" },
          { id: 5304, name: "Lesser Glyph of Cooldown Reduction", effect: "+2.5% CDR" },
        ],
        quint: [
          { id: 5305, name: "Quintessence of Life Steal", effect: "+0.5% Lifesteal" },
          { id: 5306, name: "Greater Quintessence of Life Steal", effect: "+1.0% Lifesteal" },
          { id: 5307, name: "Lesser Quintessence of Life Steal", effect: "+1.5% Lifesteal" },
          { id: 5319, name: "Quintessence of Ability Power", effect: "+3.57 AP" },
          { id: 5247, name: "Quintessence of Attack Damage", effect: "+2.84 AD" },
          { id: 5298, name: "Quintessence of Movement Speed", effect: "+1.35% MS" },
          { id: 5275, name: "Quintessence of Attack Speed", effect: "+3.23% AS" },
          { id: 5279, name: "Quintessence of Health", effect: "+16.05 HP" },
          { id: 5267, name: "Quintessence of Gold per 10", effect: "+1.05 Gp10" },
          { id: 5291, name: "Quintessence of Armor", effect: "+4.21 Armor" },
          { id: 5337, name: "Quintessence of Magic Resist", effect: "+4.21 MR" },
          { id: 5322, name: "Quintessence of Critical Strike", effect: "+5.59% Crit" },
          { id: 5313, name: "Quintessence of Spell Vamp", effect: "+1.5% Spell Vamp" },
          { id: 5304, name: "Quintessence of Cooldown Reduction", effect: "+2.5% CDR" },
        ]
      }
    };
  },
  computed: {
    ...mapState({
      runePages: state => state.runePages,
      currentRunePage: state => state.currentRunePage
    }),
    availableRunes() {
      return this.runeData[this.selectedSlotType] || [];
    },
    runeEffects() {
      const effects = {
        AttackDamage: 0,
        AbilityPower: 0,
        Armor: 0,
        MagicResist: 0,
        AttackSpeed: 0,
        MovementSpeed: 0,
        LifeSteal: 0,
        SpellVamp: 0,
        GoldPer10: 0,
        Health: 0,
        HealthRegen: 0,
        Mana: 0,
        ManaRegen: 0,
        CooldownReduction: 0,
        CriticalStrike: 0
      };

      const currentPage = this.runePages[this.currentRunePage];
      if (!currentPage || !currentPage.runes) return effects;

      const runeData = {
        5245: { type: "AttackDamage", value: 0.95 },
        5246: { type: "AttackDamage", value: 1.89 },
        5247: { type: "AttackDamage", value: 2.84 },
        5317: { type: "AbilityPower", value: 1.19 },
        5318: { type: "AbilityPower", value: 2.38 },
        5319: { type: "AbilityPower", value: 3.57 },
        5289: { type: "Armor", value: 1.41 },
        5290: { type: "Armor", value: 2.81 },
        5291: { type: "Armor", value: 4.21 },
        5335: { type: "MagicResist", value: 1.41 },
        5336: { type: "MagicResist", value: 2.81 },
        5337: { type: "MagicResist", value: 4.21 },
        5273: { type: "AttackSpeed", value: 1.08 },
        5274: { type: "AttackSpeed", value: 2.16 },
        5275: { type: "AttackSpeed", value: 3.23 },
        5296: { type: "MovementSpeed", value: 0.45 },
        5297: { type: "MovementSpeed", value: 0.9 },
        5298: { type: "MovementSpeed", value: 1.35 },
        5305: { type: "LifeSteal", value: 0.5 },
        5306: { type: "LifeSteal", value: 1.0 },
        5307: { type: "LifeSteal", value: 1.5 },
        5311: { type: "SpellVamp", value: 0.5 },
        5312: { type: "SpellVamp", value: 1.0 },
        5313: { type: "SpellVamp", value: 1.5 },
        5265: { type: "GoldPer10", value: 0.35 },
        5266: { type: "GoldPer10", value: 0.7 },
        5267: { type: "GoldPer10", value: 1.05 },
        5277: { type: "Health", value: 5.35 },
        5278: { type: "Health", value: 10.7 },
        5279: { type: "Health", value: 16.05 },
        5280: { type: "HealthRegen", value: 0.41 },
        5281: { type: "HealthRegen", value: 0.81 },
        5282: { type: "HealthRegen", value: 1.22 },
        5283: { type: "Mana", value: 8.18 },
        5284: { type: "Mana", value: 16.36 },
        5285: { type: "Mana", value: 24.54 },
        5286: { type: "ManaRegen", value: 0.26 },
        5287: { type: "ManaRegen", value: 0.51 },
        5288: { type: "ManaRegen", value: 0.77 },
        5302: { type: "CooldownReduction", value: 0.83 },
        5303: { type: "CooldownReduction", value: 1.66 },
        5304: { type: "CooldownReduction", value: 2.5 },
        5320: { type: "CriticalStrike", value: 1.86 },
        5321: { type: "CriticalStrike", value: 3.73 },
        5322: { type: "CriticalStrike", value: 5.59 }
      };

      Object.keys(currentPage.runes).forEach(slot => {
        const runeId = currentPage.runes[slot];
        if (runeData[runeId]) {
          effects[runeData[runeId].type] += runeData[runeId].value;
        }
      });

      return Object.fromEntries(
        Object.entries(effects).filter(([, v]) => v > 0)
      );
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
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/rune/${runeId}.png`;
    },
    getRuneName(runeId) {
      if (!runeId) return "Empty";
      const allRunes = [...this.runeData.mark, ...this.runeData.seal, ...this.runeData.glyph, ...this.runeData.quint];
      const rune = allRunes.find(r => r.id === runeId);
      return rune ? rune.name : "Unknown Rune";
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
      this.selectedSlotType = type;
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
};
</script>

<style lang="css" scoped>
#RuneMasteryPage {
  width: 100%;
  height: calc(100% - 115px);
  margin-top: 77px;
  position: relative;
  display: flex;
  overflow: hidden;
}

.runeMasteryContainer {
  width: 100%;
  padding: 20px 30px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.pageHeader {
  margin-bottom: 15px;
  flex-shrink: 0;
}

.pageHeader h2 {
  font-family: LoLFont2;
  font-size: 26px;
  color: #f2d030;
  margin: 0;
  -webkit-text-stroke: 1px black;
}

.pageHeader p {
  font-family: LoLFont2;
  font-size: 13px;
  color: rgba(255, 255, 255, 0.7);
  margin: 3px 0 0 0;
}

.runeMasteryContent {
  display: flex;
  gap: 15px;
  flex: 1;
  min-height: 0;
}

.runePagesList {
  width: 180px;
  min-width: 150px;
  background-color: rgba(5, 12, 20, 0.9);
  border: 1px solid rgba(100, 117, 137, 0.75);
  border-radius: 5px;
  display: flex;
  flex-direction: column;
  flex-shrink: 0;
}

.listHeader {
  padding: 8px 12px;
  font-family: LoLFont2;
  font-size: 13px;
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
  padding: 8px 12px;
  font-family: LoLFont2;
  font-size: 12px;
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
  padding: 8px 12px;
  display: flex;
  gap: 8px;
  border-top: 1px solid rgba(100, 117, 137, 0.75);
  flex-shrink: 0;
}

.actionBtn {
  flex: 1;
  padding: 6px 10px;
  font-family: LoLFont2;
  font-size: 11px;
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
}

.editorHeader {
  padding: 10px 15px;
  background-image: linear-gradient(180deg, #192e49 0%, #192e49 40%, #172b46 50%, #142131 100%);
  border-bottom: 1px solid rgba(100, 117, 137, 0.75);
  flex-shrink: 0;
}

.pageNameInput {
  width: 100%;
  padding: 6px 10px;
  background-color: #0a1320;
  border: 1px solid #304b69;
  border-radius: 3px;
  color: white;
  font-family: LoLFont2;
  font-size: 14px;
  outline: none;
}

.pageNameInput:focus {
  border-color: #f2d030;
}

.runeSlots {
  flex: 1;
  padding: 15px 20px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 8px;
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
  gap: 12px;
}

.rowLabel {
  width: 90px;
  min-width: 90px;
  font-family: LoLFont2;
  font-size: 12px;
  color: rgba(255, 255, 255, 0.7);
  text-align: right;
}

.slotGroup {
  display: flex;
  gap: 6px;
  flex-wrap: wrap;
}

.runeSlot {
  width: 44px;
  height: 44px;
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
  width: 34px;
  height: 34px;
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
  width: 52px;
  height: 52px;
  border-color: rgba(139, 92, 246, 0.5);
}

.quintSlot img {
  width: 42px;
  height: 42px;
}

.slotNumber {
  position: absolute;
  bottom: -2px;
  right: -2px;
  width: 14px;
  height: 14px;
  background-color: #0a1320;
  border-radius: 50%;
  font-size: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid rgba(100, 117, 137, 0.75);
  font-family: LoLFont2;
  color: rgba(255, 255, 255, 0.6);
}

.runeEffects {
  padding: 8px 15px;
  border-top: 1px solid rgba(100, 117, 137, 0.75);
  flex-shrink: 0;
}

.effectsHeader {
  font-family: LoLFont2;
  font-size: 12px;
  margin-bottom: 6px;
  color: rgba(255, 255, 255, 0.7);
}

.effectsList {
  display: flex;
  flex-wrap: wrap;
  gap: 10px 20px;
}

.effectItem {
  display: flex;
  gap: 8px;
  font-family: LoLFont2;
  font-size: 11px;
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
  width: 480px;
  max-height: 70vh;
  display: flex;
  flex-direction: column;
}

.modalHeader {
  padding: 12px 15px;
  background-image: linear-gradient(180deg, #192e49 0%, #192e49 40%, #172b46 50%, #142131 100%);
  border-bottom: 1px solid rgba(100, 117, 137, 0.75);
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-family: LoLFont2;
  font-size: 15px;
  flex-shrink: 0;
}

.closeBtn {
  background: none;
  border: none;
  color: rgba(255, 255, 255, 0.7);
  font-size: 18px;
  cursor: pointer;
  padding: 0 5px;
}

.closeBtn:hover {
  color: white;
}

.runeList {
  flex: 1;
  overflow-y: auto;
  padding: 8px 10px;
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
  gap: 12px;
  padding: 8px 10px;
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
  width: 32px;
  height: 32px;
  object-fit: contain;
  flex-shrink: 0;
}

.runeInfo {
  flex: 1;
  min-width: 0;
}

.runeName {
  font-family: LoLFont2;
  font-size: 13px;
  color: white;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.runeEffect {
  font-family: LoLFont2;
  font-size: 11px;
  color: #f2d030;
}
</style>