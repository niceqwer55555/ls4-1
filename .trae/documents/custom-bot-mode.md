# 自定义对战添加人机模式（参考420 LOL登录器）

## Context

当前自定义对战模式（CUSTOM）不支持人机。`Play.vue` 中 CUSTOM 模式只有 BLIND/DRAFT/ARAM，没有 BOT 变体；`LobbyCustom.vue` 没有"添加机器人"UI；Java API 服务器 `Lobby` 没有机器人字段，`GameManagerService` 不会注入 AI 玩家。

项目记忆中的硬约束：
- AI 玩家在 `GameInfo.json` 中 `playerId` 必须为 `-1`，以激活 `ChampionAI` 脚本
- AI 角色由名字前缀决定（`Blue`/`Red` + 角色：Top/Jungle/Mid/ADC/Support）
- AI 阵容固定为 2-1-2 分路

用户确认的需求：两支队伍都可加机器人 / 需要难度选择（简单/普通/困难）/ 玩家手动选择英雄 / Java服务器+Vue登录器两端都改。

## 实现方案

### 一、Java API 服务器

#### 1. 新建文件

**`model/lobby/BotDifficulty.java`** — 机器人难度枚举
```java
public enum BotDifficulty { EASY, NORMAL, HARD }
```

**`model/lobby/LobbyBot.java`** — 机器人数据类
- 字段：`String botId`(UUID)、`String name`(如 `BlueTop`)、`String championId`、`BotDifficulty difficulty`、`LobbyTeam team`
- `name` 由 `team` + `role` 组合而成，符合项目记忆中 ChampionAI 脚本的角色识别规则

**`model/websocket/lobby/LobbyBotIn.java`** — 请求 DTO
- 字段：`String championId`、`String difficulty`、`String team`("TEAM1"/"TEAM2")、`String role`("Top"/"Jungle"/"Mid"/"ADC"/"Support")

**`model/websocket/lobby/LobbyBotOut.java`** — 响应 DTO
- 字段：`String botId`、`String name`、`String championId`、`String championDisplayName`、`BotDifficulty difficulty`、`LobbyTeam team`

#### 2. 修改文件

**[model/lobby/Lobby.java](file:///d:/game/ls4-1/ls4-api-server/src/main/java/de/jandev/ls4apiserver/model/lobby/Lobby.java)**
- 新增 `private List<LobbyBot> bots = new CopyOnWriteArrayList<>();`
- 在 `toString()` 中加入 bots

**[model/champselect/GameLobby.java](file:///d:/game/ls4-1/ls4-api-server/src/main/java/de/jandev/ls4apiserver/model/champselect/GameLobby.java)**
- 新增 `private List<LobbyBot> bots = new ArrayList<>();`
- 机器人不参与 `team1`/`team2`（避免影响 `setupLobby`/`updatePlayersWhoCanPickAndBan` 现有的选人/禁用流程）

**[model/websocket/lobby/LobbyMessageOut.java](file:///d:/game/ls4-1/ls4-api-server/src/main/java/de/jandev/ls4apiserver/model/websocket/lobby/LobbyMessageOut.java)**
- 新增 `private List<LobbyBotOut> bots = new ArrayList<>();`
- 构造函数中遍历 `lobby.getBots()` 转换为 `LobbyBotOut`

**[model/websocket/MessageType.java](file:///d:/game/ls4-1/ls4-api-server/src/main/java/de/jandev/ls4apiserver/model/websocket/MessageType.java)**
- 新增 `LOBBY_ADD_BOT, LOBBY_REMOVE_BOT`

**[service/LobbyService.java](file:///d:/game/ls4-1/ls4-api-server/src/main/java/de/jandev/ls4apiserver/service/LobbyService.java)**
- 新增 `addBot(Lobby lobby, User user, LobbyBotIn botIn)`：
  - `isOwnerOrThrowException` 校验
  - 校验目标队伍 `(members+bots)` 数量 `< teamSize`
  - 校验同队 `role` 不重复（避免两个 `BlueTop`）
  - 通过 `userCollectionService.getChampionByIdWithoutSpellsAndSkins(championId)` 验证英雄存在
  - 构造 `name = (team==TEAM1 ? "Blue" : "Red") + role`
  - 加入 `lobby.getBots()`
- 新增 `removeBot(Lobby lobby, User user, String botId)`：
  - `isOwnerOrThrowException` 校验
  - 从 `lobby.getBots()` 移除
- 修改 `changeLobbyType`：尺寸校验改为 `members.size() + bots.size() <= teamSize * 2`
- 修改 `acceptInvite`：尺寸校验改为 `members.size() + bots.size() < teamSize * 2`

**[websocket/handler/LobbyMessageHandler.java](file:///d:/game/ls4-1/ls4-api-server/src/main/java/de/jandev/ls4apiserver/websocket/handler/LobbyMessageHandler.java)**
- 新增 `handleLobbyAddBot(Lobby, User, LobbyBotIn)`：调用 `lobbyService.addBot`，发 `sendLobbyUpdate`
- 新增 `handleLobbyRemoveBot(Lobby, User, String botId)`：调用 `lobbyService.removeBot`，发 `sendLobbyUpdate`
- 修改 `createCustomGame`：把 `lobby.getBots()` 复制到 `team1`/`team2`（按 `LobbyTeam` 分组）
- 修改 `handleLobbyMatchmakingStart`：队伍尺寸校验改为 `team1Humans + team1Bots <= teamSize`、`team2Humans + team2Bots <= teamSize`

**[websocket/LobbySocketController.java](file:///d:/game/ls4-1/ls4-api-server/src/main/java/de/jandev/ls4apiserver/websocket/LobbySocketController.java)**
- 在 `incomingLobbyMessage` 的 switch 中新增：
  - `LOBBY_ADD_BOT` → 解析 `LobbyBotIn`，调 `handleLobbyAddBot`
  - `LOBBY_REMOVE_BOT` → 取 `botId`，调 `handleLobbyRemoveBot`

**[service/GameManagerService.java](file:///d:/game/ls4-1/ls4-api-server/src/main/java/de/jandev/ls4apiserver/service/GameManagerService.java)**
- 修改 `getGameConfig`：人类玩家循环之后，遍历 `gameLobby.getBots()` 注入 `Player`：
  - `playerId = -1`（关键：激活 ChampionAI）
  - `team = bot.getTeam()==TEAM1 ? "BLUE" : "RED"`
  - `name = bot.getName()`（如 `BlueTop`，供 AI 脚本识别角色）
  - `champion = bot.getChampionId()`
  - `summoner1 = SummonerSpell.SUMMONER_HEAL.getGameServerName()`、`summoner2 = SummonerSpell.SUMMONER_FLASH.getGameServerName()`
  - 其他字段（`skin=0`、`rank="DIAMOND"`、`ribbon=2`、`icon=0`、`runes=new Runes()`、`blowfishKey` 使用默认值）

### 二、Vue 登录器

#### 1. 修改 [src/store/utils/socketManager.js](file:///d:/game/ls4-1/ls4-launcher/src/store/utils/socketManager.js)

在 `sendLobbyMessage` 的 switch 中新增：
- `LOBBY_ADD_BOT` — payload 为 `{ data: { championId, difficulty, team, role } }`
- `LOBBY_REMOVE_BOT` — payload 为 `{ data: { botId } }`

#### 2. 修改 [src/views/LobbyCustom.vue](file:///d:/game/ls4-1/ls4-launcher/src/views/LobbyCustom.vue)

**模板层：**
- 在每队 `team` 块的 `inner` 内，玩家列表后增加：
  - 机器人列表（`v-for` 遍历 `bots1`/`bots2`），显示机器人卡片：英雄图标、名字（如 `BlueTop`）、难度标签、移除按钮（仅房主可见）
  - "添加机器人"按钮（仅房主可见且队伍未满）

- 新增"添加机器人"模态框：
  - 英雄选择网格（复用 `state.collection.champions`，调用 `getCollectionChampions` action 获取）
  - 角色下拉：Top/Jungle/Mid/ADC/Support（已选角色置灰）
  - 难度下拉：Easy/Normal/Hard
  - 确认/取消按钮

**脚本层：**
- `computed` 新增 `bots1`、`bots2`（从 `state.lobby.bots` 按 `team` 过滤）
- `methods` 新增：
  - `openBotModal(team)` — 打开模态框，加载英雄列表
  - `confirmAddBot()` — 调 `$socket.sendLobbyMessage("LOBBY_ADD_BOT", ...)`
  - `removeBot(botId)` — 调 `$socket.sendLobbyMessage("LOBBY_REMOVE_BOT", ...)`
- `canJoinTeam` 与队伍满判断逻辑：把 `bots1.length`/`bots2.length` 计入队伍人数

#### 3. 修改 [src/components/Lobby/Player.vue](file:///d:/game/ls4-1/ls4-launcher/src/components/Lobby/Player.vue)

- `props` 新增 `isBot: Boolean`、`botDifficulty: String`
- 模板中机器人卡片：去掉等级/好友按钮/踢出按钮，加机器人标识图标 + 难度色块；房主可点移除按钮（emit `remove` 事件由父组件处理）
- 或者新建 `src/components/Lobby/BotPlayer.vue` 单独渲染机器人（避免在 Player.vue 中加太多分支）— 推荐这种

#### 4. 无需改 `socketMutations.js`

`socketSetUserLobby` 直接 `state.lobby = data`，新返回的 `bots` 字段会自动落到 `state.lobby.bots`。

### 三、关键约束与不变式

| 约束 | 实现位置 |
|---|---|
| AI `playerId = -1` | `GameManagerService.getGameConfig` |
| AI 名字格式 `{Team}{Role}` | `LobbyService.addBot` 构造 `name` |
| AI 队伍前缀 Blue/Red | TEAM1→Blue, TEAM2→Red |
| 同队角色不重复 | `LobbyService.addBot` 校验 |
| 队伍人数 ≤ 5 | `addBot`/`changeLobbyType`/`acceptInvite`/`handleLobbyMatchmakingStart` 校验 |
| 机器人不进 champ select | 仅放入 `GameLobby.bots`，不进 `team1`/`team2` |

### 四、验证方法

1. **构建 Java 服务器**：`cd d:\game\ls4-1\ls4-api-server && mvn package -DskipTests`
2. **构建登录器**：`d:\game\ls4-1\BuildClient.bat`
3. **启动服务**：`run1-ApiServer.bat`、`run2-CdnServer.bat`、`run3-LobbyServer.bat`
4. **功能测试**：
   - 登录器进入 Play → 自定义对战 → 创建房间
   - 房主点"添加机器人" → 选英雄+角色+难度 → 确认，机器人出现在队伍列表
   - 给两支队伍各加机器人直到 5 人
   - 点开始游戏 → 进入 champ select → 选完英雄 → 进入游戏
   - 在游戏服务器 `Settings/GameInfo.json` 中确认 bot 玩家 `playerId=-1`、`name` 形如 `BlueTop`
   - 进游戏后确认 AI 正常活动（参考项目记忆中 2-1-2 分路）
5. **回归测试**：PVP 模式、COOPVSAI 模式仍可正常开局

### 五、文件改动清单

**新增（4 个）：**
- `ls4-api-server/src/main/java/de/jandev/ls4apiserver/model/lobby/BotDifficulty.java`
- `ls4-api-server/src/main/java/de/jandev/ls4apiserver/model/lobby/LobbyBot.java`
- `ls4-api-server/src/main/java/de/jandev/ls4apiserver/model/websocket/lobby/LobbyBotIn.java`
- `ls4-api-server/src/main/java/de/jandev/ls4apiserver/model/websocket/lobby/LobbyBotOut.java`

**修改 Java（7 个）：**
- `model/lobby/Lobby.java`
- `model/champselect/GameLobby.java`
- `model/websocket/lobby/LobbyMessageOut.java`
- `model/websocket/MessageType.java`
- `service/LobbyService.java`
- `service/GameManagerService.java`
- `websocket/handler/LobbyMessageHandler.java`
- `websocket/LobbySocketController.java`

**修改前端（3 个）：**
- `src/views/LobbyCustom.vue`
- `src/components/Lobby/Player.vue`（或新建 `BotPlayer.vue`）
- `src/store/utils/socketManager.js`
