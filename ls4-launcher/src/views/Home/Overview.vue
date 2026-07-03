<template>
  <div>
    <div class="wrapper">
      <div class="newsBox">
        <div class="newsList">
          <div class="newsBlock" v-for="(news, index) in newsList" :key="index">
            <div class="newsImage">
              <img :src="news.imageUrl" alt="NewsImage" />
            </div>
            <div class="newsContent">
              <div class="title">
                {{ news.title }}
              </div>
              <p>
                {{ news.content }}
              </p>
              <p>{{ news.author }} | {{ news.created }}</p>
            </div>
          </div>
        </div>
        <div class="bugReportBtn">
          <button class="ls4core-btn" @click="$router.push('/bugreport')">
            {{ t("HOME_BUGREPORT_BTN") }}
          </button>
        </div>
      </div>
      <div class="changelogBox">
        <div class="changelogTitle">
          {{ t("HOME_LS_CHANGES_NOTICE") }}
        </div>
        <div class="changelogList">
          <div
            class="changelogBlock"
            v-bind:data-author="commit.commit.author.name"
            v-bind:data-date="commit.commit.author.date"
            v-for="(commit, index) in commitList"
            :key="index"
          >
            <p>
              {{ commit.commit.message }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import CriticalSystemError from "../../utils/errors/CriticalSystemError";
import { mapState } from "vuex";

export default {
  computed: mapState({
    commitList: state => state.commitList,
    newsList: state => state.newsList
  }),
  created() {
    // REMOVE THIS AFTER DEVELOPMENT
    if (this.$store.state.user.summonerExperienceNeeded === undefined) {
      this.$store.dispatch("readRememberToken").then(loadedData => {
        if (loadedData && loadedData.token) {
          this.$store
            .dispatch("validateToken", {
              token: loadedData.token
            })
            .then(user => {
              if (!user) {
                throw new CriticalSystemError(
                  "Token validation resulted in NULL-user!"
                );
              } else {
                this.$store.dispatch("getNews");
                this.$store.dispatch("getAlerts");
              }
            })
            .catch(err => {
              console.error(err);
            });
        } else {
          //There is no remember token
        }
      });
    } else {
      // Already logged in
      this.$store.dispatch("getNews");
      this.$store.dispatch("getAlerts");
    }
    // PLEASE
  },
  beforeMount() {
    this.$store.dispatch("changeBackgroundState", "HOME");
    this.$store.dispatch("getLeagueSandboxCommits");
  }
};
</script>

<style lang="css" scoped>
a {
  background-color: white;
  border: none;
  color: black;
  padding: 15px 60px;
  text-align: center;
  text-decoration: none;
  display: inline-block;
  font-size: 40px;
  border-radius: 80px;
  outline: none;
  transition: background-color ease-in-out 200ms;
  cursor: pointer;
  font-family: LoLFont4;
}
.wrapper {
  position: relative;
  display: flex;
  padding: 10px;
  width: 100%;
  height: 100%;
  justify-content: space-between;
}

.wrapper .newsBox {
  display: flex;
  flex-direction: column;
  width: 60%;
  height: 100%;
  padding: 10px 5px;
  background-image: linear-gradient(180deg, #07111d 0%, #051121 100%);
  border: 1px solid #0c212c;
  border-radius: 2px;
}

.wrapper .changelogBox {
  display: flex;
  flex-direction: column;
  width: 38%;
  height: 100%;
  padding: 10px 5px;
  background-image: linear-gradient(180deg, #07111d 0%, #051121 100%);
  border: 1px solid #0c212c;
  border-radius: 2px;
}

.wrapper .changelogBox .changelogTitle {
  width: 100%;
  font-size: 14px;
  font-family: LoLFont2;
  margin-bottom: 10px;
  text-indent: 5px;
}

.wrapper .changelogBox .changelogList {
  display: flex;
  flex-direction: column;
  height: 100%;
  width: 100%;
  border-radius: 2px;
  overflow: auto;
}

.wrapper .changelogBox .changelogList .changelogBlock {
  background-color: #040d16;
  padding: 15px 20px;
  border: 1px solid #0c212c;
  position: relative;
  margin-bottom: 10px;
  font-size: 14px;
  word-break: break-word;
}

.wrapper .changelogBox .changelogList .changelogBlock::before {
  position: absolute;
  left: 5px;
  top: 5px;
  content: attr(data-author);
  font-size: 12px;
  font-style: italic;
  font-family: LoLFont4;
  color: goldenrod;
}

.wrapper .changelogBox .changelogList .changelogBlock::after {
  position: absolute;
  right: 5px;
  top: 5px;
  content: attr(data-date);
  font-size: 12px;
  font-style: italic;
  font-family: LoLFont4;
}

.wrapper .newsBox > div {
  width: 100%;
}

.wrapper .newsBox .newsList {
  display: flex;
  flex-direction: column;
  background-color: #040d16;
  height: 92%;
  padding: 15px 20px;
  border: 1px solid #0c212c;
  border-radius: 2px;
  overflow: auto;
}

.wrapper .newsBox .newsList .newsBlock {
  display: flex;
  width: 100%;
  height: 90px;
  margin-bottom: 10px;
  justify-content: space-between;
}

.wrapper .newsBox .newsList .newsBlock .newsImage {
  width: 25%;
  height: 100%;
}

.wrapper .newsBox .newsList .newsBlock .newsImage img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  object-position: top;
}

.wrapper .newsBox .newsList .newsBlock .newsContent {
  display: flex;
  flex-direction: column;
  width: 73%;
  height: 100%;
}

.wrapper .newsBox .newsList .newsBlock .newsContent .title {
  color: rgba(245, 245, 245, 0.75);
  text-shadow: 0px 0px 7px #0358ff;
  font-size: 12px;
  font-family: LoLFont2;
}

.wrapper .newsBox .newsList .newsBlock .newsContent p {
  font-size: 10px;
  color: rgba(245, 245, 245, 0.75);
  font-family: LoLFont2;
}

.wrapper .newsBox .bugReportBtn {
  height: 8%;
  position: relative;
}

.wrapper .newsBox .bugReportBtn button {
  position: absolute;
  left: 50%;
  top: 65%;
  transform: translate(-50%, -50%);
}

/* COPY TO GLOBAL */
.ls4core-btn {
  border: 1px solid rgba(46, 48, 49, 0.75);
  font-size: 14px;
  font-weight: bold;
  color: rgba(245, 245, 245, 0.95);
  font-family: LoLFont4;
  text-shadow: 0 0 7px rgba(245, 245, 245, 0.95);
  text-align: center;
  padding: 5px 35px;
  border-radius: 3px;
  outline: none;
  background: radial-gradient(circle, #0c2e5c 0%, #0c263f 100%),
    linear-gradient(90deg, #071d31 0%, transparent 50%, #071d31 100%);
  cursor: pointer;
  transition: filter ease-in-out 200ms;
}

.ls4core-btn:disabled {
  cursor: auto;
  text-shadow: none;
  background: radial-gradient(circle, #4b4b4b 0%, #2b2b2b 100%);
}

.ls4core-btn:not(:disabled):hover {
  filter: brightness(1.2);
}
</style>
