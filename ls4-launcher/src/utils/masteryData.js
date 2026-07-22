/**
 * Complete LoL S4 Mastery/Talent Tree Data (2014 Season)
 * Correct 2014 preseason mastery tree with 3 trees of 6 tiers each
 * Trees: offense (42xx), defense (41xx), utility (43xx)
 * Point requirements per tier: 0/4/8/12/16/20
 */

export const MASTERY_TREES = {
  offense: {
    name: "攻击",
    color: "#c53030",
    maxPoints: 21,
    tiers: [
      { requiredPoints: 0, masteries: ["4211", "4212", "4213", "4214"] },
      { requiredPoints: 4, masteries: ["4221", "4222", "4224"] },
      { requiredPoints: 8, masteries: ["4231", "4232", "4233", "4234"] },
      { requiredPoints: 12, masteries: ["4241", "4242", "4243", "4244"] },
      { requiredPoints: 16, masteries: ["4251", "4252", "4253"] },
      { requiredPoints: 20, masteries: ["4262"] },
    ]
  },
  defense: {
    name: "防御",
    color: "#2b6cb0",
    maxPoints: 21,
    tiers: [
      { requiredPoints: 0, masteries: ["4111", "4112", "4113", "4114"] },
      { requiredPoints: 4, masteries: ["4121", "4122", "4123", "4124"] },
      { requiredPoints: 8, masteries: ["4131", "4132", "4133", "4134"] },
      { requiredPoints: 12, masteries: ["4141", "4142", "4143", "4144"] },
      { requiredPoints: 16, masteries: ["4151", "4152", "4154"] },
      { requiredPoints: 20, masteries: ["4162"] },
    ]
  },
  utility: {
    name: "通用",
    color: "#2f855a",
    maxPoints: 21,
    tiers: [
      { requiredPoints: 0, masteries: ["4311", "4312", "4313", "4314"] },
      { requiredPoints: 4, masteries: ["4321", "4322", "4323", "4324"] },
      { requiredPoints: 8, masteries: ["4331", "4332", "4333", "4334"] },
      { requiredPoints: 12, masteries: ["4341", "4342", "4343", "4344"] },
      { requiredPoints: 16, masteries: ["4352", "4353"] },
      { requiredPoints: 20, masteries: ["4362"] },
    ]
  },
};

const MASTERY_DATA = {
  // === OFFENSE TREE ===
  // Tier 1 (0 points required)
  4211: { name: "双刃剑", desc: "近战：造成2%额外伤害，承受1%额外伤害;远程：造成1.5%额外伤害，承受1.5%额外伤害", tree: "offense", tier: 1, col: 1, maxRanks: 1 },
  4212: { name: "狂暴", desc: "攻击速度+1.25/2.5/3.75/5%", tree: "offense", tier: 1, col: 2, maxRanks: 4 },
  4213: { name: "巫术", desc: "法术和技能伤害+1.25/2.5/3.75/5%", tree: "offense", tier: 1, col: 3, maxRanks: 4 },
  4214: { name: "屠夫", desc: "对野怪造成2/4%额外伤害", tree: "offense", tier: 1, col: 4, maxRanks: 2 },

  // Tier 2 (4 points required)
  4221: { name: "盛宴", desc: "击杀单位回复2/3生命值和1/2法力值", tree: "offense", tier: 2, col: 1, maxRanks: 2 },
  4222: { name: "蛮力", desc: "攻击力+2/4/6", tree: "offense", tier: 2, col: 2, maxRanks: 3 },
  4224: { name: "思想之力", desc: "法术强度+2/4/6", tree: "offense", tier: 2, col: 3, maxRanks: 3 },

  // Tier 3 (8 points required)
  4231: { name: "武术精通", desc: "攻击力+1.5/3", tree: "offense", tier: 3, col: 1, maxRanks: 2 },
  4232: { name: "法术编织", desc: "技能命中英雄后，普攻+1%伤害", tree: "offense", tier: 3, col: 2, maxRanks: 1 },
  4233: { name: "奥术精通", desc: "法术穿透+2/4%", tree: "offense", tier: 3, col: 3, maxRanks: 2 },
  4234: { name: "咒刃编织", desc: "普攻命中英雄后，技能+1%伤害", tree: "offense", tier: 3, col: 4, maxRanks: 1 },

  // Tier 4 (12 points required)
  4241: { name: "死神", desc: "对生命值低于50%的英雄造成2/3.5%额外伤害", tree: "offense", tier: 4, col: 1, maxRanks: 2 },
  4242: { name: "危险游戏", desc: "击杀或助攻回复5%已损失生命值和法力值", tree: "offense", tier: 4, col: 2, maxRanks: 1 },
  4243: { name: "毁灭攻势", desc: "攻击力+1.5/3/4.5，法术穿透+1.5/3/4.5%", tree: "offense", tier: 4, col: 3, maxRanks: 3 },
  4244: { name: "浩劫", desc: "造成3%额外伤害", tree: "offense", tier: 4, col: 4, maxRanks: 1 },

  // Tier 5 (16 points required)
  4251: { name: "狂战之怒", desc: "暴击后攻击速度+5/10/15%，持续3秒", tree: "offense", tier: 5, col: 1, maxRanks: 3 },
  4252: { name: "法术编织", desc: "技能命中英雄后，普攻+2%伤害(升级版)", tree: "offense", tier: 5, col: 2, maxRanks: 1 },
  4253: { name: "咒刃编织", desc: "普攻命中英雄后，技能+2%伤害(升级版)", tree: "offense", tier: 5, col: 3, maxRanks: 1 },

  // Tier 6 (20 points required) - Keystone
  4262: { name: "领主之令", desc: "普攻对英雄造成6%已损失生命值的额外物理伤害", tree: "offense", tier: 6, col: 1, maxRanks: 1 },

  // === DEFENSE TREE ===
  // Tier 1 (0 points required)
  4111: { name: "格挡", desc: "减少来自英雄的普攻伤害1/2", tree: "defense", tier: 1, col: 1, maxRanks: 2 },
  4112: { name: "愈合", desc: "生命回复+1/2每5秒", tree: "defense", tier: 1, col: 2, maxRanks: 2 },
  4113: { name: "不屈", desc: "减少来自小兵和野怪的伤害1/2", tree: "defense", tier: 1, col: 3, maxRanks: 2 },
  4114: { name: "老兵伤痕", desc: "生命值+30", tree: "defense", tier: 1, col: 4, maxRanks: 1 },

  // Tier 2 (4 points required)
  4121: { name: "压迫", desc: "对移动受限(减速/定身/眩晕)的目标造成2.5/5%额外伤害", tree: "defense", tier: 2, col: 1, maxRanks: 2 },
  4122: { name: "硬化皮肤", desc: "减少来自英雄的普攻伤害2/3.5", tree: "defense", tier: 2, col: 2, maxRanks: 2 },
  4123: { name: "刃甲", desc: "对攻击你的英雄造成6伤害", tree: "defense", tier: 2, col: 3, maxRanks: 1 },
  4124: { name: "灵敏", desc: "减速效果减少10/20%", tree: "defense", tier: 2, col: 4, maxRanks: 2 },

  // Tier 3 (8 points required)
  4131: { name: "耐久", desc: "生命回复+1/2/3每5秒，生命值低于25%时回复效果翻倍", tree: "defense", tier: 3, col: 1, maxRanks: 3 },
  4132: { name: "坚韧", desc: "减速效果减少5/10/15%", tree: "defense", tier: 3, col: 2, maxRanks: 3 },
  4133: { name: "符能盾甲", desc: "护盾效果+1.5/3%", tree: "defense", tier: 3, col: 3, maxRanks: 2 },
  4134: { name: "军旅之速", desc: "脱离战斗后+5%移速", tree: "defense", tier: 3, col: 4, maxRanks: 1 },

  // Tier 4 (12 points required)
  4141: { name: "传奇卫士", desc: "附近每名敌方英雄+1/2/3护甲和魔抗", tree: "defense", tier: 4, col: 1, maxRanks: 3 },
  4142: { name: "护卫", desc: "向友方英雄移动时+1/2/3%移速", tree: "defense", tier: 4, col: 2, maxRanks: 3 },
  4143: { name: "复苏之风", desc: "生命值低于25%时+5%生命回复", tree: "defense", tier: 4, col: 3, maxRanks: 1 },
  4144: { name: "坚毅", desc: "+5%额外生命值", tree: "defense", tier: 4, col: 4, maxRanks: 1 },

  // Tier 5 (16 points required)
  4151: { name: "史诗级守卫", desc: "对减速/禁锢/嘲讽/恐惧/魅惑/眩晕/压制效果减少15%", tree: "defense", tier: 5, col: 1, maxRanks: 1 },
  4152: { name: "符能盾甲", desc: "护盾值+8%", tree: "defense", tier: 5, col: 2, maxRanks: 1 },
  4154: { name: "传奇守卫", desc: "附近每名敌方英雄+4护甲和魔抗", tree: "defense", tier: 5, col: 3, maxRanks: 1 },

  // Tier 6 (20 points required) - Keystone
  4162: { name: "顽石誓约", desc: "减少4%所有伤害;被硬控时减少8%伤害;周围有友方英雄时，替其承受4%伤害", tree: "defense", tier: 6, col: 1, maxRanks: 1 },

  // === UTILITY TREE ===
  // Tier 1 (0 points required)
  4311: { name: "相位行走", desc: "离开战斗后+0.5/1%移动速度", tree: "utility", tier: 1, col: 1, maxRanks: 2 },
  4312: { name: "飞毛腿", desc: "移动速度+0.67/1.33/2%", tree: "utility", tier: 1, col: 2, maxRanks: 3 },
  4313: { name: "冥想", desc: "法力回复+1/2/3每5秒", tree: "utility", tier: 1, col: 3, maxRanks: 3 },
  4314: { name: "召唤师的感悟", desc: "召唤师技能冷却-5/10%", tree: "utility", tier: 1, col: 4, maxRanks: 2 },

  // Tier 2 (4 points required)
  4321: { name: "炼金术士", desc: "药水持续时间+10/20%", tree: "utility", tier: 2, col: 1, maxRanks: 2 },
  4322: { name: "烹饪大师", desc: "药水替换为饼干，回复生命和法力", tree: "utility", tier: 2, col: 2, maxRanks: 1 },
  4323: { name: "符文亲和", desc: "Buff持续时间+10/20%", tree: "utility", tier: 2, col: 3, maxRanks: 2 },
  4324: { name: "吸血习性", desc: "生命偷取+2%", tree: "utility", tier: 2, col: 4, maxRanks: 1 },

  // Tier 3 (8 points required)
  4331: { name: "贪婪", desc: "每10秒+0.5/1/1.5金币", tree: "utility", tier: 3, col: 1, maxRanks: 3 },
  4332: { name: "拾荒者", desc: "辅助装备效果+25%", tree: "utility", tier: 3, col: 2, maxRanks: 1 },
  4333: { name: "冥想", desc: "法力回复+2/4/6每5秒", tree: "utility", tier: 3, col: 3, maxRanks: 3 },
  4334: { name: "财富", desc: "初始金币+40", tree: "utility", tier: 3, col: 4, maxRanks: 1 },

  // Tier 4 (12 points required)
  4341: { name: "探索者", desc: "饰品施放范围+15%", tree: "utility", tier: 4, col: 1, maxRanks: 1 },
  4342: { name: "符能亲和", desc: "Buff持续时间+5/10/15%", tree: "utility", tier: 4, col: 2, maxRanks: 3 },
  4343: { name: "智谋", desc: "冷却缩减+1.25/2.5/3.75%", tree: "utility", tier: 4, col: 3, maxRanks: 3 },
  4344: { name: "风之精灵", desc: "离开泉水时+5%移动速度", tree: "utility", tier: 4, col: 4, maxRanks: 1 },

  // Tier 5 (16 points required)
  4352: { name: "智谋", desc: "冷却缩减+5%", tree: "utility", tier: 5, col: 1, maxRanks: 1 },
  4353: { name: "漫游", desc: "移动速度+5%", tree: "utility", tier: 5, col: 2, maxRanks: 1 },

  // Tier 6 (20 points required) - Keystone
  4362: { name: "风暴骑士的涌动", desc: "2秒内造成30%最大生命值伤害后+40%移速3秒", tree: "utility", tier: 6, col: 1, maxRanks: 1 },
};

export function getMasteryById(id) {
  return MASTERY_DATA[String(id)] || null;
}

export function getMasteriesByTree(tree) {
  return Object.values(MASTERY_DATA).filter(m => m.tree === tree);
}

export function validateMasteryPage(masteries) {
  if (!masteries) return true;
  let totalPoints = 0;
  for (const [id, rank] of Object.entries(masteries)) {
    const m = MASTERY_DATA[id];
    if (!m) return false;
    if (rank > m.maxRanks) return false;
    totalPoints += rank;
  }
  return totalPoints <= 30;
}

export { MASTERY_DATA };
export default MASTERY_DATA;
