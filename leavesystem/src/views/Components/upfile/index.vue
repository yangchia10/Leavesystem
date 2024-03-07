<template>
  <el-upload
    class="upload-demo"
    :style="style"
    :action="path + action"
    :headers="headers"
    :name="name"
    :data="body"
    :show-file-list="false"
    :before-upload="beforeUpload"
    :on-success="onSuccess"
    :on-error="onError"
  >
    <el-button
      v-if="permission"
      v-permission="{ name: permission }"
      class="filter-item"
      type="warning"
      icon="el-icon-upload2"
      :loading="loading"
      :disabled="loading"
    >
      {{ loading ? loadingTitle : title }}
    </el-button>
    <el-button v-else class="filter-item" type="warning" icon="el-icon-upload2" :loading="loading" :disabled="loading">
      {{ loading ? loadingTitle : title }}
    </el-button>
  </el-upload>
</template>

<script>
import { getToken } from '@/utils/auth'
/**
 * 组件--导入Exlse
 * ! action 上传接口（必填）
 * ? title 按钮显示文本
 * ? left 是否有左边距（默认无）
 * ? right 是否有右边距（默认无）
 * 暴露方法：
 * ? on-success 上传成功事件
 */
export default {
  props: {
    // 上传接口（必填）
    action: {
      type: String,
      default: ''
    },
    // 按钮权限
    permission: {
      type: String,
      default: ''
    },
    // 是否有左边距
    left: {
      type: Boolean,
      default: false
    },
    // 是否有右边距
    right: {
      type: Boolean,
      default: false
    },
    // 上传数据
    upData: {
      type: Object,
      default: () => {}
    },
    // 删除文件名称
    name: {
      type: String,
      default: 'file'
    },
    // 相关文本
    title: {
      type: String,
      default: '导入'
    },
    loadingTitle: {
      type: String,
      default: '加载中'
    }
  },
  data() {
    return {
      // 上传路径
      path: process.env.VUE_APP_BASE_API || '',
      // 上传header
      headers: {
        AppID: 'PC',
        'X-Token': getToken()
      },
      // 上传body
      body: {},
      style: {
        'margin-left': this.left ? '10px' : '0',
        'margin-right': this.right ? '10px' : '0'
      },
      loading: false
    }
  },
  methods: {
    /**
     * 文件上传之前
     */
    beforeUpload(file) {
      return new Promise((resolve, reject) => {
        if (!this.action) {
          console.error('【上传按钮未配置接口】')
          return reject()
        }
        const { name } = file
        const suffix = name.split('.').pop()
        // 上传数据
        this.body = {
          ...this.upData,
          FileType: suffix
        }
        this.loading = true
        resolve()
      })
    },
    /**
     * 上传完成
     */
    onSuccess(res) {
      this.loading = false
      if (res.Code === 200) {
        this.$showSuccess('导入成功')
        this.$emit('fetc-list', res)
      } else this.$showError(res.Message || '导入失败')
    },
    onError() {
      this.loading = false
    }
  }
}
</script>

<style lang="scss" scoped>
.upload-demo {
  display: inline-block;
}
</style>
