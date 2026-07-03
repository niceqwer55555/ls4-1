<template>
  <div class="player">
    <div
      class="tradeModal"
      v-if="
        tradeState != null &&
          player.user.summonerName == currentPlayer.user.summonerName
      "
    >
      <div class="head">
        {{ t("CS_PENDING_TRADE") }}
      </div>
      <div class="tradeinfo" v-if="tradeState.trade">
        <div class="trade initiator" v-if="tradeState.status == 'INITIATOR'">
          <div class="champion">
            <img
              :src="
                getChampionImgUrl(tradeState.trade.target.selectedChampion.id)
              "
            />
          </div>
          <div class="exchange">
            <i class="fas fa-exchange"></i>
          </div>
          <div class="champion">
            <img
              :src="
                getChampionImgUrl(
                  tradeState.trade.initiator.selectedChampion.id
                )
              "
            />
          </div>
        </div>
        <div class="trade target" v-if="tradeState.status == 'TARGET'">
          <div class="champion">
            <img
              :src="
                getChampionImgUrl(
                  tradeState.trade.initiator.selectedChampion.id
                )
              "
            />
          </div>
          <div class="exchange">
            <i class="fas fa-exchange"></i>
          </div>
          <div class="champion">
            <img
              :src="
                getChampionImgUrl(tradeState.trade.target.selectedChampion.id)
              "
            />
          </div>
        </div>
        <div class="info">
          <span>{{ t("CS_TRADE_SELF_INFO") }}</span>
          <span>{{ t("CS_TRADE_TARGET_INFO") }}</span>
        </div>
      </div>
      <div class="actions">
        <div class="buttons" v-if="tradeState.status == 'TARGET'">
          <button
            @click="acceptTrade(tradeState.trade.initiator.user.summonerName)"
          >
            {{ t("CS_TRADE_ACCEPT_BTN") }}
          </button>
          <button
            @click="denyTrade(tradeState.trade.initiator.user.summonerName)"
          >
            {{ t("CS_TRADE_DENY_BTN") }}
          </button>
        </div>
        <span v-else>
          <i class="fas fa-spinner spinner"></i>
        </span>
      </div>
    </div>
    <button
      class="tradeButton"
      @click="initiateTrade"
      v-if="
        globalState.phase == 'PRE' &&
          globalState.team != 'BLIND' &&
          player.user.summonerName != currentPlayer.user.summonerName
      "
      :disabled="!canTradeWith"
    >
      <i class="fas fa-exchange"></i>
    </button>
    <div class="champion">
      <img
        :src="
          player.selectedChampion
            ? getChampionImgUrl(player.selectedChampion.id)
            : 'static/images/general/not-selected.png'
        "
        :alt="player.selectedChampion ? player.selectedChampion.id : 'NONE'"
      />
      <div class="spells">
        <img :src="getSpell(player.spell1)" alt="SummonerSpell1" />
        <img :src="getSpell(player.spell2)" alt="SummonerSpell2" />
      </div>
    </div>
    <div class="details">
      <div class="name">
        {{ `${player.user.summonerName}` }}
        {{
          player.user.summonerName == currentPlayer.user.summonerName
            ? " (YOU)"
            : ""
        }}
      </div>
    </div>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  props: {
    player: Object
  },
  methods: {
    getPlayerStateAnim() {
      if (this.player.lockedIn) {
        return "static/anims/accent/champselect-player-idle.webm";
      } else if (this.player.canBan || this.player.canLockIn) {
        return "static/anims/accent/champselect-player-active.webm";
      } else {
        return "static/anims/accent/champselect-player-idle.webm";
      }
    },
    getChampionImgUrl(champId) {
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/champions/${champId}.jpg`;
    },
    initiateTrade() {
      this.$socket.sendChampSelectMessage(
        `CHAMPSELECT_TRADE_REQUEST`,
        { data: this.player.user.summonerName },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }

          console.log(response);
        }
      );
    },
    getSpell(spell) {
      spell = spell.toLowerCase();
      const parts = spell.split("_");
      parts.forEach((part, index) => {
        parts[index] = part.charAt(0).toUpperCase() + part.slice(1);
      });
      const spellimg = parts.join("");

      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/summoner_spells/${spellimg}.jpg`;
    },
    acceptTrade(playerName) {
      this.$socket.sendChampSelectMessage(
        `CHAMPSELECT_TRADE_ACCEPT`,
        { data: playerName },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }

          console.log(response);
        }
      );
    },
    denyTrade(playerName) {
      this.$socket.sendChampSelectMessage(
        `CHAMPSELECT_TRADE_DENY`,
        { data: playerName },
        (response, error) => {
          if (error) {
            console.log("Flyback error:");
            console.log(error);
          }

          console.log(response);
        }
      );
    }
  },
  computed: mapState({
    globalState: state => state.csGlobalState,
    tradeState: state => {
      const myTrades = state.champselect.tradesTeam.filter(trade => {
        return (
          trade.initiator.user.summonerName ==
            state.csCurrentPlayer.user.summonerName ||
          trade.target.user.summonerName ==
            state.csCurrentPlayer.user.summonerName
        );
      });

      if (myTrades.length == 0) return;

      const mytrade = myTrades[0];

      if (
        mytrade.initiator.user.summonerName ==
        state.csCurrentPlayer.user.summonerName
      ) {
        return { status: "INITIATOR", trade: mytrade };
      } else if (
        mytrade.target.user.summonerName ==
        state.csCurrentPlayer.user.summonerName
      ) {
        return { status: "TARGET", trade: mytrade };
      } else {
        return null;
      }
    },
    canTradeWith(state) {
      if (
        this.player.user.summonerName == state.csCurrentPlayer.user.summonerName
      ) {
        console.log("A");
        return false;
      }

      if (
        state.champselect.tradesTeam.filter(trade => {
          return (
            trade.initiator.summonerName == this.player.summonerName ||
            trade.target.summonerName == this.player.summonerName
          );
        }).length > 0
      ) {
        console.log("B");
        return false;
      }

      return true;
    },
    currentPlayer: state => state.csCurrentPlayer
  })
};
</script>

<style lang="css" scoped>
.player {
  width: 100%;
  height: 19%;
  background: linear-gradient(180deg, #363636 0%, #090b0a 100%);
  border: 2px solid #5f5f5f;
  border-radius: 3px;
  display: flex;
  flex-direction: column;
  position: relative;
  z-index: 15 !important;
}

.player .tradeButton {
  position: absolute;
  top: 5px;
  right: 5px;
  width: 30px;
  height: 30px;
  font-size: 16px;
  padding: 5px;
  background: linear-gradient(
    180deg,
    #3c73b4 0%,
    #20477e 45%,
    #1e3e6d 50%,
    #0e284b 100%
  );
  outline: none;
  border: none;
  border-radius: 2px;
  border: 1px solid #304b69;
  cursor: pointer;
  color: #bda21b;
}

.player .tradeButton:disabled {
  filter: grayscale(100%);
}

.player .tradeButton:not(:disabled):hover {
  filter: brightness(1.25);
}

.player .tradeModal {
  width: 200px;
  z-index: 15 !important;
  height: 110px;
  position: absolute;
  right: -205px;
  top: -15px;
  border-radius: 3px;
  display: flex;
  flex-direction: column;
  background: linear-gradient(0deg, #100e0f 0%, #212222 100%);
  border: 1px solid #3d3d3d;
}

.player .tradeModal .head {
  width: 100%;
  height: 20%;
  font-family: LoLFont2;
  font-size: 12px;
  padding: 2px 5px;
  text-align: center;
}

.player .tradeModal .tradeinfo {
  display: flex;
  width: 100%;
  height: 60%;
  background-color: #060605;
  border: 1px solid #3d3d3d;
  flex-direction: column;
}
.player .tradeModal .tradeinfo .trade {
  display: flex;
  justify-content: space-around;
  width: 100%;
  height: 80%;
  padding: 3px 8px;
}

.player .tradeModal .tradeinfo .exchange {
  padding-top: 15px;
  color: #f3c447;
}
.player .tradeModal .tradeinfo .champion {
  width: 35px;
  height: 35px;
  border-radius: 3px;
}

.player .tradeModal .tradeinfo .champion img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  object-position: center;
  border: 1px solid #3d3d3d;
  border-radius: 3px;
}

.player .tradeModal .tradeinfo .info {
  display: flex;
  justify-content: space-between;
  width: 100%;
  padding: 0px 20px;
  height: 80%;
  font-family: LoLFont2;
  font-size: 12px;
}

.player .tradeModal .actions {
  width: 100%;
  height: 20%;
  position: relative;
  text-align: center;
}

.player .tradeModal .actions .buttons {
  display: flex;
  justify-content: space-around;
  width: 100%;
  padding: 2px 0;
  height: 100%;
}

.player .tradeModal .actions .buttons button {
  font-size: 12px;
  padding: 2px 10px;
  background: linear-gradient(
    180deg,
    #3c73b4 0%,
    #20477e 45%,
    #1e3e6d 50%,
    #0e284b 100%
  );
  outline: none;
  border: none;
  border-radius: 2px;
  border: 1px solid #304b69;
  cursor: pointer;
  color: white;
  transition: filter ease-in-out 200ms;
}

.player .tradeModal .actions .buttons button:hover {
  filter: brightness(1.25);
}

.player .tradeModal .actions .spinner {
  animation-name: spin;
  animation-duration: 2s;
  animation-iteration-count: infinite;
  animation-timing-function: linear;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.player > .champion {
  padding: 5px;
  display: flex;
  width: 50%;
  height: 75%;
  position: relative;
  left: 50%;
  transform: translateX(-50%);
}

.player > .champion img {
  width: 50px;
  height: 50px;
  margin-right: 5px;
  border: 1.5px solid #5f5f5f;
  border-radius: 3px;
}

.player > .champion .spells {
  display: flex;
  flex-direction: column;
}

.player > .champion .spells img {
  width: 25px;
  height: 25px;
  border: 1px solid #5f5f5f;
  border-radius: 3px;
}

.player > .details {
  width: 100%;
  height: 25%;
  text-align: center;
  font-family: LoLFont2;
  font-size: 12px;
}
</style>
