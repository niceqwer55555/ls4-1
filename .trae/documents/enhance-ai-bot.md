# 增强 AI Bot 智能计划

## 背景

当前 AI bot（ChampionAI.cs）行为简单：只购买初始物品、只升级一次技能、目标选择基础、所有角色使用相同天赋。用户希望增强 AI 智能，包括合成物品、选天赋、提升战斗智商。

## 修改文件

1. [ChampionAI.cs](file:///D:/game/ls4-1/ls4-game-server/Gameserver/Content/LeagueSandbox-Scripts/AIScripts/ChampionAI.cs) - 核心AI逻辑
2. [GameManagerService.java](file:///D:/game/ls4-1/ls4-api-server/src/main/java/de/jandev/ls4apiserver/service/GameManagerService.java) - bot Runes 配置

## 实施步骤

### 步骤 1：修改 GameManagerService.java - 角色专属 Runes

在 `getGameConfig` 方法的 bot 注入循环中，将 `p.setRunes(new Runes())` 替换为根据角色设置不同 Runes：

```java
private Runes getRunesForRole(String name) {
    Runes r = new Runes(); // 默认 AD（5245/5317/5289/5335）
    if (name == null) return r;
    if (name.contains("Mid")) {  // AP 天赋
        // 9个法穿印记(5275) + 9个护甲封印(5317) + 9个魔抗刻纹(5289) + 3个法强精华(5337)
        r.setOne(5275); r.setTwo(5275); r.setThree(5275);
        r.setFour(5275); r.setFive(5275); r.setSix(5275);
        r.setSeven(5275); r.setEight(5275); r.setNine(5275);
        r.setTwentyEight(5337); r.setTwentyNine(5337); r.setThirty(5337);
    } else if (name.contains("Support")) {  // 坦克辅助天赋
        // 9个护甲印记(5317) + 9个护甲封印(5317) + 9个魔抗刻纹(5289) + 3个血量精华(5310)
        r.setOne(5317); r.setTwo(5317); r.setThree(5317);
        r.setFour(5317); r.setFive(5317); r.setSix(5317);
        r.setSeven(5317); r.setEight(5317); r.setNine(5317);
        r.setTwentyEight(5310); r.setTwentyNine(5310); r.setThirty(5310);
    }
    // Top/ADC/Jungle 保持默认 AD Runes
    return r;
}
```

修改 bot 循环中的 Runes 设置：`p.setRunes(getRunesForRole(bot.getName()));`

### 步骤 2：修改 ChampionAI.cs - 核心改进

#### 2a. 新增数据结构

```csharp
enum Role { Top, Jungle, Mid, ADC, Support, Unknown }
Role role;
List<int> buildOrder;     // 完整6件套构建路径
int buildIndex = 0;       // 下一件要买的物品
const float BASE_RADIUS = 1000f;
const float RECALL_GOLD_THRESHOLD = 800f;
```

#### 2b. 角色检测（OnActivate 中）

解析 `champion.Name`（如 "BlueTop"），缓存 Role 枚举，并初始化 `buildOrder`。

#### 2c. 物品构建路径

```csharp
List<int> GetBuildOrder(Role r) {
    switch(r) {
        case Role.Mid:    // AP
            return new List<int> { 1001, 3020, 1058, 3089, 3135, 3157, 3041, 3116 };
        case Role.Support: // Tank/Support
            return new List<int> { 1001, 3047, 2049, 3068, 3143, 3102, 3083, 3401 };
        default:          // Top/ADC/Jungle - AD
            return new List<int> { 1001, 3006, 1036, 3031, 3046, 3072, 3035, 3074 };
    }
}
```

#### 2d. 动态购买物品（替换 BuyStartingItems）

新方法 `TryBuyItems()`：
- 移除 `itemsBought` 一次性标志，每 tick 调用
- 若 `buildIndex >= buildOrder.Count` 则返回
- 若在基地（`IsAtBase()`），调用 `champion.Shop.HandleItemBuyRequest(buildOrder[buildIndex])`
- 检查 `champion.Inventory.GetAllItems()` 确认购买成功，成功则 `buildIndex++`
- `HandleItemBuyRequest` 金币不足时静默失败，安全

#### 2e. 自动升级技能（替换 LevelUpStartingSkill）

新方法 `AutoLevelSkills()`，每 tick 调用：
```csharp
static readonly Dictionary<Role, byte[]> skillPriority = new() {
    { Role.Top,     new byte[]{0,2,1,0,0,3,0,2,2,2,3,2,1,1,1,3,1,1} },
    { Role.Jungle,  new byte[]{2,0,1,2,2,3,2,0,0,0,3,0,1,1,1,3,1,1} },
    { Role.Mid,     new byte[]{0,2,1,0,0,3,0,1,1,1,3,1,2,2,2,3,2,2} },
    { Role.ADC,     new byte[]{1,0,2,1,1,3,1,0,0,0,3,0,2,2,2,3,2,2} },
    { Role.Support, new byte[]{2,0,1,2,2,3,2,1,1,1,3,1,0,0,0,3,0,0} },
};
// R(3) 在 6/11/16 级（数组索引 5/10/15）
```

```csharp
void AutoLevelSkills() {
    if (champion.SkillPoints <= 0) return;
    int idx = champion.Stats.Level - 1;
    if (idx < 0 || idx >= skillPriority[role].Length) return;
    byte slot = skillPriority[role][idx];
    if (champion.Spells.ContainsKey(slot)) champion.LevelUpSpell(slot);
}
```

#### 2f. 智能行为改进

**基地检测**：
```csharp
bool IsAtBase() {
    Vector2 basePos = champion.Team == TeamId.TEAM_BLUE 
        ? new Vector2(0, 1000) : new Vector2(13300, 14600);
    return Vector2.Distance(champion.Position, basePos) < BASE_RADIUS;
}
```

**回城购买逻辑**（在 UpdateBehavior 中，HP 检查后）：
```csharp
if (!IsAtBase() && champion.Stats.Gold >= RECALL_GOLD_THRESHOLD 
    && buildIndex < buildOrder.Count && hpPercent > 0.5f) {
    champion.Recall();
    return;
}
if (IsAtBase()) {
    TryBuyItems();
}
```

**目标选择改进**（ScanForTargets 中）：
- 扩大 `DETECT_RANGE` 从 700 到 900
- 使用复合评分：优先级 × 100 + 距离 × 0.1 - 目标血量百分比 × 50
- 选择最低分目标

**法力管理**（TryCastSpells 中，冷却检查后）：
```csharp
float manaCost = spell.SpellData.ManaCost[spell.CastInfo.SpellLevel];
if (champion.Stats.CurrentMana < manaCost) continue;
if (champion.Stats.CurrentMana / champion.Stats.ManaPoints.Total < 0.1f) continue;
```

**回城恢复**（UpdateBehavior 中）：
- 血量 < 25% 或法力 < 15% 且不在基地时，使用 `champion.Recall()` 代替走到基地
- 只在 `IsAtBase()` 时重置 `retreating` 标志

### 步骤 3：重构 OnUpdate

```csharp
public void OnUpdate(float diff) {
    if (champion == null || champion.IsDead) return;
    
    AutoLevelSkills();     // 每 tick 检查技能点
    TryBuyItems();         // 每 tick 尝试购买
    
    if (champion.IsAIPaused()) return;
    
    lastSpellCheckTime += diff / 1000f;
    stuckCheckTimer += diff / 1000f;
    UpdateBehavior(diff);
}
```

## 验证步骤

1. **编译 Java**：`cd d:\game\ls4-1\ls4-api-server && mvn package -DskipTests`
2. **部署 Java**：复制 jar 到 `server/` 目录
3. **重启 API 服务器**：关闭后运行 `run1-ApiServer.bat`
4. **测试 AI 行为**：
   - 创建自定义房间，添加不同角色的 bot
   - 开始游戏，观察：
     - AI 在每次升级时自动学习技能
     - AI 金币足够时回城购买高级物品
     - Mid bot 使用 AP 天赋，Support bot 使用坦克天赋
     - AI 目标选择更智能（优先低血量、近距离目标）
     - AI 低血量/低法力时回城恢复
5. **检查日志**：`d:\game\ls4-1\ls4-api-server\server\logs\ls-gameserver_2026-07-04.log` 确认无异常

## 注意事项

- 保持 2-1-2 分路不变
- 保持 bot 名称格式 `{Team}{Role}`
- 保持 GetPath() 寻路（防御塔避障）
- 保持同线 150 单位偏移
- HandleItemBuyRequest 金币不足时静默失败，无需额外检查
- 技能升级数组中 R(3) 只在 6/11/16 级（索引 5/10/15）
