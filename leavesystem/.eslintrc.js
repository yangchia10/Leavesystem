module.exports = {
  root: true,
  env: {
    node: true,
  },
  extends: [
    "plugin:vue/vue3-essential",
    "eslint:recommended",
    // 添加其他您想要扩展的配置
  ],
  parserOptions: {
    parser: "babel-eslint",
  },
  rules: {
    "vue/no-parsing-error": [2, { "x-invalid-end-tag": false }],
    // 在这里添加或覆盖规则
  },
};
