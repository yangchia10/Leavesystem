<template>
  <div>

    <main-list ref="mainList" @click-main="clickMain" />
    <hr class="style-two">

    <!-- <detail-list ref="detailList" @click-main="clickDetail" />
    <hr class="style-two">

    <child-list ref="childList" /> -->
  </div>
</template>

<script>
import mainList from './components/main-list.vue'
import detailList from './components/detail-list.vue'
import childList from './components/child-list.vue'
/**
 *
 */
export default {
  name: 'MD_ProcessRoute',
  components: { mainList, detailList, childList },
  data() {
    return {

    }
  },
  mounted() {},
  methods: {
    /**
     * 点击主表行数据
     * 加载明细数据
     */
    clickMain(props) {
      this.$refs.detailList.loading(props)
    },
    clickDetail(props) {
      this.$refs.childList.loading(props)
    }
  }
}
</script>

<style lang="scss" scoped>
.style-two{
 border: 0;
    height: 1px;
    background-image: linear-gradient(to right, rgba(0,0,0,0), rgba(0,0,0,0.75), rgba(0,0,0,0));
}

</style>
