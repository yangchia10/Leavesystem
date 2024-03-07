<template>
  <div class="app-container">
    <!-- 检索以及工具条区域 -->
    <div class="filter-container">
      <el-input
        v-model="listQuery.keyword"
        :placeholder="$t('Common.keyword')"
        style="width: 220px;"
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
      <hr/>

      <el-button
        v-waves
        :loading="downloadLoading"
        class="filter-item"
        type="primary"
        icon="el-icon-download"
      >导出
      </el-button>
    </div>

    <!-- 数据列表 -->
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
    >
      <el-table-column
        type="selection"
        width="40" fixed
      />

      <el-table-column v-if="false" align="center" :label="$t('ui.Sys.Sys_Message.MessageID')">
        <template slot-scope="scope">
          <span>{{ scope.row.MessageID }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Message.MessageTypeID')">
        <template slot-scope="scope">
          <span>{{ scope.row.MessageTypeID }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="left" :label="$t('ui.Sys.Sys_Message.MessageTitle')">
        <template slot-scope="scope">
          <span>{{ scope.row.MessageTitle }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="left" :label="$t('ui.Sys.Sys_Message.MessageBody')">
        <template slot-scope="scope">
          <span>{{ scope.row.MessageBody }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Message.NotifyType')">
        <template slot-scope="scope">
          <span>{{ scope.row.NotifyType }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Message.MessagePublisher')">
        <template slot-scope="scope">
          <span>{{ scope.row.MessagePublisher }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Message.MessagePublishTime')"
                       :formatter="formatDate"/>
      <el-table-column width="120px" align="center" :label="$t('Common.Remark')">
        <template slot-scope="scope">
          <span>{{ scope.row.Remark }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('Common.CUser')">
        <template slot-scope="scope">
          <span>{{ scope.row.CUser }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" prop="CTime" :label="$t('Common.CTime')"
                       :formatter="formatDateTime"/>

    </el-table>
    <!-- 分页组件 -->
    <pagination
      v-show="total>0"
      :total="total"
      :page.sync="listQuery.PageNumber"
      :limit.sync="listQuery.PageSize"
      @pagination="getList"
    />

    <!-- 表单对话框 -->
    <el-dialog :title="formTitle" :visible.sync="dialogFormVisible" top="10vh">
      <el-form ref="dataForm" :rules="rules" :model="formData" label-position="left" label-width="70px">

        <el-form-item :label="$t('ui.Sys.Sys_Message.MessageTypeID')">
          <el-input v-model="formData.MessageTypeID" autocomplete="off"/>
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Message.MessageTitle')">
          <el-input v-model="formData.MessageTitle" autocomplete="off"/>
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Message.MessageBody')">
          <el-input v-model="formData.MessageBody" autocomplete="off"/>
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Message.NotifyType')">
          <el-input v-model="formData.NotifyType" autocomplete="off"/>
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Message.MessagePublisher')">
          <el-input v-model="formData.MessagePublisher" autocomplete="off"/>
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_Message.MessagePublishTime')">
          <el-input v-model="formData.MessagePublishTime" autocomplete="off"/>
        </el-form-item>
        <el-form-item :label="$t('Common.Remark')">
          <el-input v-model="formData.Remark" :autosize="{minRows: 3, maxRows: 5}" type="textarea"/>
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

import { fetchList, add, update, batchDelete } from '@/api/Sys/Sys_Message'
import { convertToKeyValue } from '@/utils'
import waves from '@/directive/waves' // waves directive 特效
import Pagination from '@/components/Pagination' // 分页
import { formatDate, formatDateTime } from '../../../utils'

export default {
  name: 'SysMessage',
  components: { Pagination },
  directives: { waves },
  filters: {
    statusFilter(status) {
      const statusMap = {
        true: '正常',
        false: '冻结'
      }
      return statusMap[status]
    }
  },
  data() {
    return {
      list: null,
      total: 0,
      listLoading: true,
      listQuery: {
        keyword: '', // 用户姓名 | 登录账号
        PageNumber: 1,
        PageSize: 10
      },
      formData: { // 表单数据
        MessageID: '',
        MessageTypeID: '',
        MessageTitle: '',
        MessageBody: '',
        NotifyType: '',
        MessagePublisher: '',
        MessagePublishTime: '',
        Remark: ''

      },
      formTitle: '', // 弹窗标题
      formMode: '',
      dialogFormVisible: false, // 表单窗口是否显示标志
      rules: { // 表单校验逻辑
        UserName: [
          { required: true, message: '用户名必须输入', trigger: 'change' }
        ]
      },
      downloadLoading: false
    }
  },
  created() {
    this.getList()
  },
  methods: {
    formatDate,
    formatDateTime,
    getList() {
      // 获取数据
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
      // 重置表单数据
      this.formData = {
        MessageID: '',
        MessageTypeID: '',
        MessageTitle: '',
        MessageBody: '',
        NotifyType: '',
        MessagePublisher: '',
        MessagePublishTime: '',
        Remark: ''

      }
    }
    /* handleCreate() {
      this.resetFormData()
      this.formTitle = '添加'
      this.formMode = 'Create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate() // 清除校验
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
      var selectRows = this.$refs.multipleTable.store.states.selection
      if (selectRows.length > 1) {
        this.$notify({
          title: '提示',
          message: '编辑时只能选中一行数据',
          type: 'info',
          duration: 2000
        })
        return
      } else if (selectRows.length == 0) {
        this.$notify({
          title: '提示',
          message: '编辑时请先选中一行数据',
          type: 'info',
          duration: 2000
        })
        return
      } else {
        this.resetFormData()
        this.formData = Object.assign({}, selectRows[0]) // 对象拷贝
        this.formTitle = '更新'
        this.formMode = 'Update'
        this.dialogFormVisible = true
        this.$nextTick(() => {
          this.$refs['dataForm'].clearValidate() // 清除校验
        })
      }
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
          message: '请至少选择一行数据进行删除',
          type: 'info',
          duration: 2000
        })
        return
      } else {
        console.log('batchDelete')
        this.$confirm('是否确认要删除该行数据？', '提示', { confirmButtonText: '确定', cancelButtonText: '取消', type: 'warning' }).then(() => {
          var arrRowsID = selectRows.map(function(v) { return v.MessageID })

          // 删除逻辑处理
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
    }*/
  }
}
</script>

