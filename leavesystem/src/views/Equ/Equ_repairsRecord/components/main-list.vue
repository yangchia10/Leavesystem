<template>
  <div class="app-container">
    <!-- 檢索以及工具條區域 -->
    <div class="filter-container-2">

      <!-- <el-input v-model="query.CardName" class="filter-item filter-item-input" placeholder="名稱" /> -->
      <el-select v-model="query.CardCode" class="filter-item filter-item-input" clearable placeholder="代碼">
        <el-option v-for="item in dicts" :key="item.CardCode" :label="item.CardCode" :value="item.CardCode" />
      </el-select>
      <!-- <el-select v-model="query.WorkShop" class="filter-item filter-item-input" clearable placeholder="叫修人員">
        <el-option v-for="item in dicts" :key="item.value" :label="item.label" :value="item.value" />
      </el-select> -->
      <el-button class="filter-item" icon="el-icon-search" type="primary" @click="fetchList()" />
      <br>
      <up-file permission="MD.MD_Permission.onloadMain" right :action="property.action" @fetc-list="fetchList" />
      <el-button class="filter-item" icon="el-icon-plus" type="success" @click="openAdd()">
        新增
      </el-button>
      <el-button class="filter-item" type="primary" icon="el-icon-edit" :disabled="singleFlag" @click="openEdit()">
        編輯
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
      <el-table-column type="selection" width="40" />

      <el-table-column width="160px" align="center" prop="CardCode" label="門市代號" /> <!-- :label="$t('Common.select')" -->
      <el-table-column width="160px" align="center" prop="CardName" label="門市名稱" />
      <el-table-column width="160px" align="center" prop="CUser" label="叫修人員" />
      <el-table-column width="160px" align="center" prop="DocNum" label="叫修單號" />
      <!-- <el-table-column width="160px" align="center" prop="DocStatus" label="叫修單狀態" /> -->
      <el-table-column width="160px" align="center" prop="USER_CODE" label="審核人員 " />
      <!-- <el-table-column width="160px" align="center" prop="CheckStatus" label="審核狀態" /> -->
      <el-table-column width="160px" align="center" prop="SAPDocNum" label="SAP單號" />
      <el-table-column width="160px" align="center" prop="CTime" label="叫修日期" />
      <el-table-column width="160px" align="center" prop="Remark" label="備註" />

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
import upFile from '@/views/Components/upfile'
import MainEdit from './main-edit.vue'
import { tableQuery } from '@/mixins'
import { GetPageList as GetList } from '@/api/MD/MD_ProcessRoute'
import { GetOCRDList } from '@/api/MD/MD_ProcessRoute'

/**
 * 生產管理/銷售需求計劃
 */
export default {
  name: 'Plan_BreakSchedule',
  components: { upFile, MainEdit },
  mixins: [tableQuery],
  data() {
    return {

      // 權限
      permission: {
        onload: 'MD.MD_ProcessRoute.onloadMain' // 導入
      },
      // 接口
      property: {
        action: '/MD_ProcessRoute/AddMain'
      },
      // 查詢條件
      query: {
        CardCode: ''

      },
      dicts: [],
      GetList // 查詢接口
    }
  },
  created() {
    this.GetWarehouseAll()
  },

  mounted() {
    this.fetchList()
  },

  methods: {
    /**
     * 重寫行點擊
     */
    handleTableRowClick(props) {
      // console.log(props)

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
    // 获取仓库列表
    GetWarehouseAll() {
      GetOCRDList().then(res => {
        if (res.Code === 2000) {
          this.dicts = res.Data

          this.fetchList()
        }
      })
    }
  }
}
</script>

