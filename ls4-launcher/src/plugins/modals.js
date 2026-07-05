import SetterModalComponent from "../components/Modals/SetterModal.vue";
import FriendContextMenuModal from "../components/Modals/FriendContextMenuModal.vue";
import LogoutModal from "../components/Modals/LogoutModal.vue";
import MatchMakingModal from "../components/Modals/MatchMakingModal.vue";
import SummonerIconModal from "../components/Modals/SummonerIconModal.vue";
import ReconnectModal from "../components/Modals/ReconnectModal.vue";

const Modal = {
  install(Vue) {
    this.EventBus = new Vue();

    Vue.component("setter-modal", SetterModalComponent);
    Vue.component("friend-contextmenu", FriendContextMenuModal);
    Vue.component("logout", LogoutModal);
    Vue.component("matchmaking", MatchMakingModal);
    Vue.component("summonericonmodal", SummonerIconModal);
    Vue.component("reconnectmodal", ReconnectModal);

    Vue.prototype.$modal = {
      setter: {
        show(params) {
          Modal.EventBus.$emit("setter-show", params);
        }
      },
      friendcontextmenu: {
        show(params) {
          Modal.EventBus.$emit("friend-contextmenu-show", params);
        }
      },
      logout: {
        show() {
          Modal.EventBus.$emit("logout-show");
        }
      },
      matchmaking: {
        show() {
          Modal.EventBus.$emit("matchmaking-show");
        },
        hide() {
          Modal.EventBus.$emit("matchmaking-hide");
        }
      },
      summonericon: {
        show() {
          Modal.EventBus.$emit("summonericon-modal-show");
        }
      },
      reconnect: {
        show() {
          Modal.EventBus.$emit("reconnect-modal-show");
        },
        hide() {
          Modal.EventBus.$emit("reconnect-modal-hide");
        }
      }
    };
  }
};

export default Modal;
