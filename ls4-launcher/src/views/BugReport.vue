<template>
  <div id="bug-report">
    <div class="content">
      <div class="report">
        <h2>
          {{ t("BUG_TITLE") }}
        </h2>
        <p>
          {{ t("BUG_SUBTITLE") }}
        </p>
        <input
          type="text"
          ref="shorttext"
          :placeholder="t('BUG_SHORTDESC_PLACEHOLDER')"
        />
        <textarea
          ref="longtext"
          :placeholder="t('BUG_LONGDESC_PLACEHOLDER')"
        ></textarea>
        <div class="actions">
          <button @click="$router.push('/LoggedIn/home')">
            {{ t("BUG_HOME_BTN") }}
          </button>
          <button @click="sendBugreport">{{ t("BUG_SUBMIT_BTN") }}</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  beforeMount() {
    this.$store.dispatch("changeBackgroundState", "ERROR");
  },
  methods: {
    sendBugreport: function(e) {
      e.preventDefault();
      const router = this.$router;

      this.$store
        .dispatch("sendBugReport", {
          description: this.$refs.shorttext.value,
          text: this.$refs.longtext.value
        })
        .then(function() {
          router.push("/LoggedIn/home");
        })
        .catch(function(err) {
          console.error(err);
        });
    }
  }
};
</script>

<style lang="css" scoped>
#bug-report {
  width: 100%;
  height: calc(100% - 130px);
  margin-top: 90px;
  position: relative;
  z-index: 0;
  display: flex;
}

#bug-report .content {
  width: 100%;
  height: 100%;
  position: relative;
}

#bug-report .content .report {
  position: absolute;
  display: flex;
  flex-direction: column;
  left: 50%;
  top: 50%;
  width: 50%;
  border: 1px solid red;
  transform: translate(-50%, -50%);
  border: 1px solid rgba(100, 117, 137, 0.75);
  border-radius: 5px;
  background-color: rgba(5, 12, 20, 0.95);
  padding: 20px;
}

#bug-report .content .report h2 {
  margin-top: 0;
  text-align: center;
  font-family: LoLFont2;
  margin-bottom: 6px;
}

#bug-report .content .report p {
  margin-top: 0;
  text-align: center;
  font-family: LoLFont2;
  font-size: 14px;
}

#bug-report .content .report input {
  border: 1px solid #404549;
  border-radius: 3px;
  box-shadow: inset 0 0 5px 1px #1c2932;
  width: 100%;
  outline: none;
  margin-bottom: 10px;
  padding: 5px;
  font-family: LoLFont2;
}

#bug-report .content .report textarea {
  border: 1px solid #404549;
  border-radius: 3px;
  box-shadow: inset 0 0 5px 1px #1c2932;
  width: 100%;
  outline: none;
  margin-bottom: 10px;
  padding: 5px;
  height: 300px;
  font-family: LoLFont2;
}

#bug-report .content .report .actions {
  display: flex;
  justify-content: space-between;
  width: 50%;
  left: 50%;
  position: relative;
  transform: translateX(-50%);
}

#bug-report .content .report .actions button {
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
  margin-right: 10px;
  padding: 5px 40px;
  border: 1px solid #304b69;
  color: white;
  font-family: LoLFont2;
  transition: filter ease-in-out 200ms;
  cursor: pointer;
}

#bug-report .content .report .actions button:hover {
  filter: brightness(1.25);
}
</style>
