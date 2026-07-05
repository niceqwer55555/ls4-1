# 自定义对战机器人模式 - 前端 UI 实现计划

## 摘要

为 LOL 4.20 私服项目补全"自定义对战机器人模式"的剩余工作。后端 Java API、WebSocket 协议、前端 `socketManager.js` 与 `BotPlayer.vue` 组件均已完成。本计划聚焦于：修改 `LobbyCustom.vue` 集成机器人 UI、补充翻译键、构建并验证。

## 当前状态分析

### 已完成（前期会话）
- **后端 Java**：`BotDifficulty`、`LobbyBot`、`LobbyBotIn`、`LobbyBotOut` 模型；`Lobby.bots`、`GameLobby.bots`、`LobbyMessageOut.bots` 字段；`LobbyService.addBot/removeBot` 含校验（owner/custom/team size/role uniqueness/champion 存在性）；`LobbyMessageHandler` 创建/移除/复制机器人；`LobbySocketController` 路由 `LOBBY_ADD_BOT`/`LOBBY_REMOVE_BOT`；`GameManagerService` 注入 `playerId=-1` 触发 ChampionAI。
- **前端**：`socketManager.js` 已加 `LOBBY_ADD_BOT`/`LOBBY_REMOVE_BOT` case；`BotPlayer.vue` 已创建（显示头像/名称/英雄/难度徽章/移除按钮）。
- **状态自动同步**：`socketMutations.socketSetUserLobby` 执行 `state.lobby = data`，后端推送的 `bots` 数组会自动写入 store。

### 数据契约（前端必须遵守）
- `LOBBY_ADD_BOT` 发送 payload：`{ data: { championId, difficulty, team, role } }`
  - `championId`：字符串（如 `"Ezreal"`，来自 `state.collection.champions[*].id`）
  - `difficulty`：`"EASY"` | `"NORMAL"` | `"HARD"`
  - `team`：`"TEAM1"` | `"TEAM2"`
  - `role`：`"Top"` | `"Jungle"` | `"Mid"` | `"ADC"` | `"Support"`（首字母大写，与服务端 `Set.of("Top","Jungle","Mid","ADC","Support")` 一致）
- `LOBBY_REMOVE_BOT` 发送 payload：`{ data: botId }`（botId 为字符串 UUID）
- 服务端校验：每队 human+bots < teamSize；同队 role 唯一。

### 关键文件清单
| 文件 | 状态 |
|------|------|
| `d:\game\ls4-1\ls4-launcher\src\views\LobbyCustom.vue` | 待修改 |
| `d:\game\ls4-1\ls4-launcher\public\static\lang\zh_CN.json` | 待加翻译键 |
| `d:\game\ls4-1\ls4-launcher\public\static\lang\en_US.json` | 待加翻译键 |
| `d:\game\ls4-1\ls4-launcher\src\components\Lobby\BotPlayer.vue` | 已完成（无需改动） |
| `d:\game\ls4-1\ls4-launcher\src\store\utils\socketManager.js` | 已完成（无需改动） |

## 拟定改动

### 1. 修改 `LobbyCustom.vue`

#### 1.1 Template 改动
- 在每个 `team` 块的 `<div class="inner">` 内，`<Player v-for>` 之后、`joinTeam` 之前，插入机器人列表与"添加机器人"按钮：
  - `<BotPlayer v-for="(bot, index) in bots1" :key="bot.botId" :bot="bot" :canRemove="owner" @remove="removeBot" />`（队伍 2 用 `bots2`）
  - 添加按钮：`<div class="addBot" v-if="canAddBot(1)"><button @click="openBotModal(1)">+ {{ t("CTLOBBY_ADD_BOT_BTN") }}</button></div>`（队伍 2 传 2）
- 在 `#LobbyCustom` 根 div 内末尾追加**内联机器人模态框**（局部模态框，不走全局 `$modal`，因为需要持有 team/champion/role/difficulty 状态，且仅在自定义大厅使用）：
  - 模态框结构：`v-if="botModalVisible"`，含 head/foot，body 内含英雄网格 + 角色下拉 + 难度下拉。
  - 英雄网格：`v-for` 遍历 `state.collection.champions`，点击高亮选中（参考 `SummonerIconModal.vue` 的 `.iconlist`/`.selected` 模式与 `ChampionIcon.vue` 的图片 URL 构造）。
  - 角色下拉：`<select v-model="botForm.role">`，选项 `Top/Jungle/Mid/ADC/Support`（值用英文，显示用翻译键 `BOT_ROLE_*`）。
  - 难度下拉：`<select v-model="botForm.difficulty">`，选项 `EASY/NORMAL/HARD`，显示用 `BOT_DIFFICULTY_*`。
  - 确认按钮：`@click="confirmAddBot"`；取消按钮：`@click="closeBotModal"`。

#### 1.2 Script 改动
- **import**：`import BotPlayer from "@/components/Lobby/BPlayer.vue";`
- **components**：注册 `BotPlayer`。
- **data()** 新增：
  ```js
  botModalVisible: false,
  botForm: {
    team: null,        // 1 或 2
    championId: null,
    role: "Top",
    difficulty: "NORMAL"
  }
  ```
- **computed** 新增（在现有 `mapState` 块内）：
  ```js
  bots1: state => state.lobby.bots ? state.lobby.bots.filter(b => b.team === "TEAM1") : [],
  bots2: state => state.lobby.bots ? state.lobby.bots.filter(b => b.team === "TEAM2") : [],
  champions: state => state.collection.champions || []
  ```
- **methods** 新增：
  - `canAddBot(team)`：仅房主（`this.owner`）且非队列中（`!this.inQueue`）且队伍未满时返回 true。逻辑：
    ```js
    if (!this.owner || this.inQueue) return false;
    const humans = team === 1 ? this.players1.length : this.players2.length;
    const bots = team === 1 ? this.bots1.length : this.bots2.length;
    return humans + bots < 5; // 自定义默认 5v5
    ```
  - `openBotModal(team)`：设置 `botForm.team = team`，重置 `championId=null/role="Top"/difficulty="NORMAL"`，若 `state.collection.champions` 为空则 `dispatch("getCollectionChampions")`，设 `botModalVisible = true`。
  - `closeBotModal()`：`botModalVisible = false`。
  - `confirmAddBot()`：校验 `championId` 已选；构造 payload `{ data: { championId, difficulty, role, team: botForm.team === 1 ? "TEAM1" : "TEAM2" } }`；调用 `this.$socket.sendLobbyMessage("LOBBY_ADD_BOT", payload, flyback)`；flyback 中无 error 则 `closeBotModal()`。
  - `removeBot(botId)`：调用 `this.$socket.sendLobbyMessage("LOBBY_REMOVE_BOT", { data: botId }, flyback)`。
  - `isChampionSelected(champId)`：`return this.botForm.championId === champId;`
  - `selectChampion(champId)`：`this.botForm.championId = champId;`
  - `getChampionImgUrl(champId)`：复用 `${host}:${port}/champions/${champId}.jpg`。
- **canJoinTeam(team)** 更新：原 `players2.length != 5` 改为 `players2.length + bots2.length < 5`（队伍 1 同理），保持人 + 机器人总数不超过 5。
- **startQueue** 无需改动（后端 `handleLobbyMatchmakingStart` 已含机器人计数校验）。

#### 1.3 Style 改动
- 新增 `.addBot` 按钮样式，复用现有 `.joinTeam button` 的渐变背景配色，保持视觉一致。
- 新增 `.botModal` 局部模态框样式，参考 `SummonerIconModal.vue` 的 `.modal` 定位（absolute, 居中, z-index 10）。
- 英雄网格复用 `.iconlist`/`.icon`/`.selected` 模式。

### 2. 翻译键（zh_CN.json + en_US.json）

在 `CTLOBBY_START_GAME_BTN` 行之后追加：
```json
"CTLOBBY_ADD_BOT_BTN": "添加机器人",
"BOT_DIFFICULTY_EASY": "简单",
"BOT_DIFFICULTY_NORMAL": "普通",
"BOT_DIFFICULTY_HARD": "困难",
"BOT_ROLE_TOP": "上单",
"BOT_ROLE_JUNGLE": "打野",
"BOT_ROLE_MID": "中单",
"BOT_ROLE_ADC": "射手",
"BOT_ROLE_SUPPORT": "辅助",
"BOT_MODAL_TITLE": "添加机器人",
"BOT_MODAL_SELECT_CHAMPION": "选择英雄",
"BOT_MODAL_SELECT_ROLE": "选择位置",
"BOT_MODAL_SELECT_DIFFICULTY": "选择难度",
"BOT_MODAL_CONFIRM_BTN": "确认",
"BOT_MODAL_CANCEL_BTN": "取消"
```
en_US.json 同位置加对应英文。

### 3. 构建与验证

#### 3.1 后端构建（确认无回归）
```bash
cd d:\game\ls4-1\ls4-api-server && mvn package -DskipTests
```

#### 3.2 启动器构建
```bash
d:\game\ls4-1\BuildClient.bat
```

#### 3.3 功能验证清单
1. 启动 API server 与启动器，登录后创建自定义大厅。
2. 房主点击"添加机器人"按钮，模态框弹出，英雄网格显示。
3. 选英雄 + 位置 + 难度，确认后机器人出现在对应队伍列表，名称为 `Blue{Role}` 或 `Red{Role}`。
4. 重复添加直至队伍满（人 + 机器人 = 5），"添加机器人"按钮消失。
5. 点击机器人移除按钮，机器人从列表移除。
6. 非房主看不到添加/移除按钮。
7. 点击"开始游戏"，进入 champ select（机器人不显示在 champ select，但其英雄已预定），完成 champ select 后游戏服务器启动，`GameInfo.json` 中机器人 `playerId=-1`。
8. 进入游戏后机器人由 ChampionAI 控制行走/施法/对线（2-1-2 分线）。

## 假设与决策
- **局部模态框 vs 全局模态框**：选局部。机器人模态框仅在大厅自定义页面使用，且需持有表单状态，嵌入 `LobbyCustom.vue` 内更简洁，无需触碰 `plugins/modals.js` 与 `App.vue`。
- **champions 数据源**：复用 `state.collection.champions`（与 champ select 同源）。打开模态框时若为空则懒加载 `getCollectionChampions`。
- **teamSize 硬编码 5**：自定义大厅默认 5v5（SUMMONERS_RIFT_DRAFT/BLIND/ARAM_BLIND）。若后续支持 3v3（TWISTED_TREELINE），需改为读取 `lobby.lobbyType` 的 teamSize；当前项目无该 getter 暴露给前端，暂用 5。
- **角色选项**：含 Jungle，但 ChampionAI 实际分线为 2-1-2（无打野）。Jungle 角色名仅用于机器人命名 `BlueJungle`，AI 脚本如何处理该名称由服务端 ChampionAI 决定，前端不干预。
- **不改动 `BotPlayer.vue`**：已创建完成，本次只引用。

## 验证步骤
- [ ] `LobbyCustom.vue` 编译无报错（启动器构建通过）
- [ ] 翻译键在两份 json 文件中均存在且 JSON 格式合法
- [ ] 后端 `mvn package` 成功
- [ ] 手动流程 1-8 全部通过
