export default {
  CHAMPION_GRID_CLICK: {
    type: "SOUND",
    state: "CHAMPSELECT_CLICK",
    fileName: "champion-grid-click.ogg"
  },
  CHAMPION_GRID_HOVER: {
    type: "SOUND",
    state: "CHAMPSELECT_CLICK",
    fileName: "champion-grid-hover.ogg",
    volume: 0.25
  },
  LOCKIN_HOVER: {
    type: "SOUND",
    state: "CHAMPSELECT_CLICK",
    fileName: "lockin-hover.ogg",
    volume: 0.25
  },
  LOCKIN_CLICK: {
    type: "SOUND",
    state: "CHAMPSELECT_CLICK",
    fileName: "lockin-click.ogg"
  },
  CHAMPSELECT_INTRO: {
    type: "SOUND",
    state: "CHAMPSELECT_PHASE",
    fileName: "unlockcelebration.mp3",
    volume: 0.25
  },
  CHAMPSELECT_COUNTDOWN: {
    type: "MUSIC",
    state: "CHAMPSELECT_PHASE",
    fileName: "countdown10seconds.mp3"
  },
  CHAMPSELECT_EXIT: {
    type: "SOUND",
    state: "CHAMPSELECT_PHASE",
    fileName: "exitchampionselect.mp3"
  },
  CHAMPSELECT_PHASECHANGE: {
    type: "SOUND",
    state: "CHAMPSELECT_PHASE",
    fileName: "phasechangedrums2.mp3"
  },
  CHAMPSELECT_YOURTURN: {
    type: "SOUND",
    state: "CHAMPSELECT_PHASE",
    fileName: "yourturn.mp3"
  },
  CHAMPSELECT_TRADE: {
    type: "SOUND",
    state: "CHAMPSELECT_GENERAL",
    fileName: "traderequested.mp3"
  },
  CHAMPSELECT_MUSIC_BLIND: {
    type: "MUSIC",
    state: "CHAMPSELECT_MUSIC",
    fileName: "blindpickmusic.ogg",
    volume: 0.25,
    local: true
  },
  CHAMPSELECT_MUSIC_DRAFT: {
    type: "MUSIC",
    state: "CHAMPSELECT_MUSIC",
    fileName: "draftpickmusic.ogg",
    volume: 0.25,
    local: true
  },
  LOGIN_ACTION: {
    type: "SOUND",
    state: "LOGIN",
    fileName: "loginaction.mp3"
  },
  LOGIN_MUSIC: {
    type: "MUSIC",
    state: "LOGIN",
    fileName: "loginmusic.ogg",
    volume: 0.1,
    local: true
  },
  OVERVIEW_CHATINIT: {
    type: "SOUND",
    state: "OVERVIEW",
    fileName: "chatinit.mp3"
  },
  OVERVIEW_CLICK: {
    type: "SOUND",
    state: "OVERVIEW",
    fileName: "click.ogg"
  },
  OVERVIEW_INVITE: {
    type: "SOUND",
    state: "OVERVIEW",
    fileName: "invite.mp3"
  },
  OVERVIEW_OPENSTORE: {
    type: "SOUND",
    state: "OVERVIEW",
    fileName: "openstore.mp3"
  },
  OVERVIEW_PLAYBUTTON: {
    type: "SOUND",
    state: "OVERVIEW",
    fileName: "playbutton.mp3"
  },
  CORE_CLICK: {
    type: "SOUND",
    state: "CORE",
    fileName: "core_click.mp3",
    local: true
  },
  CORE_INVITE_RECIEVED: {
    type: "SOUND",
    state: "CORE",
    fileName: "invite_recieved.mp3",
    local: true
  },
  CORE_QUEUE_START: {
    type: "SOUND",
    state: "CORE",
    fileName: "closeplayscreen.mp3",
    local: true
  },
  CORE_SHOP_BUY_GENERAL: {
    type: "SOUND",
    state: "CORE",
    fileName: "shop_buy_general.mp3",
    local: true
  },
  CORE_SHOP_BUY_CHAMPION: {
    type: "SOUND",
    state: "CORE",
    fileName: "shop_buy_champion.mp3",
    local: true
  },
  CORE_MUSIC_QUEUE: {
    loop: true,
    type: "MUSIC",
    state: "CORE_MUSIC",
    fileName: "queue.ogg",
    volume: 0.5,
    local: true
  },
  CORE_MATCH_FOUND: {
    type: "SOUND",
    state: "CORE",
    fileName: "matchmakingqueued.mp3",
    local: true
  }
};
