<template>
  <div class="app-container">
    <!-- 檢索以及工具條區域 -->
    <div class="filter-container">
      <el-input
        v-model="listQuery.keyword"
        :placeholder="$t('Common.keyword')"
        style="width: 300px;"
        class="filter-item"
        @keyup.enter.native="handleFilter"
      />

      <el-button
        v-waves
        class="filter-item"
        type="primary"
        icon="el-icon-search"
        @click="handleFilter"
      />

      <el-button
        v-waves
        class="filter-item"
        type="primary"
        icon="el-icon-plus"
        @click="handleCreate"
      >添加</el-button>

      <el-button
        v-waves
        class="filter-item"
        type="primary"
        :disabled="canNotUpdate"
        icon="el-icon-edit"
        @click="handleUpdate"
      >編輯</el-button>

      <el-button
        v-waves
        class="filter-item"
        type="danger"
        icon="el-icon-delete"
        @click="handleDelete"
      >刪除</el-button>

      <el-button
        v-waves
        :loading="downloadLoading"
        class="filter-item"
        type="primary"
        icon="el-icon-download"
      >導出</el-button>
    </div>

    <!-- 數據列表 -->
    <el-table
      ref="multipleTable"
      v-loading="listLoading"
      :data="list"
      border
      :header-cell-style="{background:'#eef1f6',color:'#606266'}"
      fit
      highlight-current-row
      style="width: 100%;"
      max-height="450"
      @selection-change="handleSelectChange"
    >
      <el-table-column
        type="selection"
        width="40"
      />

      <el-table-column v-if="false" width="160px" align="center" prop="ResourceID" :label="$t('ui.Sys.Sys_Resource.ResourceID')" />
      <el-table-column width="160px" align="center" prop="FathResourceID" :label="$t('ui.Sys.Sys_Resource.FathResourceID')" />
      <el-table-column width="160px" align="center" prop="ResourceTitle" :label="$t('ui.Sys.Sys_Resource.ResourceTitle')" />
      <el-table-column width="160px" align="center" prop="ResourceName" :label="$t('ui.Sys.Sys_Resource.ResourceName')" />
      <el-table-column width="160px" align="center" prop="ResourceType" :label="$t('ui.Sys.Sys_Resource.ResourceType')" />
      <el-table-column width="160px" align="center" prop="RedirectUrl" :label="$t('ui.Sys.Sys_Resource.RedirectUrl')" />
      <el-table-column width="160px" align="center" prop="ResourcePath" :label="$t('ui.Sys.Sys_Resource.ResourcePath')" />
      <el-table-column width="160px" align="center" prop="ResourceLevel" :label="$t('ui.Sys.Sys_Resource.ResourceLevel')" />
      <el-table-column width="160px" align="center" prop="ResourceIcon" :label="$t('ui.Sys.Sys_Resource.ResourceIcon')" />
      <el-table-column width="160px" align="center" prop="IsCache" :label="$t('ui.Sys.Sys_Resource.IsCache')" />
      <el-table-column width="160px" align="center" prop="Component" :label="$t('ui.Sys.Sys_Resource.Component')" />
      <el-table-column width="160px" align="center" prop="Layout" :label="$t('ui.Sys.Sys_Resource.Layout')" />
      <el-table-column width="160px" align="center" prop="Role" :label="$t('ui.Sys.Sys_Resource.Role')" />
      <el-table-column width="160px" align="center" prop="AppID" :label="$t('ui.Sys.Sys_Resource.AppID')" />
      <el-table-column width="160px" align="center" prop="Remark" :label="$t('ui.Sys.Sys_Resource.Remark')" />
      <el-table-column width="160px" align="center" prop="CUser" :label="$t('ui.Sys.Sys_Resource.CUser')" />
      <el-table-column width="160px" align="center" prop="CTime" :label="$t('ui.Sys.Sys_Resource.CTime')" />
      <el-table-column width="160px" align="center" prop="MUser" :label="$t('ui.Sys.Sys_Resource.MUser')" />
      <el-table-column width="160px" align="center" prop="MTime" :label="$t('ui.Sys.Sys_Resource.MTime')" />

    </el-table>
    <!-- 分頁組件 -->
    <pagination
      v-show="total>0"
      :total="total"
      :page.sync="listQuery.PageNumber"
      :limit.sync="listQuery.PageSize"
      @pagination="getList"
    />

    <!-- 表單對話框 -->
    <el-dialog :title="formTitle" :visible.sync="dialogFormVisible" top="10vh">
      <el-form ref="dataForm" :rules="rules" :model="formData" label-position="left" label-width="70px">

        <el-form-item :label="$t('ui.Sys.Sys_Resource.FathResourceID')">
          <el-input v-model="formData.FathResourceID" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.ResourceTitle')">
          <el-input v-model="formData.ResourceTitle" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.ResourceName')">
          <el-input v-model="formData.ResourceName" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.ResourceType')">
          <el-input v-model="formData.ResourceType" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.RedirectUrl')">
          <el-input v-model="formData.RedirectUrl" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.ResourcePath')">
          <el-input v-model="formData.ResourcePath" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.ResourceLevel')">
          <el-input v-model="formData.ResourceLevel" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.ResourceIcon')">
          <el-input v-model="formData.ResourceIcon" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.IsCache')">
          <el-input v-model="formData.IsCache" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.Component')">
          <el-input v-model="formData.Component" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.Layout')">
          <el-input v-model="formData.Layout" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.Role')">
          <el-input v-model="formData.Role" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.AppID')">
          <el-input v-model="formData.AppID" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Resource.Remark')">
          <el-input v-model="formData.Remark" :autosize="{minRows: 3, maxRows: 5}" type="textarea" />
        </el-form-item>

      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">
          {{ $t('Common.cancel') }}
        </el-button>
        <el-button type="primary" @click="formMode==='Create'?createData():updateData()">
          {{ $t('Common.confirm') }}
        </el-button>
      </div>
    </el-dialog>

  </div>
</template>

<script>

import { fetchList, add, update, batchDelete } from '@/api/Sys/Sys_Resource'
// import { convertToKeyValue } from '@/utils'
import waves from '@/directive/waves' // waves directive 特效
import Pagination from '@/components/Pagination' // 分頁
// import { MessageBox } from 'element-ui' // 提示框

export default {
  name: 'Sys_Resource',
  components: { Pagination },
  directives: { waves },
  filters: {
    statusFilter(status) {
      const statusMap = {
        true: '正常',
        false: '凍結'
      }
      return statusMap[status]
    }
  },
  data() {
    return {
      list: null,
      selectedRowsData: [],
      total: 0,
      listLoading: true,
      listQuery: {
        keyword: '', // 用戶姓名 | 登錄賬號
        PageNumber: 1,
        PageSize: 10
      },
      formData: { // 表單數據
        ResourceID: '',
        FathResourceID: '',
        ResourceTitle: '',
        ResourceName: '',
        ResourceType: '',
        RedirectUrl: '',
        ResourcePath: '',
        ResourceLevel: '',
        ResourceIcon: '',
        IsCache: '',
        Component: '',
        Layout: '',
        Role: '',
        AppID: '',
        Remark: ''

      },
      formTitle: '', // 彈窗標題
      formMode: '',
      dialogFormVisible: false, // 表單窗口是否顯示標誌
      rules: { // 表單校驗邏輯
        UserName: [
          { required: true, message: '用戶名必須輸入', trigger: 'change' }
        ]
      },
      downloadLoading: false
    }
  },
  computed: {
    canNotUpdate() {
      var selectRows = this.selectedRowsData || []
      return selectRows.length !== 1
    }
  },
  created() {
    this.getList()
  },
  methods: {
    getList() {
      // 獲取數據
      this.listLoading = true
      fetchList(this.listQuery).then(response => {
        this.list = response.Data.items
        this.total = response.Data.total
        this.listLoading = false
      })
    },
    handleFilter() {
      this.getList()
    },
    resetFormData() {
      // 重置表單數據
      this.formData = {
        ResourceID: '',
        FathResourceID: '',
        ResourceTitle: '',
        ResourceName: '',
        ResourceType: '',
        RedirectUrl: '',
        ResourcePath: '',
        ResourceLevel: '',
        ResourceIcon: '',
        IsCache: '',
        Component: '',
        Layout: '',
        Role: '',
        AppID: '',
        Remark: ''

      }
    },
    handleSelectChange(selection) {
      this.selectedRowsData = selection
    },
    handleCreate() {
      this.resetFormData()
      this.formTitle = '添加'
      this.formMode = 'Create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate() // 清除校驗
      })
    },
    createData() {
      // 添加
      add(this.formData).then((response) => {
        // this.list.unshift(this.temp)
        this.dialogFormVisible = false
        this.$notify({
          title: '成功',
          message: '操作成功',
          type: 'success',
          duration: 2000
        })
        this.getList()
      })
    },
    handleUpdate() {
      this.resetFormData()
      this.formData = Object.assign({}, this.selectRows[0]) // 對象拷貝
      this.formTitle = '更新'
      this.formMode = 'Update'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate() // 清除校驗
      })
    },
    updateData() {
      update(this.formData).then(() => {
        this.getList()
        this.dialogFormVisible = false
        this.$notify({
          title: '成功',
          message: '操作成功',
          type: 'success',
          duration: 2000
        })
      })
    },
    handleDelete() {
      var selectRows = this.$refs.multipleTable.store.states.selection

      if (selectRows.length < 1) {
        this.$notify({
          title: '提示',
          message: '請至少選擇一行數據進行刪除',
          type: 'info',
          duration: 2000
        })
        return
      } else {
        console.log('batchDelete')
        this.$confirm('是否確認要刪除該行數據？', '提示', { confirmButtonText: '確定', cancelButtonText: '取消', type: 'warning' }).then(() => {
          var arrRowsID = selectRows.map(function(v) { return v.ResourceID })

          // 刪除邏輯處理
          batchDelete(arrRowsID).then(() => {
            this.$notify({
              title: '成功',
              message: '操作成功',
              type: 'success',
              duration: 2000
            })
            this.getList()
          })
        })
      }
    }
  }
}
</script>

