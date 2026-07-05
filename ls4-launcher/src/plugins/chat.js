import PrivateChat from "../components/Chat/PrivateChat.vue";

const Chat = {
  install(Vue) {
    this.EventBus = new Vue();

    Vue.component("private-chat", PrivateChat);

    Vue.prototype.$chat = {
      show(params) {
        Chat.EventBus.$emit("show", params);
      },
      getState() {
        return Chat.EventBus.$emit("getstate");
      }
    };
  }
};

export default Chat;
