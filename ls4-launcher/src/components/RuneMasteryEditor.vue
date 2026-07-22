<template>
  <div class="runeMasteryEditor" v-if="visible">
    <div class="backdrop" @click="close"></div>
    <div class="modal">
      <div class="header">
        <h2>{{ t("RM_EDITOR_TITLE") }}</h2>
        <button class="closeBtn" @click="close">
          <i class="fas fa-times"></i>
        </button>
      </div>
      <div class="tabs">
        <button :class="{ active: activeTab == 'RUNES' }" @click="activeTab = 'RUNES'">
          <span class="tabIcon">&#x2694;</span> {{ t("RM_TAB_RUNES") }}
        </button>
        <button :class="{ active: activeTab == 'MASTERIES' }" @click="activeTab = 'MASTERIES'">
          <span class="tabIcon">&#x2728;</span> {{ t("RM_TAB_MASTERIES") }}
        </button>
      </div>
      <div class="content">
        <!-- RUNES TAB -->
        <div class="runePageView" v-if="activeTab == 'RUNES' && !editing">
          <div class="pagesColumn">
            <div class="pageList">
              <div v-for="(page, index) in runePages" :key="'rp-'+index"
                :class="{ active: currentRunePage == index }" class="pageItem"
                @click="selectRunePage(index)">
                <span class="pageName">{{ page.name || ('符文页 ' + (index+1)) }}</span>
                <div class="actions">
                  <button @click.stop="editRunePage(index)" title="编辑"><i class="fas fa-edit"></i></button>
                  <button @click.stop="deleteRunePage(index)" v-if="runePages.length > 1" title="删除"><i class="fas fa-trash"></i></button>
                </div>
              </div>
            </div>
            <button class="addBtn" @click="addRunePage"><i class="fas fa-plus"></i> 新符文页</button>
          </div>
          <div class="runePreview" v-if="runePages.length > 0 && runePages[currentRunePage]">
            <h3>{{ (runePages[currentRunePage] || {}).name || '符文页' }}</h3>
            <div class="runeGrid">
              <div class="runeSection" v-for="section in runeSections" :key="section.type">
                <div class="sectionHeader" :style="{ borderColor: section.color }">
                  <span class="sectionIcon">{{ section.symbol }}</span>
                  <span class="sectionLabel">{{ section.label }}</span>
                </div>
                <div class="runeSlots">
                  <div v-for="slot in section.slots" :key="slot" class="runeSlot" :style="getSlotStyle(section, slot)">
                    <span class="runeSymbol">{{ section.symbol }}</span>
                    <span class="runeName" v-if="getRuneName(currentRunePage, slot)">{{ getRuneStatText(currentRunePage, slot) }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- MASTERIES TAB -->
        <div class="masteryPageView" v-if="activeTab == 'MASTERIES' && !editing">
          <div class="pagesColumn">
            <div class="pageList">
              <div v-for="(page, index) in masteryPages" :key="'mp-'+index"
                :class="{ active: currentMasteryPage == index }" class="pageItem"
                @click="selectMasteryPage(index)">
                <span class="pageName">{{ page.name || ('天赋页 ' + (index+1)) }}</span>
                <div class="actions">
                  <button @click.stop="editMasteryPage(index)" title="编辑"><i class="fas fa-edit"></i></button>
                  <button @click.stop="deleteMasteryPage(index)" v-if="masteryPages.length > 1" title="删除"><i class="fas fa-trash"></i></button>
                </div>
              </div>
            </div>
            <button class="addBtn" @click="addMasteryPage"><i class="fas fa-plus"></i> 新天赋页</button>
          </div>
          <div class="masteryPreview" v-if="masteryPages.length > 0 && masteryPages[currentMasteryPage]">
            <h3>{{ (masteryPages[currentMasteryPage] || {}).name || '天赋页' }}</h3>
            <div class="masterySummary">
              <span class="summaryOffense" :style="{color: '#c53030'}">攻击 {{ getTreePoints('offense') }}</span>
              <span class="summaryDefense" :style="{color: '#3182ce'}">防御 {{ getTreePoints('defense') }}</span>
              <span class="summaryUtility" :style="{color: '#38a169'}">通用 {{ getTreePoints('utility') }}</span>
            </div>
            <div class="masteryTreePreview">
              <div v-for="treeName in ['offense', 'defense', 'utility']" :key="treeName" class="treePreviewSection"
                :style="{ borderLeftColor: getTreeColor(treeName) }">
                <div class="treePreviewHeader">{{ getTreeLabel(treeName) }} ({{ getTreePoints(treeName) }})</div>
                <div v-for="(tier, tIdx) in getMasteryTreeData(treeName)" :key="tIdx" class="tierPreviewRow">
                  <span v-for="m in tier" :key="m.id" class="tierPreviewItem"
                    :class="{ hasPoints: getMasteryPoints(m.id) > 0 }"
                    :style="{ borderColor: getTreeColor(treeName) }">
                    {{ m.name }} {{ getMasteryPoints(m.id) }}/{{ m.maxRanks }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- EDIT RUNE PAGE -->
        <div class="editPanel" v-if="editing && editing.isRune">
          <div class="editHeader">
            <h3>编辑符文页</h3>
            <button class="cancelBtn" @click="cancelEdit">取消</button>
          </div>
          <div class="editForm">
            <div class="formGroup">
              <label>名称</label>
              <input v-model="editing.name" type="text" />
            </div>
            <div class="formGroup">
              <label>符文配置</label>
              <div class="runeEditGrid">
                <div v-for="section in runeSections" :key="section.type" class="runeEditSection">
                  <div class="sectionHeader" :style="{ borderColor: section.color }">
                    {{ section.label }}
                  </div>
                  <div class="runeEditSlots">
                    <div v-for="(slotEntry, idx) in getEditingSlotsForType(section.type)" :key="idx" class="runeEditSlot">
                      <span class="slotLabel">槽位 {{ slotEntry.slot }}</span>
                      <select v-model="slotEntry.runeId" class="runeSelect">
                        <option v-for="rune in getRunesForType(section.type)" :key="rune.id" :value="rune.id">
                          {{ rune.name }} ({{ rune.statText }})
                        </option>
                      </select>
                      <button class="removeSlotBtn" @click="removeRuneSlot(section.type, idx)" title="移除">
                        <i class="fas fa-minus-circle"></i>
                      </button>
                    </div>
                    <button class="addSlotBtn" @click="addRuneSlot(section.type)">
                      <i class="fas fa-plus"></i> 添加{{ section.label }}
                    </button>
                  </div>
                </div>
              </div>
            </div>
            <div class="formActions">
              <button class="saveBtn" @click="saveEdit">保存</button>
              <button class="cancelBtn" @click="cancelEdit">取消</button>
            </div>
          </div>
        </div>

        <!-- EDIT MASTERY PAGE -->
        <div class="editPanel" v-if="editing && !editing.isRune">
          <div class="editHeader">
            <h3>编辑天赋页</h3>
            <button class="cancelBtn" @click="cancelEdit">取消</button>
          </div>
          <div class="editForm">
            <div class="formGroup">
              <label>名称</label>
              <input v-model="editing.name" type="text" />
            </div>
            <div class="formGroup">
              <label>天赋点分配 (总计: {{ totalMasteryPoints }}/30)</label>
              <div class="masteryEditorTree">
                <div v-for="treeName in ['offense', 'defense', 'utility']" :key="treeName" class="masteryEditorSection"
                  :style="{ borderLeft: '3px solid ' + getTreeColor(treeName) }">
                  <div class="masteryEditorHeader" :style="{ color: getTreeColor(treeName) }">
                    {{ getTreeLabel(treeName) }} ({{ getEditingTreePoints(treeName) }})
                  </div>
                  <div v-for="(tier, tIdx) in getMasteryTreeData(treeName)" :key="tIdx" class="masteryTierRow">
                    <div v-for="m in tier" :key="m.id" class="masteryTierItem"
                      :class="{ maxed: getEditingMasteryPoints(m.id) >= m.maxRanks, hasPoints: getEditingMasteryPoints(m.id) > 0 }">
                      <span class="masteryName" :title="m.desc">{{ m.name }}</span>
                      <input v-model.number="editing.masteries[m.id]" type="number"
                        :min="0" :max="m.maxRanks" class="masteryInput"
                        @change="clampMasteryPoints(m.id, m.maxRanks)" />
                      <span class="masteryMax">/{{ m.maxRanks }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="formActions">
              <button class="saveBtn" @click="saveEdit">保存</button>
              <button class="cancelBtn" @click="cancelEdit">取消</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mapState } from "vuex";
import { RUNE_DATA, getSlotType, STAT_NAMES } from "@/utils/runeData";
import { MASTERY_TREES, MASTERY_DATA } from "@/utils/masteryData";
import { RUNE_TYPE_ICONS } from "@/utils/runeIcons";

export default {
  props: { visible: Boolean },
  data() {
    return {
      activeTab: "RUNES",
      editing: null
    };
  },
  computed: {
    t() { return (key) => this.$translate.text(key); },
    ...mapState({
      runePages: state => state.runePages || [],
      masteryPages: state => state.masteryPages || [],
      currentRunePage: state => state.currentRunePage || 0,
      currentMasteryPage: state => state.currentMasteryPage || 0
    }),
    runeSections() {
      return [
        { type: "mark", label: "印记 (红)", symbol: RUNE_TYPE_ICONS.mark.symbol, color: "#c53030", slots: [1,2,3,4,5,6,7,8,9] },
        { type: "seal", label: "符印 (黄)", symbol: RUNE_TYPE_ICONS.seal.symbol, color: "#d69e2e", slots: [10,11,12,13,14,15,16,17,18] },
        { type: "glyph", label: "雕文 (蓝)", symbol: RUNE_TYPE_ICONS.glyph.symbol, color: "#3182ce", slots: [19,20,21,22,23,24,25,26,27] },
        { type: "quintessence", label: "精华 (紫)", symbol: RUNE_TYPE_ICONS.quintessence.symbol, color: "#805ad5", slots: [28,29,30] }
      ];
    },
    totalMasteryPoints() {
      if (!this.editing || !this.editing.masteries) return 0;
      return Object.values(this.editing.masteries).reduce((s, v) => s + (v || 0), 0);
    }
  },
  methods: {
    getRunesForType(type) {
      return Object.entries(RUNE_DATA)
        .filter(([, r]) => r.type === type && r.tier === 3)
        .map(([id, r]) => {
          const statText = Object.entries(r.stats).map(([k, v]) => {
            const name = STAT_NAMES[k] || k;
            const val = Math.abs(v);
            const sign = v > 0 ? "+" : "";
            return sign + (val < 1 ? (val * 100).toFixed(2) + "%" : val.toFixed(2)) + " " + name;
          }).join(", ");
          return { id: parseInt(id), name: r.name, statText };
        })
        .sort((a, b) => a.name.localeCompare(b.name, "zh-CN"));
    },
    getRunesForSlot(slotPosition) {
      const slotType = getSlotType(slotPosition);
      return this.getRunesForType(slotType);
    },
    getMasteryTreeData(treeName) {
      const tree = MASTERY_TREES[treeName];
      if (!tree) return [];
      return tree.tiers.map(tier =>
        tier.masteries.map(id => {
          const m = MASTERY_DATA[id];
          return m ? { id: parseInt(id), name: m.name, desc: m.desc, maxRanks: m.maxRanks } : null;
        }).filter(Boolean)
      );
    },
    getTreeColor(treeName) {
      const colors = { offense: "#c53030", defense: "#3182ce", utility: "#38a169" };
      return colors[treeName] || "#999";
    },
    getTreeLabel(treeName) {
      const labels = { offense: "攻击", defense: "防御", utility: "通用" };
      return labels[treeName] || treeName;
    },
    getTreePoints(treeName) {
      const page = this.masteryPages[this.currentMasteryPage];
      if (!page || !page.masteries) return 0;
      return Object.entries(page.masteries).reduce((sum, [id, rank]) => {
        const m = MASTERY_DATA[id];
        return sum + (m && m.tree === treeName ? rank : 0);
      }, 0);
    },
    getEditingTreePoints(treeName) {
      if (!this.editing || !this.editing.masteries) return 0;
      return Object.entries(this.editing.masteries).reduce((sum, [id, rank]) => {
        const m = MASTERY_DATA[id];
        return sum + (m && m.tree === treeName ? (rank || 0) : 0);
      }, 0);
    },
    getMasteryPoints(id) {
      const page = this.masteryPages[this.currentMasteryPage];
      if (!page || !page.masteries) return 0;
      return page.masteries[id] || 0;
    },
    getEditingMasteryPoints(id) {
      if (!this.editing || !this.editing.masteries) return 0;
      return this.editing.masteries[id] || 0;
    },
    clampMasteryPoints(id, maxRanks) {
      if (!this.editing || !this.editing.masteries) return;
      const current = this.editing.masteries[id] || 0;
      this.editing.masteries[id] = Math.max(0, Math.min(maxRanks, current));
      // Enforce 30 point cap
      const total = Object.values(this.editing.masteries).reduce((s, v) => s + (v || 0), 0);
      if (total > 30) {
        this.editing.masteries[id] = Math.max(0, current - (total - 30));
      }
    },
    getSlotStyle(section, slot) {
      const page = this.runePages[this.currentRunePage];
      const runeId = page && page.runes ? page.runes[slot] : null;
      const hasRune = runeId && RUNE_DATA[runeId];
      return {
        background: hasRune ? section.color + "40" : "rgba(0,0,0,0.3)",
        borderColor: hasRune ? section.color : "rgba(255,255,255,0.15)",
        borderRadius: section.type === "quintessence" ? "50%" : "4px"
      };
    },
    getRuneName(pageIdx, slot) {
      const page = this.runePages[pageIdx];
      if (!page || !page.runes) return "";
      const runeId = page.runes[slot];
      return runeId && RUNE_DATA[runeId] ? RUNE_DATA[runeId].name : "";
    },
    getRuneStatText(pageIdx, slot) {
      const page = this.runePages[pageIdx];
      if (!page || !page.runes) return "";
      const runeId = page.runes[slot];
      if (!runeId || !RUNE_DATA[runeId]) return "";
      const stats = RUNE_DATA[runeId].stats;
      return Object.entries(stats).map(([k, v]) => {
        const name = STAT_NAMES[k] || k;
        const val = Math.abs(v);
        const sign = v > 0 ? "+" : "";
        return sign + (val < 1 ? (val * 100).toFixed(1) + "%" : val.toFixed(2)) + name;
      }).join(" ");
    },
    getEditingSlotsForType(type) {
      if (!this.editing || !this.editing.slots) return [];
      const slotRange = {
        mark: [1,9], seal: [10,18], glyph: [19,27], quintessence: [28,30]
      };
      const range = slotRange[type] || [1,9];
      return this.editing.slots.filter(s => s.slot >= range[0] && s.slot <= range[1]);
    },
    addRuneSlot(type) {
      if (!this.editing) return;
      const slotRange = { mark: [1,9], seal: [10,18], glyph: [19,27], quintessence: [28,30] };
      const range = slotRange[type] || [1,9];
      const usedSlots = this.editing.slots.filter(s => s.slot >= range[0] && s.slot <= range[1]).map(s => s.slot);
      const maxSlots = range[1] - range[0] + 1;
      if (usedSlots.length >= maxSlots) return;
      const nextSlot = range[0] + usedSlots.length;
      const defaultRuneId = this.getRunesForType(type)[0]?.id || 5245;
      this.editing.slots.push({ slot: nextSlot, runeId: defaultRuneId, count: 1 });
    },
    removeRuneSlot(type, idx) {
      if (!this.editing) return;
      const slotsForType = this.getEditingSlotsForType(type);
      if (slotsForType[idx]) {
        const slotEntry = slotsForType[idx];
        const globalIdx = this.editing.slots.indexOf(slotEntry);
        if (globalIdx >= 0) this.editing.slots.splice(globalIdx, 1);
      }
    },
    close() { this.$emit("close"); },
    selectRunePage(index) { this.$store.dispatch("selectRunePage", index); },
    selectMasteryPage(index) { this.$store.dispatch("selectMasteryPage", index); },
    addRunePage() {
      const newPage = {
        id: Date.now(), name: "新符文页",
        runes: { 1:5245,2:5245,3:5245,4:5245,5:5245,6:5245,7:5245,8:5245,9:5245,
                 10:5317,11:5317,12:5317,13:5317,14:5317,15:5317,16:5317,17:5317,18:5317,
                 19:5289,20:5289,21:5289,22:5289,23:5289,24:5289,25:5289,26:5289,27:5289,
                 28:5335,29:5335,30:5335 }
      };
      this.$store.dispatch("addRunePage", newPage);
    },
    editRunePage(index) {
      const page = { ...this.runePages[index] };
      const runes = page.runes || {};
      const slots = [];
      for (let slot = 1; slot <= 30; slot++) {
        slots.push({ slot, runeId: runes[slot] || null, count: 1 });
      }
      this.editing = { isRune: true, index, ...page, slots };
    },
    deleteRunePage(index) {
      if (confirm("确定要删除这个符文页吗？")) this.$store.dispatch("deleteRunePage", index);
    },
    addMasteryPage() {
      this.$store.dispatch("addMasteryPage", { id: Date.now(), name: "新天赋页", masteries: {} });
    },
    editMasteryPage(index) {
      const page = { ...this.masteryPages[index] };
      this.editing = { isRune: false, index, ...page, masteries: { ...(page.masteries || {}) } };
    },
    deleteMasteryPage(index) {
      if (confirm("确定要删除这个天赋页吗？")) this.$store.dispatch("deleteMasteryPage", index);
    },
    saveEdit() {
      if (this.editing.isRune) {
        const runes = {};
        this.editing.slots.forEach(s => { if (s.slot && s.runeId) runes[s.slot] = s.runeId; });
        this.$store.dispatch("editRunePage", { index: this.editing.index, page: { id: this.editing.id, name: this.editing.name, runes } });
      } else {
        this.$store.dispatch("editMasteryPage", { index: this.editing.index, page: { id: this.editing.id, name: this.editing.name, masteries: { ...this.editing.masteries } } });
      }
      this.editing = null;
    },
    cancelEdit() { this.editing = null; }
  }
};
</script>

<style scoped>
.runeMasteryEditor { position: fixed; top: 0; left: 0; width: 100%; height: 100%; z-index: 1000; display: flex; align-items: center; justify-content: center; }
.backdrop { position: absolute; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.75); cursor: pointer; }
.modal { position: relative; width: clamp(600px, 80vw, 850px); max-height: 85vh; background: linear-gradient(180deg, #1a1d26 0%, #0d1017 100%); border: 2px solid #3c73b4; border-radius: 8px; overflow: hidden; box-shadow: 0 0 40px rgba(60,115,180,0.3); display: flex; flex-direction: column; }
.header { display: flex; justify-content: space-between; align-items: center; padding: 12px 16px; background: linear-gradient(180deg, #2a3444 0%, #1a2230 100%); border-bottom: 1px solid #3c73b4; }
.header h2 { color: #e0e0e0; font-size: clamp(14px, 2vw, 18px); margin: 0; }
.closeBtn { background: none; border: none; color: #999; font-size: 18px; cursor: pointer; padding: 4px 8px; transition: color 0.2s; }
.closeBtn:hover { color: #ff6b6b; }
.tabs { display: flex; background: #141820; border-bottom: 1px solid #3c73b4; }
.tabs button { flex: 1; padding: 10px; background: none; border: none; color: #8892a8; font-size: clamp(11px, 1.3vw, 14px); cursor: pointer; transition: all 0.2s; border-bottom: 2px solid transparent; }
.tabs button.active { color: white; border-bottom-color: #3c73b4; background: rgba(60,115,180,0.1); }
.tabs button:hover { color: white; background: rgba(60,115,180,0.08); }
.tabIcon { margin-right: 6px; }
.content { padding: 0; flex: 1; overflow-y: auto; overflow-x: hidden; }
.runePageView, .masteryPageView { display: flex; min-height: 300px; max-height: 65vh; }
.pagesColumn { width: clamp(160px, 20vw, 220px); background: rgba(0,0,0,0.2); border-right: 1px solid rgba(60,115,180,0.2); display: flex; flex-direction: column; padding: 8px; gap: 6px; }
.pageList { flex: 1; overflow-y: auto; display: flex; flex-direction: column; gap: 4px; }
.pageList::-webkit-scrollbar { width: 3px; }
.pageList::-webkit-scrollbar-thumb { background: rgba(100,117,137,0.5); border-radius: 2px; }
.pageItem { display: flex; justify-content: space-between; align-items: center; padding: 8px 10px; background: rgba(60,115,180,0.08); border: 1px solid rgba(60,115,180,0.2); border-radius: 4px; cursor: pointer; transition: all 0.2s; }
.pageItem:hover { background: rgba(60,115,180,0.15); }
.pageItem.active { background: rgba(60,115,180,0.25); border-color: #3c73b4; }
.pageName { color: #e0e0e0; font-size: 12px; flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.actions { display: flex; gap: 4px; }
.actions button { background: none; border: 1px solid rgba(255,255,255,0.15); color: #999; font-size: 10px; padding: 2px 5px; border-radius: 3px; cursor: pointer; transition: all 0.2s; }
.actions button:hover { background: rgba(60,115,180,0.3); color: white; border-color: #3c73b4; }
.addBtn { display: flex; align-items: center; justify-content: center; gap: 6px; padding: 8px; background: none; border: 1px dashed rgba(60,115,180,0.5); color: #3c73b4; font-size: 12px; border-radius: 4px; cursor: pointer; transition: all 0.2s; }
.addBtn:hover { background: rgba(60,115,180,0.1); border-style: solid; }

/* Rune Preview */
.runePreview { flex: 1; padding: 12px; overflow-y: auto; }
.runePreview::-webkit-scrollbar { width: 3px; }
.runePreview::-webkit-scrollbar-thumb { background: rgba(100,117,137,0.5); border-radius: 2px; }
.runePreview h3 { color: #e0e0e0; margin: 0 0 10px 0; font-size: 14px; }
.runeGrid { display: flex; flex-direction: column; gap: 10px; }
.runeSection { background: rgba(0,0,0,0.2); border-radius: 6px; padding: 8px; }
.sectionHeader { display: flex; align-items: center; gap: 6px; padding-bottom: 6px; border-bottom: 1px solid rgba(255,255,255,0.1); margin-bottom: 6px; font-size: 12px; color: #ccc; }
.sectionIcon { font-size: 14px; }
.runeSlots { display: flex; flex-wrap: wrap; gap: 4px; }
.runeSlot { width: clamp(28px, 4vw, 36px); height: clamp(28px, 4vw, 36px); border: 1px solid; display: flex; flex-direction: column; align-items: center; justify-content: center; position: relative; }
.runeSymbol { font-size: 12px; color: white; }
.runeName { font-size: 5px; color: #aaa; text-align: center; line-height: 1.1; max-width: 30px; overflow: hidden; }

/* Mastery Preview */
.masteryPreview { flex: 1; padding: 12px; overflow-y: auto; }
.masteryPreview::-webkit-scrollbar { width: 3px; }
.masteryPreview::-webkit-scrollbar-thumb { background: rgba(100,117,137,0.5); border-radius: 2px; }
.masteryPreview h3 { color: #e0e0e0; margin: 0 0 6px 0; font-size: 14px; }
.masterySummary { display: flex; gap: 16px; margin-bottom: 10px; font-size: 13px; font-weight: bold; }
.masteryTreePreview { display: flex; flex-direction: column; gap: 8px; }
.treePreviewSection { background: rgba(0,0,0,0.2); border-radius: 4px; padding: 6px; border-left: 3px solid; }
.treePreviewHeader { font-size: 12px; font-weight: bold; margin-bottom: 4px; color: #ccc; }
.tierPreviewRow { display: flex; flex-wrap: wrap; gap: 3px; margin-bottom: 3px; }
.tierPreviewItem { font-size: 10px; padding: 2px 5px; background: rgba(0,0,0,0.3); border-radius: 3px; color: #888; border: 1px solid rgba(255,255,255,0.05); }
.tierPreviewItem.hasPoints { color: #ddd; border-color: rgba(255,255,255,0.2); background: rgba(60,115,180,0.15); }

/* Edit Panel */
.editPanel { padding: 12px; overflow-y: auto; max-height: 65vh; }
.editPanel::-webkit-scrollbar { width: 3px; }
.editPanel::-webkit-scrollbar-thumb { background: rgba(100,117,137,0.5); border-radius: 2px; }
.editHeader { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
.editHeader h3 { color: #e0e0e0; margin: 0; font-size: 14px; }
.editForm { display: flex; flex-direction: column; gap: 10px; }
.formGroup { display: flex; flex-direction: column; gap: 4px; }
.formGroup label { color: #8892a8; font-size: 11px; }
.formGroup input { padding: 6px; background: rgba(0,0,0,0.4); border: 1px solid rgba(60,115,180,0.3); border-radius: 4px; color: white; font-size: 12px; outline: none; transition: border-color 0.2s; }
.formGroup input:focus { border-color: #3c73b4; }

/* Rune Edit */
.runeEditGrid { display: flex; gap: 10px; flex-wrap: wrap; }
.runeEditSection { background: rgba(0,0,0,0.2); border-radius: 4px; padding: 6px; min-width: 160px; flex: 1; }
.runeEditSlots { display: flex; flex-direction: column; gap: 4px; }
.runeEditSlot { display: flex; align-items: center; gap: 4px; }
.slotLabel { color: #999; font-size: 10px; min-width: 36px; }
.runeSelect { flex: 1; background: #1a1d26; color: white; border: 1px solid #3c73b4; border-radius: 3px; padding: 3px 5px; font-size: 10px; outline: none; max-width: 180px; min-width: 0; }
.removeSlotBtn { background: none; border: none; color: #c53030; cursor: pointer; font-size: 12px; padding: 2px; }
.removeSlotBtn:hover { color: #ff6b6b; }
.addSlotBtn { display: flex; align-items: center; gap: 4px; background: none; border: 1px dashed rgba(60,115,180,0.4); color: #3c73b4; font-size: 10px; padding: 3px 6px; border-radius: 3px; cursor: pointer; }
.addSlotBtn:hover { background: rgba(60,115,180,0.1); border-style: solid; }

/* Mastery Edit */
.masteryEditorTree { display: flex; flex-direction: column; gap: 8px; max-height: 50vh; overflow-y: auto; }
.masteryEditorTree::-webkit-scrollbar { width: 3px; }
.masteryEditorTree::-webkit-scrollbar-thumb { background: rgba(100,117,137,0.5); border-radius: 2px; }
.masteryEditorSection { background: rgba(0,0,0,0.2); border-radius: 4px; padding: 6px; }
.masteryEditorHeader { font-size: 12px; font-weight: bold; margin-bottom: 4px; padding-bottom: 3px; border-bottom: 1px solid rgba(255,255,255,0.1); }
.masteryTierRow { display: flex; flex-wrap: wrap; gap: 4px; margin-bottom: 3px; }
.masteryTierItem { display: flex; align-items: center; gap: 3px; padding: 2px 6px; background: rgba(0,0,0,0.3); border-radius: 3px; border: 1px solid rgba(255,255,255,0.05); transition: all 0.2s; }
.masteryTierItem.hasPoints { background: rgba(60,115,180,0.1); border-color: rgba(60,115,180,0.3); }
.masteryTierItem.maxed { background: rgba(60,115,180,0.2); border-color: #3c73b4; }
.masteryName { color: #ccc; font-size: 10px; white-space: nowrap; }
.masteryInput { width: 28px; padding: 1px 3px; text-align: center; background: #0d1017; border: 1px solid #3c73b4; border-radius: 3px; color: white; font-size: 10px; }
.masteryMax { color: #999; font-size: 10px; }

/* Form Actions */
.formActions { display: flex; justify-content: flex-end; gap: 8px; margin-top: 8px; }
.saveBtn { background: linear-gradient(180deg, #3c73b4 0%, #20477e 100%); border: 1px solid #3c73b4; color: white; padding: 6px 16px; border-radius: 4px; cursor: pointer; font-size: 12px; transition: filter 0.2s; }
.saveBtn:hover { filter: brightness(1.15); }

/* Responsive: narrow screens */
@media (max-width: 900px) {
  .modal { width: clamp(400px, 95vw, 700px); }
  .runePageView, .masteryPageView { flex-direction: column; min-height: 200px; }
  .pagesColumn { width: 100%; max-height: 100px; border-right: none; border-bottom: 1px solid rgba(60,117,180,0.2); flex-direction: row; }
  .pageList { flex-direction: row; overflow-x: auto; overflow-y: hidden; }
  .pageItem { flex-shrink: 0; }
}

@media (max-height: 600px) {
  .modal { max-height: 95vh; }
  .runePageView, .masteryPageView { max-height: 55vh; }
  .editPanel { max-height: 55vh; }
  .masteryEditorTree { max-height: 35vh; }
}
</style>