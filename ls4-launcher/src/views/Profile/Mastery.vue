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
              <span class="pageName">{{ page.name || '天赋页' }}</span>
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
                <div v-for="(row, rowIndex) in attackMasteries" :key="'attack-' + rowIndex" class="masteryRow" :class="{ locked: !isTierUnlocked('offense', rowIndex) }">
                  <div
                    v-for="mastery in row"
                    :key="mastery.id"
                    :class="{ selected: isMasterySelected(mastery.id) }"
                    @click="addMasteryPoint(mastery.id)"
                    @contextmenu.prevent="removeMasteryPoint(mastery.id)"
                    class="masteryItem"
                    :title="mastery.description"
                  >
                    <div class="masteryIconWrapper">
                      <img :src="getMasteryImage(mastery.id)" :alt="mastery.name" class="masteryIconImg" />
                      <span class="masteryRankBadge" :class="{ maxed: getMasteryRank(mastery.id) >= mastery.maxRank }">
                        {{ getMasteryRank(mastery.id) }}/{{ mastery.maxRank }}
                      </span>
                    </div>
                    <div class="masteryName">{{ mastery.name }}</div>
                    <div class="masteryRank" :class="{ maxed: getMasteryRank(mastery.id) >= mastery.maxRank }">
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
                <div v-for="(row, rowIndex) in defenseMasteries" :key="'defense-' + rowIndex" class="masteryRow" :class="{ locked: !isTierUnlocked('defense', rowIndex) }">
                  <div
                    v-for="mastery in row"
                    :key="mastery.id"
                    :class="{ selected: isMasterySelected(mastery.id) }"
                    @click="addMasteryPoint(mastery.id)"
                    @contextmenu.prevent="removeMasteryPoint(mastery.id)"
                    class="masteryItem"
                    :title="mastery.description"
                  >
                    <div class="masteryIconWrapper">
                      <img :src="getMasteryImage(mastery.id)" :alt="mastery.name" class="masteryIconImg" />
                      <span class="masteryRankBadge" :class="{ maxed: getMasteryRank(mastery.id) >= mastery.maxRank }">
                        {{ getMasteryRank(mastery.id) }}/{{ mastery.maxRank }}
                      </span>
                    </div>
                    <div class="masteryName">{{ mastery.name }}</div>
                    <div class="masteryRank" :class="{ maxed: getMasteryRank(mastery.id) >= mastery.maxRank }">
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
                <div v-for="(row, rowIndex) in utilityMasteries" :key="'utility-' + rowIndex" class="masteryRow" :class="{ locked: !isTierUnlocked('utility', rowIndex) }">
                  <div
                    v-for="mastery in row"
                    :key="mastery.id"
                    :class="{ selected: isMasterySelected(mastery.id) }"
                    @click="addMasteryPoint(mastery.id)"
                    @contextmenu.prevent="removeMasteryPoint(mastery.id)"
                    class="masteryItem"
                    :title="mastery.description"
                  >
                    <div class="masteryIconWrapper">
                      <img :src="getMasteryImage(mastery.id)" :alt="mastery.name" class="masteryIconImg" />
                      <span class="masteryRankBadge" :class="{ maxed: getMasteryRank(mastery.id) >= mastery.maxRank }">
                        {{ getMasteryRank(mastery.id) }}/{{ mastery.maxRank }}
                      </span>
                    </div>
                    <div class="masteryName">{{ mastery.name }}</div>
                    <div class="masteryRank" :class="{ maxed: getMasteryRank(mastery.id) >= mastery.maxRank }">
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
</template><script>
import { mapState, mapActions } from "vuex";
import { MASTERY_TREES, MASTERY_DATA } from "@/utils/masteryData";

export default {
  data() {
    return {
      currentPageName: "",
      masteryTreeData: MASTERY_TREES
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
      return this.getTreePoints("offense");
    },
    defensePoints() {
      return this.getTreePoints("defense");
    },
    utilityPoints() {
      return this.getTreePoints("utility");
    },
    attackMasteries() {
      return this.getTreeMasteries("offense");
    },
    defenseMasteries() {
      return this.getTreeMasteries("defense");
    },
    utilityMasteries() {
      return this.getTreeMasteries("utility");
    },
    masteryEffects() {
      const effects = {};
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage || !currentPage.masteries) return effects;
      Object.keys(currentPage.masteries).forEach(id => {
        const rank = currentPage.masteries[id];
        if (rank) {
          const m = MASTERY_DATA[id];
          if (m) {
            effects[m.name] = m.desc;
          }
        }
      });
      return effects;
    }
  },
  methods: {
    ...mapActions(["selectMasteryPage", "addMasteryPage", "deleteMasteryPage"]),
    getTreeMasteries(treeName) {
      const tree = MASTERY_TREES[treeName];
      if (!tree) return [];
      return tree.tiers.map(tier => {
        return tier.masteries.map(id => {
          const m = MASTERY_DATA[id];
          return m ? { id: parseInt(id), name: m.name, description: m.desc, maxRank: m.maxRanks, tree: m.tree, tier: m.tier } : null;
        }).filter(Boolean);
      });
    },
    getTreePoints(treeName) {
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage || !currentPage.masteries) return 0;
      const tree = MASTERY_TREES[treeName];
      if (!tree) return 0;
      const allIds = tree.tiers.flatMap(t => t.masteries);
      return allIds.reduce((sum, id) => sum + (currentPage.masteries[id] || 0), 0);
    },
    isTierUnlocked(treeName, tierIndex) {
      const tree = MASTERY_TREES[treeName];
      if (!tree) return false;
      const requiredPoints = tree.tiers[tierIndex].requiredPoints;
      const treePoints = this.getTreePoints(treeName);
      return treePoints >= requiredPoints;
    },
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
    addMasteryPoint(masteryId) {
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage) return;
      const masteryInfo = MASTERY_DATA[String(masteryId)];
      if (!masteryInfo) return;
      if (!this.isTierUnlocked(masteryInfo.tree, masteryInfo.tier - 1)) return;
      const currentRank = currentPage.masteries[masteryId] || 0;
      if (currentRank >= masteryInfo.maxRanks) return;
      if (this.totalPointsUsed >= 30) return;
      const updatedPages = [...this.masteryPages];
      updatedPages[this.currentMasteryPage] = {
        ...updatedPages[this.currentMasteryPage],
        masteries: {
          ...updatedPages[this.currentMasteryPage].masteries,
          [masteryId]: currentRank + 1
        }
      };
      this.$store.commit("setMasteryPages", updatedPages);
      this.$store.dispatch("saveRuneMasteryPages");
    },
    removeMasteryPoint(masteryId) {
      const currentPage = this.masteryPages[this.currentMasteryPage];
      if (!currentPage) return;
      const currentRank = currentPage.masteries[masteryId] || 0;
      if (currentRank <= 0) return;
      const updatedPages = [...this.masteryPages];
      const newMasteries = { ...updatedPages[this.currentMasteryPage].masteries };
      if (currentRank <= 1) {
        delete newMasteries[masteryId];
      } else {
        newMasteries[masteryId] = currentRank - 1;
      }
      updatedPages[this.currentMasteryPage] = {
        ...updatedPages[this.currentMasteryPage],
        masteries: newMasteries
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
    },
    getMasteryImage(masteryId) {
      const { host, port } = this.$store.state.config.download;
      const isSelected = this.isMasterySelected(masteryId);
      const prefix = isSelected ? "" : "gray_";
      return `${host}:${port}/mastery/${prefix}${masteryId}.png`;
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
}
</script>

<style lang="css" scoped>
#MasteryPage {
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

.masteryPagesList {
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

.masteryEditor {
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
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-shrink: 0;
  gap: 10px;
}

.pageNameInput {
  flex: 1;
  padding: 5px 8px;
  background-color: #0a1320;
  border: 1px solid #304b69;
  border-radius: 3px;
  color: white;
  font-family: LoLFont2;
  font-size: 13px;
  outline: none;
  min-width: 0;
}

.pageNameInput:focus {
  border-color: #f2d030;
}

.masteryPoints {
  font-family: LoLFont2;
  font-size: 12px;
  white-space: nowrap;
  flex-shrink: 0;
}

.pointsUsed {
  color: #f2d030;
}

.pointsMax {
  color: rgba(255, 255, 255, 0.7);
}

.masteryTrees {
  flex: 1;
  padding: 6px 8px;
  display: flex;
  gap: 6px;
  overflow-y: auto;
  overflow-x: hidden;
  min-height: 0;
}

.masteryTrees::-webkit-scrollbar {
  width: 4px;
}

.masteryTrees::-webkit-scrollbar-thumb {
  background: rgba(100, 117, 137, 0.5);
  border-radius: 2px;
}

.treeContainer {
  flex: 1;
  min-width: 0;
  background-color: rgba(0, 0, 0, 0.3);
  border-radius: 5px;
  border: 1px solid rgba(100, 117, 137, 0.5);
  display: flex;
  flex-direction: column;
  overflow: hidden;
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
  padding: 4px 8px;
  display: flex;
  align-items: center;
  gap: 4px;
  font-family: LoLFont2;
  font-size: 11px;
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
  font-size: 14px;
}

.treeName {
  font-size: 11px;
}

.treePoints {
  margin-left: auto;
  color: #f2d030;
  font-size: 12px;
}

.treeContent {
  flex: 1;
  padding: 4px 3px;
  overflow-y: auto;
  overflow-x: hidden;
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
  gap: 3px;
  margin-bottom: 3px;
}

.masteryItem {
  flex: 1;
  padding: 3px 2px;
  background-color: rgba(0, 0, 0, 0.3);
  border-radius: 3px;
  cursor: pointer;
  transition: all ease-in-out 200ms;
  border: 1px solid transparent;
  text-align: center;
  min-width: 0;
  overflow: hidden;
}

.masteryRow.locked .masteryItem {
  opacity: 0.4;
  cursor: not-allowed;
}

.masteryItem:hover {
  background-color: rgba(100, 117, 137, 0.2);
}

.masteryItem.selected {
  background-color: rgba(13, 71, 49, 0.4);
  border-color: #306d32;
}

.masteryRank.maxed {
  color: #f2d030;
  text-shadow: 0 0 6px rgba(242, 208, 48, 0.5);
}

.masteryIconWrapper {
  position: relative;
  width: clamp(28px, 4vw, 40px);
  height: clamp(28px, 4vw, 40px);
  margin: 0 auto 2px;
}

.masteryIconImg {
  width: 100%;
  height: 100%;
  object-fit: contain;
  border-radius: 4px;
}

.masteryRankBadge {
  position: absolute;
  bottom: -2px;
  right: -2px;
  font-size: 7px;
  background: rgba(0, 0, 0, 0.8);
  color: #f2d030;
  padding: 1px 2px;
  border-radius: 2px;
  border: 1px solid rgba(100, 117, 137, 0.75);
}

.masteryRankBadge.maxed {
  color: #50ff50;
  border-color: #50ff50;
  text-shadow: 0 0 4px rgba(80, 255, 80, 0.5);
}

.masteryName {
  font-family: LoLFont2;
  font-size: clamp(8px, 1vw, 10px);
  margin-bottom: 1px;
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
  font-size: clamp(8px, 0.9vw, 9px);
  color: #f2d030;
  margin-top: 1px;
}

.masteryEffects {
  padding: 6px 12px;
  border-top: 1px solid rgba(100, 117, 137, 0.75);
  flex-shrink: 0;
  max-height: 100px;
  overflow-y: auto;
}

.masteryEffects::-webkit-scrollbar {
  width: 3px;
}

.masteryEffects::-webkit-scrollbar-thumb {
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

/* Responsive: narrow screens - stack trees vertically */
@media (max-width: 900px) {
  .masteryTrees {
    flex-direction: column;
  }
  .treeContainer {
    min-height: 0;
  }
  .runeMasteryContent {
    flex-direction: column;
  }
  .masteryPagesList {
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
  .editorHeader {
    padding: 4px 8px;
  }
  .masteryTrees {
    padding: 4px;
    gap: 4px;
  }
  .masteryEffects {
    max-height: 60px;
    padding: 4px 8px;
  }
}
</style>
