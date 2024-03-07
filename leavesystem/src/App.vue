<template>
  <div id="app">
    <router-view></router-view>
  </div>
</template>

<script>
export default {
  name: "App",

  data() {
    return {
      someCondition: true, // 在這裡定義 someCondition
      showForm: true,
    };
  },
};
</script>
