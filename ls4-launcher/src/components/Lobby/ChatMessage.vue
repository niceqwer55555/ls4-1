<template>
  <div class="message">
    <div class="sender">{{ message.from }}:</div>
    <div class="text">
      {{ message.data }}
    </div>
  </div>
</template>

<script>
export default {
  props: {
    message: Object
  }
};
</script>

<style lang="scss" scoped>
.message {
  display: flex;
  font-size: 14px;
  font-family: LoLFont2;
}

.message .sender {
  color: #4ca3c1;
  margin-right: 5px;
  font-family: LoLFont2;
}
</style>
