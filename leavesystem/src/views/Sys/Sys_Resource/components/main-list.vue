<template>
  <div class="app-container">
    <!-- 檢索以及工具條區域 -->
    <div class="filter-container-2">

      <!-- <el-button class="filter-item" icon="el-icon-plus" type="success" @click="openAdd()">
        新增
      </el-button>
      <el-button class="filter-item" icon="el-icon-edit" type="primary" :disabled="singleFlag" @click="openEdit()">
        編輯
      </el-button> -->
      <el-button class="filter-item" icon="el-icon-plus" type="info" @click="openEdit()">
        轉入大分類資料
      </el-button>
      <el-button class="filter-item" icon="el-icon-plus" type="info" @click="openEdit()">
        轉入中分類資料
      </el-button>
      <el-button class="filter-item" icon="el-icon-plus" type="info" @click="SyncOITM()">
        轉入商品資料
      </el-button>
      <el-button class="filter-item" icon="el-icon-plus" type="info" @click="openEdit()">
        轉入商品國際條碼
      </el-button>
      <el-button class="filter-item" icon="el-icon-plus" type="info" @click="openEdit()">
        轉入商品配方檔
      </el-button>
      <el-button class="filter-item" icon="el-icon-plus" type="info" @click="openEdit()">
        轉入分店資料
      </el-button>
      <el-button class="filter-item" icon="el-icon-plus" type="info" @click="openEdit()">
        轉入倉庫資料
      </el-button>
      <el-button class="filter-item" icon="el-icon-plus" type="info" @click="openEdit()">
        轉入員工資料
      </el-button>

      <hr>
    </div>
    <el-table
      ref="multipleTable"
      v-loading="table.loading"
      :data="table.data"
      :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
      max-height="300"
      border
      fit
      highlight-current-row
      @row-click="handleTableRowClick"
      @selection-change="handleSelectionChange"
    >
      <!--<el-table-column type="selection" width="40" />

      <el-table-column width="160px" align="center" prop="ResourceID" label="ResourceID" />  :label="$t('Common.select')"
      <el-table-column width="160px" align="center" prop="FathResourceID" label="FathResourceID" />
      <el-table-column width="160px" align="center" prop="Remark" label="備註" />-->

    </el-table>
    <!-- 分頁 -->
    <pagination
      v-show="table.total > 0"
      :page.sync="page.pageNumber"
      :limit.sync="page.pageSize"
      :page-sizes="[5, 10, 20, 30, 50]"
      :total="table.total"
      @pagination="fetchList"
    />
    <!-- 編輯彈窗 -->
    <MainEdit ref="MainEdit" @fetch-list="fetchList" />
  </div>
</template>

<script>

import MainEdit from './main-edit.vue'
import { tableQuery } from '@/mixins'
import { GetPageList as GetList } from '@/api/Sys/Sys_Resource'

import { SyncOITM } from '@/api/Sys/Sys_Resource'

/**
 * 系統管理/菜單管理
 */
export default {
  name: 'Sys_Resource',
  components: { MainEdit },
  mixins: [tableQuery],
  data() {
    return {

      // 權限
      permission: {
      },
      // 接口
      property: {
        // action: '/MD_ProcessRoute/AddMain'// 上傳文件
      },
      // 查詢條件
      query: {
        'FathResourceID': ''
      },
      GetList // 查詢接口
    }
  },
  mounted() {
    this.fetchList()
  },
  methods: {
    /**
     * 重寫行點擊
     */
    handleTableRowClick(props) {
      console.log(1234)
      this.$emit('click-main', props)
    },
    /**
     * 點擊編輯
     */
    openAdd() {
      this.$refs.MainEdit.open({ type: 1 })
    },
    openEdit() {
      const data = this.multipleSelection[0]
      this.$refs.MainEdit.open({ type: 2, data })
    },
    SyncOITM() {
      console.log(1234)
      SyncOITM()
      alert('轉入完成')
    }
  }
}
</script>

