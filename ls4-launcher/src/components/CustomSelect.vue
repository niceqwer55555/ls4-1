<template>
  <div class="custom-select" :tabindex="tabindex" @blur="open = false">
    <div class="selected" :class="{ open: open }" @click="open = !open">
      {{ selected }}
    </div>
    <div class="items" :class="{ selectHide: !open }">
      <div
        v-for="(option, i) of options"
        :key="i"
        @click="
          selected = option;
          open = false;
          $emit('input', option);
        "
      >
        {{ option }}
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: {
    options: {
      type: Array,
      required: true
    },
    default: {
      type: String,
      required: false,
      default: null
    },
    tabindex: {
      type: Number,
      required: false,
      default: 0
    }
  },
  data() {
    return {
      selected: this.default
        ? this.default
        : this.options.length > 0
        ? this.options[0]
        : null,
      open: false
    };
  },
  mounted() {
    this.$emit("input", this.selected);
  }
};
</script>

<style scoped>
.custom-select {
  position: relative;
  width: 100%;
  text-align: left;
  outline: none;
  height: 22px;
  line-height: 22px;
}

.custom-select .selected {
  background: linear-gradient(0deg, #cecece 0%, #ffffff 100%);
  border-radius: 3px;
  border: 1px solid rgba(245, 245, 245, 0.75);
  color: black;
  font-size: 12px;
  font-family: LoLFont2;
  font-weight: bold;
  text-transform: uppercase;
  padding-left: 1em;
  cursor: pointer;
  user-select: none;
}

.custom-select .selected.open {
  border-radius: 3px 3px 0px 0px;
}

.custom-select .selected:before {
  position: absolute;
  content: "";
  top: 50%;
  transform: translateY(-50%);
  right: 30px;
  width: 1px;
  height: 65%;
  border-left: 2px solid rgba(183, 186, 188, 0.5);
}

.custom-select .selected:after {
  position: absolute;
  content: "";
  top: 10px;
  right: 1em;
  width: 0;
  height: 0;
  border: 5px solid transparent;
  border-color: rgba(0, 0, 0, 0.75) transparent transparent transparent;
}

.custom-select .items {
  color: black;
  border-radius: 0px 0px 6px 6px;
  overflow: hidden;
  position: absolute;
  background: linear-gradient(0deg, #cecece 0%, #ffffff 100%);
  left: 0;
  right: 0;
  z-index: 15 !important;
}

.custom-select .items div {
  color: black;
  padding-left: 1em;
  cursor: pointer;
  user-select: none;
  position: relative;
}

.custom-select .items div:hover {
  background: linear-gradient(0deg, #bbbbbb 0%, #cecece 100%);
}

.selectHide {
  display: none;
}
</style>
