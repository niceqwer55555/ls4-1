<template>
  <div id="MasteryPage">
    <div class="runeMasteryContainer">
      <div class="pageHeader">
        <h2>{{ t("MASTERY_PAGE_TITLE") }}</h2>
        <p>{{ t("MASTERY_PAGE_DESC") }}</p>
      </div>
      
      <div class="runeMasteryContent">
        <div class="masteryPagesList">
          <div class="listHeader">
            {{ t("MASTERY_PAGE_LIST_TITLE") }}
          </div>
          <div class="pageList">
            <div
              v-for="(page, index) in masteryPages"
              :key="index"
              :class="{ selected: currentMasteryPage === index }"
              @click="selectMasteryPage(index)"
              class="pageItem"
            >
              <span class="pageName">{{ page.name || `Mastery Page ${index + 1}` }}</span>
              <span v-if="index === currentMasteryPage" class="currentIndicator">✓</span>
            </div>
          </div>
          <div class="pageActions">
            <button @click="addMasteryPage" class="actionBtn addBtn">
              + {{ t("MASTERY_PAGE_ADD_BTN") }}
            </button>
            <button
              @click="deleteMasteryPage"
              :disabled="masteryPages.length <= 1"
              class="actionBtn deleteBtn"
            >
              - {{ t("MASTERY_PAGE_DELETE_BTN") }}
            </button>
          </div>
        </div>

        <div class="masteryEditor">
          <div class="editorHeader">
            <input
              v-model="currentPageName"
              @blur="saveCurrentPageName"
              class="pageNameInput"
              :placeholder="t('MASTERY_PAGE_NAME_PLACEHOLDER')"
            />
            <div class="masteryPoints">
              <span>{{ t("MASTERY_PAGE_POINTS_LABEL") }}:</span>
              <span class="pointsUsed">{{ totalPointsUsed }}</span>
              <span>/</span>
              <span class="pointsMax">30</span>
            </div>
          </div>
          
          <div class="masteryTrees">
            <div class="treeContainer attackTree">
              <div class="treeHeader">
                <span class="treeIcon">⚔️</span>
                <span class="treeName">{{ t("MASTERY_TREE_ATTACK") }}</span>
                <span class="treePoints">{{ attackPoints }}</span>
              </div>
              <div class="treeContent">
                <div v-for="(row, rowIndex) in attackMasteries" :key="'attack-' + rowIndex" class="masteryRow">
                  <div
                    v-for="(mastery, colIndex) in row"
                    :key="'attack-' + rowIndex + '-' + colIndex"
                    :class="{ selected: isMasterySelected(mastery.id) }"
                    @click="toggleMastery(mastery.id)"
                    class="masteryItem"
                  >
                    <div class="masteryIcon">{{ mastery.icon }}</div>
                    <div class="masteryName">{{ mastery.name }}</div>
                    <div class="masteryDescription">{{ mastery.description }}</div>
                    <div v-if="isMasterySelected(mastery.id)" class="masteryRank">
                      {{ getMasteryRank(mastery.id) }}/{{ mastery.maxRank }}
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="treeContainer defenseTree">
              <div class="treeHeader">
                <span class="treeIcon">🛡️</span>
                <span class="treeName">{{ t("MASTERY_TREE_DEFENSE") }}</span>
                <span class="treePoints">{{ defensePoints }}</span>
              </div>
              <div class="treeContent">
                <div v-for="(row, rowIndex) in defenseMasteries" :key="'defense-' + rowIndex" class="masteryRow">
                  <div
                    v-for="(mastery, colIndex) in row"
                    :key="'defense-' + rowIndex + '-' + colIndex"
                    :class="{ selected: isMasterySelected(mastery.id) }"
                    @click="toggleMastery(mastery.id)"
                    class="masteryItem"
                  >
                    <div class="masteryIcon">{{ mastery.icon }}</div>
                    <div class="masteryName">{{ mastery.name }}</div>
                    <div class="masteryDescription">{{ mastery.description }}</div>
                    <div v-if="isMasterySelected(mastery.id)" class="masteryRank">
                      {{ getMasteryRank(mastery.id) }}/{{ mastery.maxRank }}
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="treeContainer utilityTree">
              <div class="treeHeader">
                <span class="treeIcon">🔮</span>
                <span class="treeName">{{ t("MASTERY_TREE_UTILITY") }}</span>
                <span class="treePoints">{{ utilityPoints }}</span>
              </div>
              <div class="treeContent">
                <div v-for="(row, rowIndex) in utilityMasteries" :key="'utility-' + rowIndex" class="masteryRow">
                  <div
                    v-for="(mastery, colIndex) in row"
                    :key="'utility-' + rowIndex + '-' + colIndex"
                    :class="{ selected: isMasterySelected(mastery.id) }"
                    @click="toggleMastery(mastery.id)"
                    class="masteryItem"
                  >
                    <div class="masteryIcon">{{ mastery.icon }}</div>
                    <div class="masteryName">{{ mastery.name }}</div>
                    <div class="masteryDescription">{{ mastery.description }}</div>
                    <div v-if="isMasterySelected(mastery.id)" class="masteryRank">
                      {{ getMasteryRank(mastery.id) }}/{{ mastery.maxRank }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          
          <div class="masteryEffects">
            <div class="effectsHeader">{{ t("MASTERY_PAGE_EFFECTS_TITLE") }}</div>
            <div class="effectsList">
              <div v-for="(effect, key) in masteryEffects" :key="key" class="effectItem">
                <span class="effectName">{{ key }}</span>
                <span class="effectValue">{{ effect }}</span>
              </div>
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
      attackMasteries: [
        [{ id: 6111, icon: "⚡", name: "Double-Edged Sword", description: "+3% damage, -1.5% damage taken", maxRank: 1 }],
        [{ id: 6112, icon: "💪", name: "Sunder", description: "+5% armor penetration", maxRank: 1 }, { id: 6113, icon: "🔫", name: "Fury", description: "+10% attack speed", maxRank: 1 }],
        [{ id: 6121, icon: "🔥", name: "Warlord", description: "Basic attacks restore 2% max HP", maxRank: 1 }, { id: 6122, icon: "⚔️", name: "Feast", description: "Kills restore 10% max HP", maxRank: 1 }],
        [{ id: 6131, icon: "🎯", name: "Brute Force", description: "+5 AD", maxRank: 3 }, { id: 6132, icon: "💥", name: "Impact", description: "+6% armor pen", maxRank: 3 }],
        [{ id: 6141, icon: "🏹", name: "Savage", description: "+3% damage to minions", maxRank: 1 }, { id: 6142, icon: "🗡️", name: "Bounty Hunter", description: "+2% damage per unique kill", maxRank: 1 }],
        [{ id: 6151, icon: "💀", name: "Deadliness", description: "+5 AD, +8 AP", maxRank: 3 }],
        [{ id: 6161, icon: "💢", name: "Executioner", description: "+5% damage to low HP targets", maxRank: 1 }, { id: 6162, icon: "🎰", name: "Oppressor", description: "+5% damage to slowed/immobilized", maxRank: 1 }],
        [{ id: 6171, icon: "⚡", name: "Weapon Expertise", description: "+10% armor pen, +5% magic pen", maxRank: 3 }],
        [{ id: 6181, icon: "💥", name: "Lethality", description: "Critical strikes deal 15% more damage", maxRank: 1 }],
        [{ id: 6191, icon: "🔥", name: "Deathfire Touch", description: "Basic attacks and spells burn enemies", maxRank: 1 }]
      ],
      defenseMasteries: [
        [{ id: 6211, icon: "🛡️", name: "Block", description: "Reduce auto damage by 3", maxRank: 1 }],
        [{ id: 6212, icon: "💚", name: "Recovery", description: "+2 HP regen", maxRank: 1 }, { id: 6213, icon: "🔄", name: "Perseverance", description: "+10% HP regen when low", maxRank: 1 }],
        [{ id: 6221, icon: "🎯", name: "Unyielding", description: "+5% tenacity", maxRank: 1 }, { id: 6222, icon: "💪", name: "Tough Skin", description: "+6 armor", maxRank: 1 }],
        [{ id: 6231, icon: "🛡️", name: "Iron Skin", description: "+6 armor", maxRank: 3 }, { id: 6232, icon: "🔮", name: "Mirror Skin", description: "+6 MR", maxRank: 3 }],
        [{ id: 6241, icon: "🌀", name: "Runic Affinity", description: "+15% duration of buffs", maxRank: 1 }, { id: 6242, icon: "⚡", name: "Reinforced Armor", description: "Reduce CC duration by 10%", maxRank: 1 }],
        [{ id: 6251, icon: "💚", name: "Juggernaut", description: "+20 HP, +2 HP regen", maxRank: 3 }],
        [{ id: 6261, icon: "🔥", name: "Resistance", description: "+10% damage reduction from DOT", maxRank: 1 }, { id: 6262, icon: "💪", name: "Veteran's Scars", description: "+60 HP", maxRank: 1 }],
        [{ id: 6271, icon: "🛡️", name: "Defiance", description: "+5% armor and MR", maxRank: 3 }],
        [{ id: 6281, icon: "🗡️", name: "Revenge", description: "+10% damage when below 50% HP", maxRank: 1 }],
        [{ id: 6291, icon: "💀", name: "Stoneborn Pact", description: "Allies within 300 range gain +10% armor/MR", maxRank: 1 }]
      ],
      utilityMasteries: [
        [{ id: 6311, icon: "💰", name: "Savings", description: "+15 gold at start", maxRank: 1 }],
        [{ id: 6312, icon: "⚡", name: "Bandit", description: "2 gold per auto on champions", maxRank: 1 }, { id: 6313, icon: "🏃", name: "Swift", description: "+2% movement speed", maxRank: 1 }],
        [{ id: 6321, icon: "💨", name: "Fleet of Foot", description: "+4% movement speed out of combat", maxRank: 1 }, { id: 6322, icon: "⚡", name: "Expanded Mind", description: "+10% max mana", maxRank: 1 }],
        [{ id: 6331, icon: "🔮", name: "Meditation", description: "+1.5 MP regen", maxRank: 3 }, { id: 6332, icon: "💧", name: "Mana Font", description: "+50 max mana", maxRank: 3 }],
        [{ id: 6341, icon: "⏱️", name: "Cognitive Flexibility", description: "+3% CDR", maxRank: 1 }, { id: 6342, icon: "🔥", name: "Burning Embers", description: "+5 AP", maxRank: 1 }],
        [{ id: 6351, icon: "⚡", name: "Intelligence", description: "+5% CDR", maxRank: 3 }],
        [{ id: 6361, icon: "🔮", name: "Arcane Knowledge", description: "+8% magic penetration", maxRank: 1 }, { id: 6362, icon: "💨", name: "Phase Rush", description: "+5% movement speed", maxRank: 1 }],
        [{ id: 6371, icon: "✨", name: "Wanderer", description: "+2% movement speed", maxRank: 3 }],
        [{ id: 6381, icon: "🌀", name: "Distortion", description: "+10% summoner spell CDR", maxRank: 1 }],
        [{ id: 6391, icon: "🌟", name: "Thunderlord's Decree", description: "3 consecutive attacks/spells deal lightning damage", maxRank: 1 }]
      ]
    };
  },
  computed: {
    ...mapState({
      masteryPages: state => state.masteryPages,
      currentMasteryPage: state => state.currentMasteryPage
    }),
    totalPointsUsed() {
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage || !currentPage.masteries) return 0;
      return Object.values(currentPage.masteries).reduce((sum, rank) => sum + rank, 0);
    },
    attackPoints() {
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage || !currentPage.masteries) return 0;
      return this.attackMasteries.flat().reduce((sum, m) => sum + (currentPage.masteries[m.id] || 0), 0);
    },
    defensePoints() {
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage || !currentPage.masteries) return 0;
      return this.defenseMasteries.flat().reduce((sum, m) => sum + (currentPage.masteries[m.id] || 0), 0);
    },
    utilityPoints() {
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage || !currentPage.masteries) return 0;
      return this.utilityMasteries.flat().reduce((sum, m) => sum + (currentPage.masteries[m.id] || 0), 0);
    },
    masteryEffects() {
      const effects = {};
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage || !currentPage.masteries) return effects;

      const masteryEffectsMap = {
        6111: { "Damage": "+3%", "Damage Taken": "-1.5%" },
        6112: { "Armor Penetration": "+5%" },
        6113: { "Attack Speed": "+10%" },
        6121: { "Lifesteal": "+2% max HP per auto" },
        6122: { "HP Restore": "+10% max HP on kill" },
        6131: { "Attack Damage": "+5 AD" },
        6132: { "Armor Penetration": "+6%" },
        6141: { "Minion Damage": "+3%" },
        6142: { "Damage": "+2% per unique kill" },
        6151: { "Attack Damage": "+5 AD", "Ability Power": "+8 AP" },
        6161: { "Damage to Low HP": "+5%" },
        6162: { "Damage to CC'd": "+5%" },
        6171: { "Armor Pen": "+10%", "Magic Pen": "+5%" },
        6181: { "Crit Damage": "+15%" },
        6191: { "Burn Damage": "Active" },
        6211: { "Auto Damage Reduction": "-3" },
        6212: { "HP Regen": "+2" },
        6213: { "Low HP Regen": "+10%" },
        6221: { "Tenacity": "+5%" },
        6222: { "Armor": "+6" },
        6231: { "Armor": "+6" },
        6232: { "Magic Resist": "+6" },
        6241: { "Buff Duration": "+15%" },
        6242: { "CC Duration": "-10%" },
        6251: { "Health": "+20", "HP Regen": "+2" },
        6261: { "DOT Reduction": "+10%" },
        6262: { "Health": "+60" },
        6271: { "Armor/MR": "+5%" },
        6281: { "Damage When Low": "+10%" },
        6291: { "Ally Armor/MR": "+10%" },
        6311: { "Starting Gold": "+15" },
        6312: { "Gold per Auto": "+2" },
        6313: { "Movement Speed": "+2%" },
        6321: { "Out of Combat MS": "+4%" },
        6322: { "Max Mana": "+10%" },
        6331: { "MP Regen": "+1.5" },
        6332: { "Max Mana": "+50" },
        6341: { "Cooldown Reduction": "+3%" },
        6342: { "Ability Power": "+5" },
        6351: { "Cooldown Reduction": "+5%" },
        6361: { "Magic Penetration": "+8%" },
        6362: { "Movement Speed": "+5%" },
        6371: { "Movement Speed": "+2%" },
        6381: { "Summoner CDR": "+10%" },
        6391: { "Lightning Damage": "Active" }
      };

      Object.keys(currentPage.masteries).forEach(id => {
        const rank = currentPage.masteries[id];
        if (rank && masteryEffectsMap[id]) {
          Object.entries(masteryEffectsMap[id]).forEach(([key, value]) => {
            effects[key] = value;
          });
        }
      });

      return effects;
    }
  },
  methods: {
    ...mapActions(["selectMasteryPage", "addMasteryPage", "deleteMasteryPage"]),
    isMasterySelected(masteryId) {
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage || !currentPage.masteries) return false;
      return (currentPage.masteries[masteryId] || 0) > 0;
    },
    getMasteryRank(masteryId) {
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage || !currentPage.masteries) return 0;
      return currentPage.masteries[masteryId] || 0;
    },
    toggleMastery(masteryId) {
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage) return;

      const mastery = [...this.attackMasteries, ...this.defenseMasteries, ...this.utilityMasteries]
        .flat()
        .find(m => m.id === masteryId);

      if (!mastery) return;

      const currentRank = currentPage.masteries[masteryId] || 0;
      const pointsNeeded = currentRank < mastery.maxRank ? 1 : -1;

      if (this.totalPointsUsed + pointsNeeded > 30) return;

      const updatedPages = [...this.masteryPages];
      updatedPages[this.currentMasteryPage] = {
        ...updatedPages[this.currentMasteryPage],
        masteries: {
          ...updatedPages[this.currentMasteryPage].masteries,
          [masteryId]: Math.max(0, currentRank + pointsNeeded)
        }
      };

      this.$store.commit("setMasteryPages", updatedPages);
      this.$store.dispatch("saveRuneMasteryPages");
    },
    saveCurrentPageName() {
      if (this.currentPageName.trim()) {
        const updatedPages = [...this.masteryPages];
        updatedPages[this.currentMasteryPage] = {
          ...updatedPages[this.currentMasteryPage],
          name: this.currentPageName.trim()
        };
        this.$store.commit("setMasteryPages", updatedPages);
        this.$store.dispatch("saveRuneMasteryPages");
      }
    }
  },
  mounted() {
    this.$store.dispatch("loadRuneMasteryPages");
    const currentPage = this.masteryPages[this.currentMasteryPage];
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
#MasteryPage {
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

.masteryPagesList {
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

.masteryEditor {
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
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-shrink: 0;
  gap: 15px;
}

.pageNameInput {
  flex: 1;
  padding: 6px 10px;
  background-color: #0a1320;
  border: 1px solid #304b69;
  border-radius: 3px;
  color: white;
  font-family: LoLFont2;
  font-size: 14px;
  outline: none;
  min-width: 0;
}

.pageNameInput:focus {
  border-color: #f2d030;
}

.masteryPoints {
  font-family: LoLFont2;
  font-size: 13px;
  white-space: nowrap;
}

.pointsUsed {
  color: #f2d030;
}

.pointsMax {
  color: rgba(255, 255, 255, 0.7);
}

.masteryTrees {
  flex: 1;
  padding: 10px 15px;
  display: flex;
  gap: 12px;
  overflow: hidden;
  min-height: 0;
}

.treeContainer {
  flex: 1;
  background-color: rgba(0, 0, 0, 0.3);
  border-radius: 5px;
  border: 1px solid rgba(100, 117, 137, 0.5);
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.attackTree {
  border-color: #c84630;
}

.defenseTree {
  border-color: #3c73b4;
}

.utilityTree {
  border-color: #f2d030;
}

.treeHeader {
  padding: 6px 10px;
  display: flex;
  align-items: center;
  gap: 8px;
  font-family: LoLFont2;
  font-size: 12px;
  border-bottom: 1px solid rgba(100, 117, 137, 0.5);
  flex-shrink: 0;
}

.attackTree .treeHeader {
  background: linear-gradient(180deg, #4a1818 0%, #2a0808 100%);
}

.defenseTree .treeHeader {
  background: linear-gradient(180deg, #182a4a 0%, #08182a 100%);
}

.utilityTree .treeHeader {
  background: linear-gradient(180deg, #4a4a18 0%, #2a2a08 100%);
}

.treeIcon {
  font-size: 16px;
}

.treeName {
  font-size: 12px;
}

.treePoints {
  margin-left: auto;
  color: #f2d030;
  font-size: 13px;
}

.treeContent {
  flex: 1;
  padding: 8px 6px;
  overflow-y: auto;
}

.treeContent::-webkit-scrollbar {
  width: 3px;
}

.treeContent::-webkit-scrollbar-thumb {
  background: rgba(100, 117, 137, 0.5);
  border-radius: 2px;
}

.masteryRow {
  display: flex;
  gap: 4px;
  margin-bottom: 4px;
}

.masteryItem {
  flex: 1;
  padding: 5px 4px;
  background-color: rgba(0, 0, 0, 0.3);
  border-radius: 3px;
  cursor: pointer;
  transition: all ease-in-out 200ms;
  border: 1px solid transparent;
  text-align: center;
  min-width: 0;
}

.masteryItem:hover {
  background-color: rgba(100, 117, 137, 0.2);
}

.masteryItem.selected {
  background-color: rgba(13, 71, 49, 0.4);
  border-color: #306d32;
}

.masteryIcon {
  font-size: 14px;
  margin-bottom: 2px;
}

.masteryName {
  font-family: LoLFont2;
  font-size: 10px;
  margin-bottom: 2px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.masteryDescription {
  font-family: LoLFont2;
  font-size: 9px;
  color: rgba(255, 255, 255, 0.6);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.masteryRank {
  font-family: LoLFont2;
  font-size: 9px;
  color: #f2d030;
  margin-top: 2px;
}

.masteryEffects {
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
</style>