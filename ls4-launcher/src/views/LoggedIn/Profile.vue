<template>
  <div class="nav-view">
    <div class="page-links">
      <router-link to="/LoggedIn/profile/overview" class="page-link">
        {{ t("PROFILE_OVERVIEW") }}
      </router-link>
      <router-link to="/LoggedIn/profile/runes" class="page-link">
        {{ t("PROFILE_RUNES") }}
      </router-link>
      <router-link to="/LoggedIn/profile/mastery" class="page-link">
        {{ t("PROFILE_MASTERY") }}
      </router-link>
    </div>
    <router-view class="page-content"></router-view>
  </div>
</template>

<script>
export default {};
</script>

<style lang="css" scoped>
.nav-view {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.page-links {
  display: flex;
  gap: 10px;
  padding: 10px;
  background-image: linear-gradient(180deg, #192e49 0%, #192e49 40%, #172b46 50%, #142131 100%);
  border-bottom: 1px solid rgba(100, 117, 137, 0.75);
}

.page-link {
  padding: 8px 20px;
  font-family: LoLFont2;
  font-size: 14px;
  color: rgba(255, 255, 255, 0.7);
  text-decoration: none;
  border-radius: 3px;
  transition: all ease-in-out 200ms;
  background-color: rgba(0, 0, 0, 0.3);
}

.page-link:hover {
  background-color: rgba(13, 71, 49, 0.4);
  color: white;
}

.page-link.router-link-active {
  background-color: rgba(13, 71, 49, 0.7);
  color: #f2d030;
  box-shadow: inset 0 0 5px 1px rgba(0, 0, 0, 0.5);
}

.page-content {
  flex: 1;
  padding: 20px;
}
</style>
