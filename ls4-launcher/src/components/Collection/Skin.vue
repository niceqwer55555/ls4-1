<template>
  <router-link
    v-bind:to="'/preview/skins/' + championName + '/' + skin.id"
    class="container"
  >
    <div class="skinName">{{ skin.name }}</div>
  </router-link>
</template>

<script>
export default {
  props: {
    skin: Object,
    championName: String
  }
};
// TODO: Needs to be reworked.
</script>

<style lang="scss" scoped>
.container {
  width: 25%;
  box-sizing: border-box;
  padding: 0 20px;
}
.image {
  width: 100%;
  filter: grayscale(0.3);
}
.image:hover {
  filter: grayscale(0);
  filter: brightness(1.5);
}
.skinName {
  text-align: center;
}
</style>
