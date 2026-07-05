<template>
  <div class="nav-view">
    <div class="page-links">
      <router-link to="/LoggedIn/collection/champions">Champions</router-link>
      <router-link to="/LoggedIn/collection/icons">Icons</router-link>
    </div>
    <router-view class="page-content"></router-view>
  </div>
</template>

<script>
export default {};
</script>

<style lang="css" scoped></style>
