<template>
  <div id="error-page">
    <div class="content">
      <div class="error">
        <h2>
          {{ t("ERROR_ROUTE_NOT_FOUND") }}
        </h2>
        <p>
          {{ t("ERROR_NOTICE") }}
        </p>
        <div class="actions">
          <button @click="$router.push('/LoggedIn/home')">
            {{ t("ERROR_HOME_BTN") }}
          </button>
          <button @click="$router.push('/bugreport')">
            {{ t("ERROR_REPORT_BTN") }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  beforeMount() {
    this.$store.dispatch("changeBackgroundState", "ERROR");
  }
};
</script>

<style lang="css" scoped>
#error-page {
  width: 100%;
  height: calc(100% - 130px);
  margin-top: 90px;
  position: relative;
  z-index: 0;
  display: flex;
}

#error-page .content {
  width: 100%;
  height: 100%;
  position: relative;
}

#error-page .content .error {
  position: absolute;
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

#error-page .content .error h2 {
  margin-top: 0;
  text-align: center;
  font-family: LoLFont2;
}

#error-page .content .error p {
  margin-top: 0;
  text-align: center;
  font-family: LoLFont2;
  font-size: 14px;
}

#error-page .content .error .actions {
  display: flex;
  justify-content: space-between;
  width: 50%;
  left: 50%;
  position: relative;
  transform: translateX(-50%);
}

#error-page .content .error .actions button {
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

#error-page .content .error .actions button:hover {
  filter: brightness(1.25);
}
</style>
