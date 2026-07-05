module.exports = {
  root: true,
  env: {
    node: true,
    browser: true
  },
  globals: {
    __static: true,
    define: true,
    require: true,
    module: true,
    process: true,
    __dirname: true
  },
  extends: [
    "plugin:vue/essential",
    "eslint:recommended"
  ],
  parserOptions: {
    parser: "@babel/eslint-parser"
  },
  rules: {
    "vue/multi-word-component-names": "off",
    "no-undef": "warn"
  },
  overrides: [
    {
      files: ["src/assets/js/*.js"],
      rules: {
        "no-undef": "off"
      }
    }
  ]
}
