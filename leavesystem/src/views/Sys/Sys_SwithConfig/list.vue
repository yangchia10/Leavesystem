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
      <hr >

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_SwithConfig.Enable' }"
        class="filter-item"
        type="success"
        icon="el-icon-plus"
        :disabled="SwitchCanEnable"
        @click="handleEnable"
      >{{ $t("Common.Enable") }}</el-button>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_SwithConfig.Disable' }"
        class="filter-item"
        type="danger"
        :disabled="SwitchCanDisable"
        icon="el-icon-edit"
        @click="handleDisable"
      >{{ $t("Common.Disable") }}</el-button>
    </div>

    <!-- 数据列表 -->
    <el-table
      ref="multipleTable"
      v-loading="listLoading"
      :data="list"
      border
      :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
      fit
      highlight-current-row
      style="width: 100%;"
      max-height="450"
      @selection-change="handleSelectChange"
    >
      <el-table-column type="selection" width="40" fixed />

      <el-table-column
        v-if="false"
        width="160px"
        align="center"
        prop="ConfigID"
        :label="$t('ui.Sys.Sys_SwithConfig.ConfigID')"
      />

      <el-table-column
        width="160px"
        align="center"
        prop="ConfigCode"
        :label="$t('ui.Sys.Sys_SwithConfig.ConfigCode')"
      />

      <el-table-column
        width="240px"
        align="center"
        prop="ConfigDesc"
        :label="$t('ui.Sys.Sys_SwithConfig.ConfigDesc')"
      />
      <el-table-column
        width="160px"
        align="center"
        prop="SwitchValue"
        :label="$t('ui.Sys.Sys_SwithConfig.SwitchValue')"
      >
        <template slot-scope="scope">
          <el-switch
            v-model="scope.row.SwitchValue"
            active-color="#13ce66"
            inactive-color="#ff4949"
            disabled
          />
        </template> </el-table-column><!--开关值若无法实时保存到数据库，就取消switch组件，只显示状态，只能通过开启关闭按钮控制开关-->
      <el-table-column
        v-if="false"
        width="260px"
        align="center"
        prop="Remark"
        :label="$t('Common.Remark')"
      />

      <el-table-column
        width="160px"
        align="center"
        prop="MUser"
        :label="$t('Common.MUser')"
      />
      <el-table-column
        width="160px"
        align="center"
        prop="MTime"
        :label="$t('Common.MTime')"
        :formatter="formatDateTime"
      />
    </el-table>

    <!-- 分页组件 -->
    <pagination
      v-show="total > 0"
      :total="total"
      :page.sync="listQuery.PageNumber"
      :limit.sync="listQuery.PageSize"
      @pagination="getList"
    />
  </div>
</template>

<script>
import { fetchList, add, update, batchDelete } from '@/api/Sys/Sys_SwithConfig'
import { convertToKeyValue } from '@/utils'
import waves from '@/directive/waves' // waves directive 特效
import Pagination from '@/components/Pagination' // 分页
import { MessageBox } from 'element-ui' // 提示框
import { formatDate, formatDateTime } from '@/utils' // 列表内容格式化
import permission from '../../../directive/permission/permission'

export default {
  name: 'Sys.Sys_SwithConfig',
  components: { Pagination },
  directives: { waves, permission },
  data() {
    return {
      list: null,
      selectedRowsData: [],
      total: 0,
      listLoading: true,
      listQuery: {
        keyword: '', // 用户姓名 | 登录账号
        PageNumber: 1,
        PageSize: 10
      },
      downloadLoading: false
    }
  },
  computed: {
    SwitchCanEnable() {
      var selectRows = this.selectedRowsData || []
      return selectRows.length !== 1 || (selectRows.length === 1 && selectRows[0].SwitchValue)
    },
    SwitchCanDisable() {
      var selectRows = this.selectedRowsData || []
      return selectRows.length !== 1 || (selectRows.length === 1 && !selectRows[0].SwitchValue)
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
    handleSelectChange(selection) {
      this.selectedRowsData = selection
      // console.log(selection)
    },
    handleEnable() {
      // 启用
      var selectRow = this.selectedRowsData[0]
      selectRow.SwitchValue = true // 启用
      update(selectRow).then(() => {
        this.getList()
        this.dialogFormVisible = false
        this.$notify({
          title: this.$t('Common.success'),
          message: this.$t('Common.operationSuccess'),
          type: 'success',
          duration: 2000
        })
      })
    },
    handleDisable() {
      // 停用
      var selectRow = this.selectedRowsData[0]
      selectRow.SwitchValue = false // 停用
      update(selectRow).then(() => {
        this.getList()
        this.dialogFormVisible = false
        this.$notify({
          title: this.$t('Common.success'),
          message: this.$t('Common.operationSuccess'),
          type: 'success',
          duration: 2000
        })
      })
    }

  }
}
</script>

