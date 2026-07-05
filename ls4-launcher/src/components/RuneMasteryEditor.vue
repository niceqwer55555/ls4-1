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
        <button
          :class="{ active: activeTab == 'RUNES' }"
          @click="activeTab = 'RUNES'"
        >
          {{ t("RM_TAB_RUNES") }}
        </button>
        <button
          :class="{ active: activeTab == 'MASTERIES' }"
          @click="activeTab = 'MASTERIES'"
        >
          {{ t("RM_TAB_MASTERIES") }}
        </button>
      </div>
      <div class="content">
        <div class="pagesList" v-if="activeTab == 'RUNES'">
          <div
            v-for="(page, index) in runePages"
            :key="index"
            :class="{ active: currentRunePage == index }"
            class="pageItem"
            @click="selectRunePage(index)"
          >
            <span class="pageName">{{ page.name }}</span>
            <div class="actions">
              <button @click.stop="editRunePage(index)">
                <i class="fas fa-edit"></i>
              </button>
              <button @click.stop="deleteRunePage(index)" v-if="runePages.length > 1">
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
          <button class="addBtn" @click="addRunePage">
            <i class="fas fa-plus"></i> {{ t("RM_ADD_PAGE") }}
          </button>
        </div>
        <div class="pagesList" v-else>
          <div
            v-for="(page, index) in masteryPages"
            :key="index"
            :class="{ active: currentMasteryPage == index }"
            class="pageItem"
            @click="selectMasteryPage(index)"
          >
            <span class="pageName">{{ page.name }}</span>
            <div class="actions">
              <button @click.stop="editMasteryPage(index)">
                <i class="fas fa-edit"></i>
              </button>
              <button @click.stop="deleteMasteryPage(index)" v-if="masteryPages.length > 1">
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
          <button class="addBtn" @click="addMasteryPage">
            <i class="fas fa-plus"></i> {{ t("RM_ADD_PAGE") }}
          </button>
        </div>
      </div>
      <div class="editPanel" v-if="editing">
        <div class="editHeader">
          <h3>{{ editing.isRune ? t("RM_EDIT_RUNE") : t("RM_EDIT_MASTERY") }}</h3>
          <button class="cancelBtn" @click="cancelEdit">
            {{ t("RM_CANCEL") }}
          </button>
        </div>
        <div class="editForm">
          <div class="formGroup">
            <label>{{ t("RM_PAGE_NAME") }}</label>
            <input v-model="editing.name" type="text" />
          </div>
          <div class="formGroup" v-if="editing.isRune">
            <label>{{ t("RM_RUNE_SLOTS") }}</label>
            <div class="runeSlots">
              <div
                v-for="(slot, index) in editing.slots"
                :key="index"
                class="slotRow"
              >
                <select v-model="slot.runeId">
                  <option :value="5245">高级攻击力印记</option>
                  <option :value="5275">高级法术强度印记</option>
                  <option :value="5317">高级护甲印记</option>
                  <option :value="5250">高级魔法抗性印记</option>
                  <option :value="5231">高级攻击速度印记</option>
                  <option :value="5272">高级暴击率印记</option>
                  <option :value="5224">高级护甲穿透印记</option>
                  <option :value="5300">高级法力恢复符印</option>
                  <option :value="5302">高级魔法抗性符印</option>
                  <option :value="5327">高级生命恢复符印</option>
                  <option :value="5340">高级生命值符印</option>
                  <option :value="5341">高级移动速度符印</option>
                  <option :value="5289">高级魔法抗性雕文</option>
                  <option :value="5273">高级冷却缩减雕文</option>
                  <option :value="5265">高级法术强度雕文</option>
                  <option :value="5290">高级法力恢复雕文</option>
                  <option :value="5270">高级法力值雕文</option>
                  <option :value="5299">高级攻击速度雕文</option>
                  <option :value="5335">高级攻击力精华</option>
                  <option :value="5337">高级法术强度精华</option>
                  <option :value="5310">高级移动速度精华</option>
                  <option :value="5313">高级生命偷取精华</option>
                  <option :value="5357">高级护甲精华</option>
                  <option :value="5359">高级魔法抗性精华</option>
                  <option :value="5338">高级攻击速度精华</option>
                  <option :value="5342">高级生命值精华</option>
                  <option :value="5343">高级生命恢复精华</option>
                </select>
                <input v-model.number="slot.count" type="number" min="1" max="9" />
              </div>
              <button @click="editing.slots.push({ runeId: 5245, count: 1 })">
                <i class="fas fa-plus"></i>
              </button>
            </div>
          </div>
          <div class="formGroup" v-if="!editing.isRune">
            <label>{{ t("RM_TALENTS") }}</label>
            <div class="talentGrid">
              <div
                v-for="(rank, key) in editing.masteries"
                :key="key"
                class="talentItem"
              >
                <span class="talentId">{{ key }}</span>
                <input v-model.number="editing.masteries[key]" type="number" min="0" max="5" />
              </div>
            </div>
          </div>
          <div class="formActions">
            <button class="saveBtn" @click="saveEdit">
              {{ t("RM_SAVE") }}
            </button>
            <button class="cancelBtn" @click="cancelEdit">
              {{ t("RM_CANCEL") }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style>
.runeMasteryEditor {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
}

.runeMasteryEditor .backdrop {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.7);
  cursor: pointer;
}

.runeMasteryEditor .modal {
  position: relative;
  width: 700px;
  max-height: 80vh;
  background: linear-gradient(180deg, #1a1d26 0%, #0d1017 100%);
  border: 2px solid #3c73b4;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 0 30px rgba(60, 115, 180, 0.3);
}

.runeMasteryEditor .modal .header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 20px;
  background: linear-gradient(180deg, #2a3444 0%, #1a2230 100%);
  border-bottom: 1px solid #3c73b4;
}

.runeMasteryEditor .modal .header h2 {
  color: white;
  font-size: 18px;
  margin: 0;
}

.runeMasteryEditor .modal .header .closeBtn {
  background: none;
  border: none;
  color: white;
  font-size: 20px;
  cursor: pointer;
  padding: 5px 10px;
  transition: color 0.2s;
}

.runeMasteryEditor .modal .header .closeBtn:hover {
  color: #ff6b6b;
}

.runeMasteryEditor .modal .tabs {
  display: flex;
  background: #1a1d26;
  border-bottom: 1px solid #3c73b4;
}

.runeMasteryEditor .modal .tabs button {
  flex: 1;
  padding: 12px;
  background: none;
  border: none;
  color: #8892a8;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s;
  border-bottom: 2px solid transparent;
}

.runeMasteryEditor .modal .tabs button.active {
  color: white;
  border-bottom-color: #3c73b4;
  background: rgba(60, 115, 180, 0.1);
}

.runeMasteryEditor .modal .tabs button:hover {
  color: white;
  background: rgba(60, 115, 180, 0.1);
}

.runeMasteryEditor .modal .content {
  padding: 20px;
  max-height: 50vh;
  overflow-y: auto;
}

.runeMasteryEditor .modal .content .pagesList {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.runeMasteryEditor .modal .content .pagesList .pageItem {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 15px;
  background: rgba(60, 115, 180, 0.1);
  border: 1px solid rgba(60, 115, 180, 0.3);
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.runeMasteryEditor .modal .content .pagesList .pageItem:hover {
  background: rgba(60, 115, 180, 0.2);
}

.runeMasteryEditor .modal .content .pagesList .pageItem.active {
  background: rgba(60, 115, 180, 0.3);
  border-color: #3c73b4;
}

.runeMasteryEditor .modal .content .pagesList .pageItem .pageName {
  color: white;
  font-size: 14px;
}

.runeMasteryEditor .modal .content .pagesList .pageItem .actions {
  display: flex;
  gap: 8px;
}

.runeMasteryEditor .modal .content .pagesList .pageItem .actions button {
  background: none;
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
  font-size: 12px;
  padding: 5px 10px;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.runeMasteryEditor .modal .content .pagesList .pageItem .actions button:hover {
  background: rgba(60, 115, 180, 0.5);
  border-color: #3c73b4;
}

.runeMasteryEditor .modal .content .pagesList .addBtn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 12px;
  background: none;
  border: 1px dashed rgba(60, 115, 180, 0.5);
  color: #3c73b4;
  font-size: 14px;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.runeMasteryEditor .modal .content .pagesList .addBtn:hover {
  background: rgba(60, 115, 180, 0.1);
  border-style: solid;
}

.runeMasteryEditor .modal .content .editorForm {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.runeMasteryEditor .modal .content .editorForm .formGroup {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.runeMasteryEditor .modal .content .editorForm .formGroup label {
  color: #8892a8;
  font-size: 12px;
}

.runeMasteryEditor .modal .content .editorForm .formGroup input {
  padding: 10px;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(60, 115, 180, 0.3);
  border-radius: 4px;
  color: white;
  font-size: 14px;
  outline: none;
  transition: border-color 0.2s;
}

.runeMasteryEditor .modal .content .editorForm .formGroup input:focus {
  border-color: #3c73b4;
}

.runeMasteryEditor .modal .content .editorForm .formGroup .runeSlots,
.runeMasteryEditor .modal .content .editorForm .formGroup .talents {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.runeMasteryEditor .modal .content .editorForm .formGroup .runeSlots .slotItem,
.runeMasteryEditor .modal .content .editorForm .formGroup .talents .talentItem {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(60, 115, 180, 0.3);
  border-radius: 4px;
}

.runeMasteryEditor .modal .content .editorForm .formGroup .runeSlots .slotItem span,
.runeMasteryEditor .modal .content .editorForm .formGroup .talents .talentItem span {
  color: #8892a8;
  font-size: 12px;
}

.runeMasteryEditor .modal .content .editorForm .formGroup .runeSlots .slotItem input,
.runeMasteryEditor .modal .content .editorForm .formGroup .talents .talentItem input {
  width: 50px;
  padding: 5px;
  text-align: center;
}

.runeMasteryEditor .modal .content .editorForm .formActions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 10px;
}

.runeMasteryEditor .modal .content .editorForm .formActions button {
  padding: 10px 20px;
  border-radius: 4px;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s;
}

.runeMasteryEditor .modal .content .editorForm .formActions .saveBtn {
  background: linear-gradient(180deg, #3c73b4 0%, #20477e 100%);
  border: 1px solid #3c73b4;
  color: white;
}

.runeMasteryEditor .modal .content .editorForm .formActions .saveBtn:hover {
  filter: brightness(1.1);
}

.runeMasteryEditor .modal .content .editorForm .formActions .cancelBtn {
  background: none;
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: white;
}

.runeMasteryEditor .modal .content .editorForm .formActions .cancelBtn:hover {
  background: rgba(255, 255, 255, 0.1);
}
</style>

<script>
import { mapState } from "vuex";

export default {
  props: {
    visible: Boolean
  },
  data() {
    return {
      activeTab: "RUNES",
      editing: null
    };
  },
  methods: {
    close() {
      this.$emit("close");
    },
    selectRunePage(index) {
      this.$store.dispatch("selectRunePage", index);
    },
    selectMasteryPage(index) {
      this.$store.dispatch("selectMasteryPage", index);
    },
    addRunePage() {
      const newPage = {
        id: Date.now(),
        name: "新符文页",
        runes: {
          1: 5245, 2: 5245, 3: 5245, 4: 5245, 5: 5245, 6: 5245, 7: 5245, 8: 5245, 9: 5245,
          10: 5273, 11: 5273, 12: 5273, 13: 5273, 14: 5273, 15: 5273, 16: 5273, 17: 5273, 18: 5273,
          19: 5289, 20: 5289, 21: 5289, 22: 5289, 23: 5289, 24: 5289, 25: 5289, 26: 5289, 27: 5289,
          28: 5305, 29: 5305, 30: 5305
        }
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
      this.editing = {
        isRune: true,
        index,
        ...page,
        slots
      };
    },
    deleteRunePage(index) {
      if (confirm("确定要删除这个符文页吗？")) {
        this.$store.dispatch("deleteRunePage", index);
      }
    },
    addMasteryPage() {
      const newPage = {
        id: Date.now(),
        name: "新天赋页",
        masteries: {}
      };
      this.$store.dispatch("addMasteryPage", newPage);
    },
    editMasteryPage(index) {
      const page = { ...this.masteryPages[index] };
      const masteries = page.masteries || {};
      this.editing = {
        isRune: false,
        index,
        ...page,
        masteries
      };
    },
    deleteMasteryPage(index) {
      if (confirm("确定要删除这个天赋页吗？")) {
        this.$store.dispatch("deleteMasteryPage", index);
      }
    },
    saveEdit() {
      if (this.editing.isRune) {
        const runes = {};
        this.editing.slots.forEach(slotEntry => {
          if (slotEntry.slot && slotEntry.runeId) {
            runes[slotEntry.slot] = slotEntry.runeId;
          }
        });
        this.$store.dispatch("editRunePage", {
          index: this.editing.index,
          page: {
            id: this.editing.id,
            name: this.editing.name,
            runes
          }
        });
      } else {
        const masteries = { ...this.editing.masteries };
        this.$store.dispatch("editMasteryPage", {
          index: this.editing.index,
          page: {
            id: this.editing.id,
            name: this.editing.name,
            masteries
          }
        });
      }
      this.editing = null;
    },
    cancelEdit() {
      this.editing = null;
    }
  },
  computed: {
    t(key) {
      return this.$translate.text(key);
    },
    ...mapState({
      runePages: state => state.runePages,
      masteryPages: state => state.masteryPages,
      currentRunePage: state => state.currentRunePage,
      currentMasteryPage: state => state.currentMasteryPage
    })
  }
};
</script>

<style lang="css" scoped>
.runeMasteryEditor {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1000;
  display: flex;
  justify-content: center;
  align-items: center;
}

.backdrop {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.7);
}

.modal {
  position: relative;
  background: #2c2c2c;
  border: 2px solid #4a4a4a;
  border-radius: 8px;
  width: 600px;
  max-height: 80vh;
  overflow: hidden;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 20px;
  background: #1a1a1a;
  border-bottom: 2px solid #4a4a4a;
}

.header h2 {
  color: #fff;
  margin: 0;
  font-size: 18px;
}

.closeBtn {
  background: none;
  border: none;
  color: #fff;
  font-size: 20px;
  cursor: pointer;
}

.tabs {
  display: flex;
  border-bottom: 2px solid #4a4a4a;
}

.tabs button {
  flex: 1;
  padding: 12px;
  background: #333;
  border: none;
  color: #999;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s;
}

.tabs button.active {
  background: #4a4a4a;
  color: #fff;
}

.content {
  padding: 15px;
  max-height: 400px;
  overflow-y: auto;
}

.pagesList {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.pageItem {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 15px;
  background: #333;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.2s;
}

.pageItem:hover {
  background: #444;
}

.pageItem.active {
  background: #4a90d9;
}

.pageName {
  color: #fff;
  font-size: 14px;
}

.actions {
  display: flex;
  gap: 8px;
}

.actions button {
  background: none;
  border: none;
  color: #999;
  cursor: pointer;
  font-size: 14px;
  padding: 5px;
}

.actions button:hover {
  color: #fff;
}

.addBtn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 12px;
  background: #4a90d9;
  border: none;
  border-radius: 4px;
  color: #fff;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s;
}

.addBtn:hover {
  background: #3a7ac9;
}

.editPanel {
  margin-top: 15px;
  padding-top: 15px;
  border-top: 2px solid #4a4a4a;
}

.editHeader {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

.editHeader h3 {
  color: #fff;
  margin: 0;
  font-size: 16px;
}

.cancelBtn {
  background: #555;
  border: none;
  color: #fff;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
}

.editForm {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.formGroup {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.formGroup label {
  color: #999;
  font-size: 13px;
}

.formGroup input,
.formGroup select {
  padding: 8px;
  background: #333;
  border: 1px solid #555;
  border-radius: 4px;
  color: #fff;
  font-size: 14px;
}

.runeSlots {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.slotRow {
  display: flex;
  gap: 10px;
}

.slotRow select {
  flex: 1;
}

.slotRow input {
  width: 60px;
}

.talentGrid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 10px;
}

.talentItem {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px;
  background: #333;
  border-radius: 4px;
}

.talentId {
  color: #999;
  font-size: 12px;
}

.formActions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}

.saveBtn {
  background: #4a90d9;
  border: none;
  color: #fff;
  padding: 8px 20px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.saveBtn:hover {
  background: #3a7ac9;
}
</style>