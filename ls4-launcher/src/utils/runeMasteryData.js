import { getRuneById } from "./runeData";
import { getMasteryById } from "./masteryData";

/**
 * Get default rune pages for S4
 * Each page has 30 rune slots: 9 marks, 9 seals, 9 glyphs, 3 quintessences
 * @returns {Array} Default rune pages
 */
export function getDefaultRunePages() {
  return [
    {
      id: 1,
      name: "ADC",
      runes: {
        1: 5245, 2: 5245, 3: 5245, 4: 5245, 5: 5245, 6: 5245, 7: 5245, 8: 5245, 9: 5245,
        10: 5317, 11: 5317, 12: 5317, 13: 5317, 14: 5317, 15: 5317, 16: 5317, 17: 5317, 18: 5317,
        19: 5289, 20: 5289, 21: 5289, 22: 5289, 23: 5289, 24: 5289, 25: 5289, 26: 5289, 27: 5289,
        28: 5338, 29: 5338, 30: 5313
      }
    },
    {
      id: 2,
      name: "Mid",
      runes: {
        1: 5275, 2: 5275, 3: 5275, 4: 5275, 5: 5275, 6: 5275, 7: 5275, 8: 5275, 9: 5275,
        10: 5300, 11: 5300, 12: 5300, 13: 5300, 14: 5300, 15: 5300, 16: 5300, 17: 5300, 18: 5300,
        19: 5265, 20: 5265, 21: 5265, 22: 5265, 23: 5265, 24: 5265, 25: 5265, 26: 5265, 27: 5265,
        28: 5337, 29: 5337, 30: 5337
      }
    },
    {
      id: 3,
      name: "Jungle",
      runes: {
        1: 5245, 2: 5245, 3: 5245, 4: 5245, 5: 5245, 6: 5245, 7: 5245, 8: 5245, 9: 5245,
        10: 5317, 11: 5317, 12: 5317, 13: 5317, 14: 5317, 15: 5317, 16: 5317, 17: 5317, 18: 5317,
        19: 5289, 20: 5289, 21: 5289, 22: 5289, 23: 5289, 24: 5289, 25: 5289, 26: 5289, 27: 5289,
        28: 5335, 29: 5335, 30: 5343
      }
    },
    {
      id: 4,
      name: "Support",
      runes: {
        1: 5317, 2: 5317, 3: 5317, 4: 5317, 5: 5317, 6: 5317, 7: 5317, 8: 5317, 9: 5317,
        10: 5340, 11: 5340, 12: 5340, 13: 5340, 14: 5340, 15: 5340, 16: 5340, 17: 5340, 18: 5340,
        19: 5290, 20: 5290, 21: 5290, 22: 5290, 23: 5290, 24: 5290, 25: 5290, 26: 5290, 27: 5290,
        28: 5310, 29: 5310, 30: 5310
      }
    },
    {
      id: 5,
      name: "Top",
      runes: {
        1: 5245, 2: 5245, 3: 5245, 4: 5245, 5: 5245, 6: 5245, 7: 5245, 8: 5245, 9: 5245,
        10: 5317, 11: 5317, 12: 5317, 13: 5317, 14: 5317, 15: 5317, 16: 5317, 17: 5317, 18: 5317,
        19: 5289, 20: 5289, 21: 5289, 22: 5289, 23: 5289, 24: 5289, 25: 5289, 26: 5289, 27: 5289,
        28: 5342, 29: 5342, 30: 5342
      }
    }
  ];
}

/**
 * Get default mastery pages for S4
 * @returns {Array} Default mastery pages
 */
export function getDefaultMasteryPages() {
  return [
    {
        "id": 1,
        "name": "ADC / 物理输出",
        "masteries": {
            "4112": 2,
            "4114": 1,
            "4122": 2,
            "4124": 2,
            "4211": 1,
            "4212": 4,
            "4222": 3,
            "4231": 2,
            "4232": 1,
            "4241": 2,
            "4242": 1,
            "4243": 3,
            "4251": 3,
            "4262": 1
        }
    },
    {
        "id": 2,
        "name": "Mid / 法师",
        "masteries": {
            "4112": 2,
            "4114": 1,
            "4122": 2,
            "4124": 2,
            "4131": 1,
            "4213": 4,
            "4221": 1,
            "4224": 3,
            "4233": 2,
            "4234": 1,
            "4241": 2,
            "4242": 1,
            "4243": 3,
            "4244": 1,
            "4262": 1
        }
    },
    {
        "id": 3,
        "name": "Jungle / 打野",
        "masteries": {
            "4112": 2,
            "4114": 1,
            "4122": 2,
            "4124": 2,
            "4131": 3,
            "4211": 1,
            "4212": 4,
            "4222": 3,
            "4231": 2,
            "4241": 2,
            "4242": 1,
            "4243": 3,
            "4262": 1
        }
    },
    {
        "id": 4,
        "name": "Support / 辅助",
        "masteries": {
            "4112": 2,
            "4311": 2,
            "4312": 3,
            "4313": 3,
            "4314": 2,
            "4322": 1,
            "4323": 2,
            "4324": 1,
            "4331": 3,
            "4332": 1,
            "4333": 3,
            "4341": 1,
            "4343": 3,
            "4352": 1,
            "4362": 1
        }
    },
    {
        "id": 5,
        "name": "Top / 坦克",
        "masteries": {
            "4111": 2,
            "4112": 2,
            "4114": 1,
            "4122": 2,
            "4124": 2,
            "4131": 3,
            "4132": 3,
            "4141": 3,
            "4144": 1,
            "4151": 1,
            "4152": 1,
            "4162": 1,
            "4212": 4,
            "4214": 2,
            "4221": 1
        }
    }
];
}

/**
 * Get rune display info for a rune page
 * @param {Object} runePage - Rune page object with runes map
 * @returns {Array} Array of { slot, runeId, name, icon, stats }
 */
export function getRunePageDisplay(runePage) {
  if (!runePage || !runePage.runes) return [];
  const display = [];
  for (let slot = 1; slot <= 30; slot++) {
    const runeId = runePage.runes[slot];
    if (runeId) {
      const info = getRuneById(runeId);
      display.push({
        slot,
        runeId,
        name: info ? info.name : String(runeId),
        icon: info ? info.icon : "",
        stats: info ? info.stats : {}
      });
    }
  }
  return display;
}

/**
 * Get mastery display info for a mastery page
 * @param {Object} masteryPage - Mastery page object with masteries map
 * @returns {Object} { offense, defense, utility } point counts and details
 */
export function getMasteryPageDisplay(masteryPage) {
  if (!masteryPage || !masteryPage.masteries) return { offense: 0, defense: 0, utility: 0, details: [] };
  let offense = 0, defense = 0, utility = 0;
  const details = [];
  for (const [id, rank] of Object.entries(masteryPage.masteries)) {
    const info = getMasteryById(id);
    if (info) {
      if (info.tree === "offense") offense += rank;
      else if (info.tree === "defense") defense += rank;
      else if (info.tree === "utility") utility += rank;
      details.push({ id, rank, ...info });
    }
  }
  return { offense, defense, utility, total: offense + defense + utility, details };
}