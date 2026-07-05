<template>
  <div id="login">
    <div class="provider-logo">
      <video src="static/anims/general/l4s_logo.webm" autoplay loop></video>
    </div>
    <div class="loginbox mockbox" v-show="!register">
      <div class="head">
        <p>{{ t("LOGIN_LOGIN_TITLE") }}</p>
      </div>
      <div class="inner">
        <div class="formGroup">
          <label for="usernameInput">
            {{ t("LOGIN_USERNAME") }}
          </label>
          <input
            type="text"
            id="usernameInput"
            :placeholder="t('LOGIN_USERNAME')"
            v-model="logindata.username"
            ref="log_username"
          />
        </div>

        <div class="formGroup">
          <label for="passwordInput">
            {{ t("LOGIN_PASSWORD") }}
          </label>
          <input
            type="password"
            id="passwordInput"
            v-model="logindata.password"
            :placeholder="t('LOGIN_PASSWORD')"
            v-on:keyup.enter="onLogin"
            ref="log_password"
          />
        </div>
        <div class="submitGroup">
          <div class="remember">
            <label for="rememberCheck" class="league-check">
              <input
                type="checkbox"
                id="rememberCheck"
                v-model="logindata.rememberme"
                value="1"
              />
              {{ t("LOGIN_REMEMBER_ALL") }}
              <span class="checkmark"></span>
            </label>
          </div>
          <div class="login">
            <button class="loginBtn" @click="onLogin" ref="loginBtn">
              {{ t("LOGIN_LOGIN_BTN") }}
            </button>
          </div>
        </div>
      </div>
      <div class="foot">
        <p>
          {{ t("LOGIN_REGSWITCH_QUESTION") }}
          <a @click="onSwitchAction" ref="switchBtn">{{
            t("LOGIN_REGSWITCH_BTN")
          }}</a>
        </p>
        <p class="warning">
          {{ t("LOGIN_SETTINGS_NOTICE") }}
        </p>
        <button class="settingsBtn" @click="openSettings">
          <img src="@/assets/icons/cogs-duotone.svg" />
        </button>
        <p class="error">{{ debugInput }}</p>
      </div>
    </div>

    <div class="loginbox mockbox register" v-show="register">
      <div class="head">
        <p>{{ t("LOGIN_REGISTER_TITLE") }}</p>
      </div>
      <div class="inner">
        <div class="formGroup">
          <label for="emailInput">
            {{ t("LOGIN_EMAIL") }}
          </label>
          <input
            type="text"
            id="emailInput"
            :placeholder="t('LOGIN_EMAIL')"
            v-model="regdata.email"
            ref="reg_email"
          />
        </div>

        <div class="formGroup">
          <label for="usernameInput2">
            {{ t("LOGIN_USERNAME") }}
          </label>
          <input
            type="text"
            id="usernameInput2"
            :placeholder="t('LOGIN_USERNAME')"
            v-model="regdata.username"
            ref="reg_username"
          />
        </div>

        <div class="formGroup">
          <label for="passwordInput2">
            {{ t("LOGIN_PASSWORD") }}
          </label>
          <input
            type="password"
            id="passwordInput2"
            :placeholder="t('LOGIN_PASSWORD')"
            v-model="regdata.password"
            ref="reg_password"
          />
        </div>

        <div class="formGroup">
          <label for="summonerNameInput">
            {{ t("LOGIN_SUMMONER_NAME") }}
          </label>
          <input
            type="text"
            id="summonerNameInput"
            :placeholder="t('LOGIN_SUMMONER_NAME')"
            v-model="regdata.summonername"
            ref="reg_summonername"
          />
        </div>

        <div class="submitGroup">
          <div class="login">
            <button class="registerBtn" @click="onRegister" ref="registerBtn">
              {{ t("LOGIN_REGISTER_BTN") }}
            </button>
          </div>
        </div>
      </div>
      <div class="foot">
        <button class="settingsBtn" @click="openSettings">
          <img src="@/assets/icons/cogs-duotone.svg" />
        </button>
        <p>
          {{ t("LOGIN_LOGSWITCH_QUESTION") }}
          <a @click="onSwitchAction" ref="switchBtn">{{
            t("LOGIN_LOGSWITCH_BTN")
          }}</a>
        </p>
        <p class="error">{{ debugInput }}</p>
      </div>
    </div>

    <div class="settingsbox mockbox" v-bind:class="{ active: showSettings }">
      <div class="head">
        <p>{{ t("LOGIN_SETTINGS_TITLE") }}</p>
      </div>
      <div class="inner">
        <div class="formGroup">
          <label for="apiServerHostInput">
            {{ t("LOGIN_API_IP") }}
          </label>
          <input
            type="text"
            id="apiServerHostInput"
            placeholder="127.0.0.1"
            ref="stg_apiserverhost"
            v-model="apiserverhost"
          />
        </div>
        <div class="formGroup">
          <label for="cdnServerHostInput">
            {{ t("LOGIN_CDN_IP") }}
          </label>
          <input
            type="text"
            id="cdnServerHostInput"
            placeholder="127.0.0.1"
            ref="stg_cdnserverhost"
            v-model="cdnserverhost"
          />
        </div>
        <div class="formGroup">
          <label for="locale">
            {{ t("LOGIN_SETTINGS_LANGUAGE") }}
          </label>
          <select @change="changeLang($event)" v-model="langcode" id="">
            <option
              :selected="lang.code == 'en_US'"
              :value="lang.code"
              v-for="lang in languages"
              :key="lang.code"
              >{{ lang.language }} - {{ lang.code }}</option
            >
          </select>
        </div>
        <div class="formGroup filePicker">
          <label for="clientPathInput">
            {{ t("LOGIN_LOL_EXEPATH") }}
          </label>
          <div class="pickerWrapper">
            <input
              type="text"
              id="clientPathInput"
              v-model="clientpath"
              ref="stg_clientpath"
            />
            <button
              @click="onFilePicker"
              class="filepicker-button"
              ref="filepickerBtn"
            >
              <img src="@/assets/icons/file-search-duotone.svg" />
            </button>
          </div>
        </div>
        <div class="formGroup updater">
          <label for="updateButton">
            {{ t("CHECK_FOR_UPDATE") }}
          </label>
          <div class="updateWrapper">
            <button
              @click="onUpdate"
              class="update-button"
              ref="updateBtn"
              id="updateButton"
            >
              <i class="fas fa-sync-alt"></i>
            </button>
            <p class="updateStatus">{{ updateStatus }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ipcRenderer } from "electron";
import generalTemplates from "../plugins/soundTemplates/general";
export default {
  data() {
    return {
      clientpath: "",
      cdnserverhost: "127.0.0.1:8081/cdn", //"cdn.leagues4.com",
      apiserverhost: "127.0.0.1:8080", //"api.leagues4.com",
      langcode: "en_US",
      logindata: {
        username: "",
        password: "",
        rememberme: 1
      },
      regdata: {
        password: "",
        summonername: "",
        username: "",
        email: ""
      },
      loading: false,
      debugInput: "",
      register: false,
      showSettings: false,
      updateStatus: ""
    };
  },
  created() {
    const router = this.$router;
    this.$store.dispatch("readRememberToken").then(loadedData => {
      if (loadedData && loadedData.token) {
        this.$store
          .dispatch("validateToken", {
            token: loadedData.token
          })
          .then(() => {
            router.push("/LoggedIn/home");
          })
          .catch(() => {
            //Invalid / Expired remember token
          });
      } else if (loadedData) {
        console.log(loadedData);
        this.clientpath = loadedData.clientpath;
        this.apiserverhost = loadedData.apiserverhost;
        this.cdnserverhost = loadedData.cdnserverhost;
        this.langcode = loadedData.langcode;
      }
    });

    ipcRenderer.on("update", (event, info) => {
      this.updateStatus = info;
    });

    if (!this.$sound.isMusicPlaying("LOGIN")) {
      this.$sound.template("LOGIN_MUSIC");
    }
  },
  beforeMount() {
    this.$store.dispatch("changeBackgroundState", "LOGIN");
  },
  beforeDestroy() {
    if (!this.register) {
      this.$sound.stopAll("LOGIN");
    }
  },
  computed: {
    languages() {
      return this.$translate.getLanguages();
    }
  },
  methods: {
    onUpdate() {
      ipcRenderer.invoke("checkForUpdate");
    },
    changeLang(event) {
      this.$translate.setLang(event.target.value);
    },
    openSettings() {
      this.showSettings = !this.showSettings;
    },
    onFilePicker(e) {
      e.preventDefault();

      let that = this;

      ipcRenderer.invoke("openFileDialog").then(result => {
        if (result) {
          if (result.includes("League of Legends.exe")) {
            that.clientpath = result; //G:\Games\LoL-420\RADS\solutions\lol_game_client_sln\releases\0.0.1.68\deploy\League of Legends.exe
          }
          else {
            that.clientpath = "League of Legends executable invalid. (Download: https://github.com/LeagueSandbox/GameServer#manual-setup-windowsmac)";
          }
        }
      });
    },
    onSwitchAction(e) {
      if (e) {
        e.preventDefault();
      }

      let loading = this.loading;
      if (loading) return;

      this.debugInput = "";
      this.register = !this.register;
    },
    onLogin(e) {
      e.preventDefault();
      let loading = this.loading;
      if (loading) return;

      const loginBtn = this.$refs.loginBtn;
      const switchBtn = this.$refs.switchBtn;
      const filepickerBtn = this.$refs.filepickerBtn;
      const router = this.$router;
      const that = this;

      if (this.logindata.username == "") {
        this.$refs.log_password.focus();
        return;
      }

      if (this.logindata.password == "") {
        this.$refs.log_password.focus();
        return;
      }

      if (this.apiserverhost == "") {
        if (!this.showSettings) this.showSettings = true;
        this.$refs.stg_apiserverhost.focus();
        return;
      }

      if (this.cdnserverhost == "") {
        if (!this.showSettings) this.showSettings = true;
        this.$refs.stg_cdnserverhost.focus();
        return;
      }

      if (
        this.clientpath == "" ||
        !this.clientpath.includes("League of Legends.exe")
      ) {
        if (!this.showSettings) this.showSettings = true;
        this.$refs.stg_clientpath.focus();
        return;
      }

      loading = true;
      loginBtn.disabled = true;
      switchBtn.disabled = true;
      filepickerBtn.disabled = true;

      this.$store
        .dispatch("logUserIn", {
          username: this.logindata.username,
          password: this.logindata.password,
          apiserverhost: this.apiserverhost,
          cdnserverhost: this.cdnserverhost,
          rememberme: this.logindata.rememberme,
          clientpath: this.clientpath,
          langcode: this.langcode
        })
        .then(function() {
          that.$sound.template("LOGIN_ACTION");

          router.push("/LoggedIn");
          loading = false;
          loginBtn.disabled = false;
          switchBtn.disabled = false;
          filepickerBtn.disabled = false;

          for (let key in generalTemplates) {
            new Audio(that.$sound.getPath(generalTemplates[key]));
          }
        })
        .catch(function(err) {
          loading = false;
          loginBtn.disabled = false;
          switchBtn.disabled = false;
          filepickerBtn.disabled = false;

          that.debugInput = err;

          console.error(err);
        });
    },
    onRegister(e) {
      e.preventDefault();
      let loading = this.loading;
      if (loading) return;

      const switchBtn = this.$refs.switchBtn;
      const registerBtn = this.$refs.registerBtn;
      const that = this;

      if (this.regdata.email == "") {
        this.$refs.reg_email.focus();
        return;
      }

      if (this.regdata.username == "") {
        this.$refs.reg_username.focus();
        return;
      }

      if (this.regdata.password == "") {
        this.$refs.reg_password.focus();
        return;
      }

      if (this.regdata.summonername == "") {
        this.$refs.reg_summonername.focus();
        return;
      }

      if (this.regapiserverhost == "") {
        if (!this.showSettings) this.showSettings = true;
        this.$refs.stg_apiserverhost.focus();
        return;
      }

      loading = true;
      switchBtn.disabled = true;
      registerBtn.disabled = true;
      this.$store
        .dispatch("registerUser", {
          email: this.regdata.email,
          username: this.regdata.username,
          password: this.regdata.password,
          summonername: this.regdata.summonername,
          apiserverhost: this.apiserverhost
        })
        .then(function() {
          that.onSwitchAction();
          loading = false;
          switchBtn.disabled = false;
          registerBtn.disabled = false;
        })
        .catch(function(err) {
          loading = false;
          switchBtn.disabled = false;
          registerBtn.disabled = false;

          that.debugInput = err;

          console.error(err);
        });
    }
  }
};
</script>

<style scoped src="@/assets/css/login.css"></style>
