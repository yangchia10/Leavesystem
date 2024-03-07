<template>
  <div class="app-container">
    <!-- 检索以及工具条区域 -->
    <div class="filter-container">
      <el-date-picker
        v-model="listQuery.dateValue"
        class="filter-item"
        type="daterange"
        align="right"
        range-separator="-"
        :start-placeholder="$t('Common.startTime')"
        :end-placeholder="$t('Common.endTime')"
        style="width: 250px"
        unlink-panels
      />

      <el-input
        v-model="listQuery.keyword"
        :placeholder="$t('ui.Sys.Sys_Log.Placeholder.SearchCondition')"
        style="width: 220px;"
        class="filter-item"
        @keyup.enter.native="handleFilter"
      />


      <el-button
        v-waves
        class="filter-item"
        type="primary"
        icon="el-icon-search"
        :disabled="handleDisable"
        @click="handleFilter"
      />
      <hr/>
      <!-- v-permission="{ name: 'PP.PP_FTTP.Export' }" -->
      <el-button
        v-waves
        class="filter-item"
        type="primary"
        icon="el-icon-download"
        @click="handleExport"
      >{{ $t('Common.export') }}
      </el-button>

      <!-- v-permission="{ name: 'Sys.Sys_Log.Delete' }" -->
      <el-button
        v-waves
        class="filter-item"
        type="danger"
        icon="el-icon-delete"
        :disabled="canNotDelete"
        @click="handleDelete"
      >{{ $t('Common.delete') }}
      </el-button>
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
      :height="tableHeight"
      @selection-change="handleSelectChange"
    >
      <el-table-column type="selection" align="center" width="40" fixed/>
      <el-table-column
        label="#"
        type="index"
        align="center"
        width="40"
        fixed
      />
      <el-table-column v-if="false" align="center" :label="$t('ui.Sys.Sys_Log.LogID')">
        <template slot-scope="scope">
          <span>{{ scope.row.LogID }}</span>
        </template>
      </el-table-column>

      <el-table-column
        v-if="false"
        width="120px"
        align="center"
        :label="$t('ui.Sys.Sys_Log.LogType')"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.LogType }}</span>
        </template>
      </el-table-column>
      <el-table-column
        v-if="false"
        width="120px"
        align="center"
        :label="$t('ui.Sys.Sys_Log.ApiType')"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.ApiType }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Log.ApiModule')">
        <template slot-scope="scope">
          <span>{{ scope.row.ApiModule }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Log.ApiDescription')">
        <template slot-scope="scope">
          <span>{{ scope.row.ApiDescription }}</span>
        </template>
      </el-table-column>

      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Log.OperateUser')">
        <template slot-scope="scope">
          <span>{{ scope.row.OperateUser }}</span>
        </template>
      </el-table-column>
      <el-table-column width="250px" align="center" :label="$t('ui.Sys.Sys_Log.ApiUrl')" show-overflow-tooltip>
        <template slot-scope="scope">
          <span>{{ scope.row.ApiUrl }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Log.ClientHost')" show-overflow-tooltip>
        <template slot-scope="scope">
          <span>{{ scope.row.ClientHost }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Log.ClientIP')" show-overflow-tooltip>
        <template slot-scope="scope">
          <span>{{ scope.row.ClientIP }}</span>
        </template>
      </el-table-column>
      <el-table-column width="140px" align="center" :label="$t('ui.Sys.Sys_Log.ClientBrowser')" show-overflow-tooltip>
        <template slot-scope="scope">
          <span>{{ scope.row.ClientBrowser }}</span>
        </template>
      </el-table-column>
      <el-table-column width="240px" align="center" :label="$t('ui.Sys.Sys_Log.ClientOS')">
        <template slot-scope="scope">
          <span>{{ scope.row.ClientOS }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Log.RequestType')">
        <template slot-scope="scope">
          <span>{{ scope.row.RequestType }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Log.RequestParms')">
        <template slot-scope="{ row }">
          <el-button type="primary" size="mini" @click="showJsonView(row.RequestParms)">查看</el-button>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Log.ResponseData')">
        <template slot-scope="{ row }">
          <el-button type="primary" size="mini" @click="showJsonView(row.ResponseData)">查看</el-button>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Log.Result')">
        <template slot-scope="scope">
          <span v-if="scope.row.Result === '成功'" style="color:green;">
            {{scope.row.Result}}
          </span>
          <span v-else style="color:red;">{{ scope.row.Result }}</span>
        </template>
      </el-table-column>
      <el-table-column v-if="false" width="120px" align="center" :label="$t('Common.Remark')">
        <template slot-scope="scope">
          <span>{{ scope.row.Remark }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('Common.CUser')">
        <template slot-scope="scope">
          <span>{{ scope.row.CUser }}</span>
        </template>
      </el-table-column>
      <el-table-column
        width="160px"
        align="center"
        prop="CTime"
        :label="$t('Common.CTime')"
        :formatter="formatDateTime"
      />
      <el-table-column width="120px" align="center" :label="$t('Common.MUser')">
        <template slot-scope="scope">
          <span>{{ scope.row.MUser }}</span>
        </template>
      </el-table-column>
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

    <el-dialog v-el-drag-dialog title="查看" :visible.sync="dialogJsonViewerVisible" top="10vh">
      <json-viewer :value="jsonData"/>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogJsonViewerVisible = false">{{ $t('Common.cancel') }}</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import elDragDialog from '@/directive/el-drag-dialog' // base on element-ui

import {
  fetchList,
  add,
  update,
  batchDelete,
  exportExcelFile
} from '@/api/Sys/Sys_Log'
import { convertToKeyValue } from '@/utils'
import waves from '@/directive/waves' // waves directive 特效
import Pagination from '@/components/Pagination' // 分页
import { MessageBox } from 'element-ui' // 提示框
import JsonViewer from 'vue-json-viewer'
import { formatDateTime } from '../../../utils'
import { exportToExcel } from '@/utils/excel-export'

export default {
  name: 'Sys.Sys_Log',
  components: { Pagination, JsonViewer },
  directives: { waves, elDragDialog },
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
      handleDisable: false,
      list: null,
      total: 0,
      listLoading: true,
      listQuery: {
        keyword: '', // 模块名称 | 功能描述
        dateValue: [
          new Date(new Date().setDate(new Date().getDate() - 7)),
          new Date()
        ], // 最后需要加入查询条件listQuery
        PageNumber: 1,
        PageSize: 10
      },
      selectedRowsData: [],
      dialogJsonViewerVisible: false, // JSON Viewer对话框空值
      jsonData: null,
      tableHeight: 330
    }
  },
  computed: {
    canNotDelete() {
      return this.selectedRowsData.length < 1
    }
  },
  created() {
    this.getList()
  },
  mounted() {
    this.$nextTick(function() {
      this.tableHeight = window.innerHeight - 330
      const self = this
      window.onresize = function() {
        self.tableHeight = window.innerHeight - self.$refs.multipleTable.$el.offsetTop - 110
      }
    })
  },
  methods: {
    formatDateTime,
    getList() {
      // 获取数据
      this.listLoading = true
      fetchList(this.listQuery).then(response => {
        this.list = response.Data.items
        this.total = response.Data.total
        this.listLoading = false
        this.handleDisable = false
      })
    },
    handleFilter() {
      this.handleDisable = true
      console.log('this.handleDisable')
      this.$nextTick(() => {
        this.getList()
      })
    },
    showJsonView(jsonValue) {
      this.dialogJsonViewerVisible = true
      this.jsonData = JSON.parse(jsonValue)
    },
    handleSelectChange(selection) {
      this.selectedRowsData = selection
    },
    handleDelete() {
      this.$confirm(
        this.$i18n.t('Common.batchDeletingConfirm'),
        this.$i18n.t('Common.tip'),
        {
          confirmButtonText: this.$i18n.t('Common.affirm'),
          cancelButtonText: this.$i18n.t('Common.cancel'),
          type: 'warning'
        }
      ).then(() => {
        console.log('this.selectedRowsData', this.selectedRowsData)
        var arrRowsID = this.selectedRowsData.map(v => v.LogID)

        // 删除逻辑处理
        batchDelete(arrRowsID).then(res => {
          if (res.Code === 2000) {
            this.showNotify('success', 'Common.deleteSuccess')
          } else {
            this.showNotify('error', res.Message)
          }
          this.handleFilter()
        })
      })
    },
    handleExport() {
      // eslint-disable-next-line no-undef ExportToExcelFile
      exportExcelFile({
        keyword: this.listQuery.keyword,
        dateValue: this.listQuery.dateValue
      }).then(res => exportToExcel(res.data, res.fileName))
    }
  }
}
</script>

