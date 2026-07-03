<template>
  <div class="ban">
    <img :src="getChampionImgUrl()" alt="BAN" />
    <div class="name">
      {{ ban.id }}
    </div>
  </div>
</template>

<script>
export default {
  props: {
    ban: Object
  },
  methods: {
    getChampionImgUrl() {
      const { host, port } = this.$store.state.config.download;
      return `${host}:${port}/champions/${this.ban.id}.jpg`;
    }
  }
};
</script>

<style lang="css" scoped>
.ban {
  display: flex;
  width: 100%;
  height: 39px;
  padding: 2px;
}

.ban img {
  height: 35px;
  width: 35px;
  margin-right: 5px;
}
.ban .name {
  font-family: LoLFont2;
  font-size: 14px;
  line-height: 36px;
}
</style>
