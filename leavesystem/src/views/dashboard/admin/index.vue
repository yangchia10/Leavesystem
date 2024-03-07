
<template>
  <!-- <div class="dashboard-editor-container" />
 -->
  <div>
    <div class="iframe-container">
      <iframe :src="src" scrolling="auto" frameborder="0" class="frame" />
    </div>

  </div>
</template>

<script>
export default {
  name: 'DashboardAdmin',
  data() {
    return {
      src: 'https://www.5starlimitpot.com.tw/'
    }
  },
  created() {},
  mounted() {},
  methods: {}
}
</script>

<style lang="scss" scoped>
.dashboard-editor-container {
  padding: 32px;
  background-color: #fff;
  position: relative;
  height: calc(100vh - 84px);

  .el-card {
    height: 50%;
    margin-bottom: 10px;

    .clearfix {
      font-size: 87%;
    }

    /*.el-card__body {*/
    /*  height: 100%;*/
    /*}*/

    /*.item {*/
    /*  height: 20vh;*/
    /*}*/

    /*.chart {*/
    /*  height: 100%;*/
    /*}*/
  }
}

.iframe-container {
  position: absolute;
  top: 0px;
  left: 0px;
  right: 0px;
  bottom: 0px;
  margin-top: 60px;
  .frame {
    position: relative;
    top: 0px;
    width: 100%;
    height: 100%;
  }
}
</style>
