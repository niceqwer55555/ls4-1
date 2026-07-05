<template>
  <div id="LoggedIn">
    <div class="content">
      <router-view class="main-content"></router-view>
    </div>
    <SideBar />
  </div>
</template>

<script>
import SideBar from "@/components/SideBar.vue";

export default {
  components: {
    SideBar
  },
  beforeMount() {
    this.$store.dispatch("saveUserStatus", "ONLINE");
    this.$store.dispatch("changeBackgroundState", "HOME");
  }
};
</script>

<style lang="css" scoped>
#LoggedIn {
  width: 100%;
  height: calc(100% - 115px);
  margin-top: 77px;
  position: relative;
  z-index: 0;
  display: flex;
}

#LoggedIn .content {
  width: 80%;
  position: relative;
}
#LoggedIn .main-content {
  width: 100%;
  height: 100%;
  position: relative;
}

#LoggedIn .sidebar-holder {
  width: 20%;
}
</style>
