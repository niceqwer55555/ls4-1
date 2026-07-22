/**
 * Rune icon SVG data for S4 rune types
 * Using CSS class-based icons since we don't have image assets
 */
export const RUNE_TYPE_ICONS = {
  mark: {
    symbol: '\u2694',
    color: '#c53030',
    bgGradient: 'linear-gradient(135deg, #8b1a1a 0%, #c53030 50%, #8b1a1a 100%)'
  },
  seal: {
    symbol: '\uD83D\uDEE1',
    color: '#d69e2e',
    bgGradient: 'linear-gradient(135deg, #975a16 0%, #d69e2e 50%, #975a16 100%)'
  },
  glyph: {
    symbol: '\u2726',
    color: '#3182ce',
    bgGradient: 'linear-gradient(135deg, #2a4365 0%, #3182ce 50%, #2a4365 100%)'
  },
  quintessence: {
    symbol: '\u2605',
    color: '#805ad5',
    bgGradient: 'linear-gradient(135deg, #553c9a 0%, #805ad5 50%, #553c9a 100%)'
  }
};

export const RUNE_TYPE_NAMES = {
  mark: '\u5370\u8BB0',
  seal: '\u7B26\u5370',
  glyph: '\u96D5\u6587',
  quintessence: '\u7CBE\u534E'
};

export const RUNE_TIER_NAMES = {
  1: '\u521D\u7EA7',
  2: '\u4E2D\u7EA7',
  3: '\u9AD8\u7EA7'
};

/**
 * Get rune icon style for display
 * @param {Object} rune - Rune data object
 * @returns {Object} CSS style object
 */
export function getRuneIconStyle(rune) {
  const typeIcon = RUNE_TYPE_ICONS[rune.type] || RUNE_TYPE_ICONS.mark;
  return {
    background: typeIcon.bgGradient,
    borderRadius: rune.type === 'quintessence' ? '50%' : '4px',
    width: rune.type === 'quintessence' ? '36px' : '28px',
    height: rune.type === 'quintessence' ? '36px' : '28px',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    fontSize: rune.type === 'quintessence' ? '16px' : '12px',
    color: 'white',
    textShadow: '0 1px 2px rgba(0,0,0,0.5)',
    border: '1px solid ' + typeIcon.color,
    boxShadow: '0 0 4px ' + typeIcon.color + '40'
  };
}
