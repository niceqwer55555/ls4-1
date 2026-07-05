import Vue from "vue";
import App from "./App.vue";
import router from "./Router";

// VUEX
import store from "./store";

// PLUGINS
import ModalsPlugin from "./plugins/modals";
import ChatPlugin from "./plugins/chat";
import SoundPlugin from "./plugins/sound";
import VueTranslate from "./plugins/translate";

Vue.config.productionTip = false;

// PLUGIN REGISTERS
Vue.use(VueTranslate);
Vue.use(ModalsPlugin);
Vue.use(ChatPlugin);
Vue.use(SoundPlugin, { store });

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount("#app");
