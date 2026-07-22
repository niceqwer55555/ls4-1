/**
 * Mastery tree icon data for S4
 * Icons represented by CSS styling + unicode symbols
 */
export const MASTERY_TREE_ICONS = {
  offense: {
    symbol: '\u2694',
    color: '#c53030',
    bgGradient: 'linear-gradient(180deg, #742a2a 0%, #c53030 50%, #742a2a 100%)',
    headerBg: 'linear-gradient(180deg, #9b2c2c 0%, #742a2a 100%)'
  },
  defense: {
    symbol: '\uD83D\uDEE1',
    color: '#3182ce',
    bgGradient: 'linear-gradient(180deg, #2a4365 0%, #3182ce 50%, #2a4365 100%)',
    headerBg: 'linear-gradient(180deg, #2b6cb0 0%, #2a4365 100%)'
  },
  utility: {
    symbol: '\u2728',
    color: '#38a169',
    bgGradient: 'linear-gradient(180deg, #276749 0%, #38a169 50%, #276749 100%)',
    headerBg: 'linear-gradient(180deg, #2f855a 0%, #276749 100%)'
  }
};

export const MASTERY_ULTIMATE_NAMES = {
  offense: '\u9886\u4E3B\u4E4B\u4EE4',
  defense: '\u987D\u77F3\u8A93\u7EA6',
  utility: '\u98CE\u66B4\u9A91\u58EB\u7684\u6D8C\u52A8'
};

/**
 * Get mastery tier icon style
 * @param {string} tree - 'offense', 'defense', 'utility'
 * @param {number} tier - 1-6
 * @returns {Object} CSS style object
 */
export function getMasteryTierStyle(tree, tier) {
  const treeIcon = MASTERY_TREE_ICONS[tree] || MASTERY_TREE_ICONS.offense;
  const opacity = Math.min(1, 0.4 + tier * 0.1);
  return {
    borderLeft: '3px solid ' + treeIcon.color,
    paddingLeft: '8px',
    opacity: opacity
  };
}
