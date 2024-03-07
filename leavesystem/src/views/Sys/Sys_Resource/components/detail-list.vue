/* eslint-disable no-unused-vars */
<template>
  <div class="app-container">
    <!-- 檢索以及工具條區域 -->
    <div class="filter-container-1">
      <!-- <el-select v-model="query.SectionCode" class="filter-item filter-item-input" placeholder="工段" clearable>
        <el-option v-for="item in dicts.level2" :key="item.value" :label="item.label" :value="item.value" />
      </el-select> -->
      <!-- <el-button class="filter-item" icon="el-icon-search" type="primary" @click="fetchList()" />
      <br /> -->
      <up-file v-permission="{ name:'MD.MD_Permission.onloadDetail'}" right :action="property.action" @fetc-list="fetchList" />
      <el-button permission="MD.MD_Permission.Add" class="filter-item" icon="el-icon-plus" type="success" :disabled="!mainData" @click="openAdd()">
        新增
      </el-button>

      <el-button
        class="filter-item"
        type="primary"
        icon="el-icon-edit"
        :disabled="singleFlag"
        @click="openEdit()"
      >
        編輯
      </el-button>
      <el-button class="filter-item" type="danger" icon="el-icon-delete" :disabled="multipleFlag" @click="handleDelete">
        刪除
      </el-button>

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
      <el-table-column v-if="value11" width="160px" align="center" prop="ItmsGrpCod" label="大類" /> <!-- :label="$t('Common.select')" -->
      <el-table-column width="160px" align="center" prop="ItemCode" label="項目號碼" />
      <el-table-column width="160px" align="center" prop="ItemName" label="項目名稱" />
      <el-table-column width="160px" align="center" prop="Quantity" label="數量" />
      <el-table-column width="160px" align="center" prop="UomCode" label="單位" />
      <el-table-column width="160px" align="center" prop="Cause1" label="瑕疵原因(初判)" />
      <el-table-column width="160px" align="center" prop="ItemCode" label="固定資產編號 " />
      <el-table-column width="160px" align="center" prop="Cause2" label="總部提報維修原因" />
      <el-table-column width="160px" align="center" prop="Deal" label="外部修繕/內部維修" />
      <el-table-column width="160px" align="center" prop="Final" label="最終處理結果" />
      <el-table-column width="160px" align="center" prop="Remark" label="維修內容描述" />

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

    <!-- 編輯 -->
    <detail-edit ref="detailEdit" @fetch-list="fetchList" />

  </div>
</template>

<script>
import upFile from '@/views/Components/upfile'
import detailEdit from './detail-edit.vue'

import { tableQuery } from '@/mixins'
import { GetDetailPageList as GetList, DeleteDetail as Delete } from '@/api/MD/MD_ProcessRoute'
/* import { fetchList as getDicts } from '@/api/MD/MD_Process' */

/**
 * 生產管理/銷售需求計劃
 */
export default {

  name: 'Plan_BreakSchedule',
  components: { upFile, detailEdit },

  mixins: [tableQuery],
  data() {
    return {
      // 權限
      permission: {
        onload: 'MD.MD_ProcessRoute.onloadDetail', // 導入
        edit: 'MD.MD_ProcessRoute.edit' // 導入
      },
      // 接口
      property: {
        action: '/MD_ProcessRoute/AddDetail'
      },
      // 查詢條件
      query: {
        RouteCode: '', // 工藝路線編號
        RowUid: '', // 工藝路線編號
        SectionCode: '' // 工藝路線編號
      },
      // 選中主表資料
      mainData: null,
      GetList, // 查詢接口
      Delete // 刪除
    }
  },
  computed: {

    value11: function() {
      // const userff = this.$store.state.user.userinfo ? this.$store.state.user.userinfo : JSON.parse(sessionStorage.getItem('userinfo'))
      // if (userff.UserRole == 1) {
      //   console.log('serff123')
      //   return false
      // } else {
      //   return true
      // }
      return true
    }

  },
  mounted() {
    /* // 查詢字典
    getDicts().then(res => {
      // 工序
      const list = res.Data || []
      this.dicts.level2 = list.map(e => ({
        value: e.ProcessCode,
        label: e.ProcessName
      }))
    }) */
    /* this.fetchList() */
  },

  methods: {
    /**
     * 加載資料
     */
    loading(props) {
      this.mainData = props
      this.query.RouteCode = props.RouteCode
      this.query.RowUid = props.RowUid
      this.query.SectionCode = props.SectionCode
      const userff = this.$store.state.user.userinfo ? this.$store.state.user.userinfo : JSON.parse(sessionStorage.getItem('userinfo'))
      console.log(userff)
      if (userff.UserRole == 1) {
        console.log('serff')
      }
      this.fetchList()
      this.$emit('click-main', null)
    },
    /**
     * 點擊編輯
     */
    openAdd() {
      const main = {
        WorkShopCode: this.mainData.WorkShopCode,
        RouteCode: this.mainData.RouteCode,
        RouteName: this.mainData.RouteName,
        ItemCode: this.mainData.ItemCode,
        Headid: this.mainData.RowUid,
        ItemName: this.mainData.ItemName
      }
      this.$refs.detailEdit.open({ type: 1, main })
    },
    openEdit() {
      // console.log('openEdit')
      const data = this.multipleSelection[0]
      // console.log(data, 'openEdit')
      const main = {
        WorkshopCode: this.mainData.WorkShopCode,
        RouteCode: this.mainData.RouteCode,
        RouteName: this.mainData.RouteName,
        ItemCode: this.mainData.ItemCode,
        ItemName: this.mainData.ItemName,
        Headid: this.mainData.RowUid
      }
      this.$refs.detailEdit.open({ type: 2, data, main })
    },
    handleTableRowClick(props) {
      // console.log(123)
      this.$emit('click-main', props)
    },
    handleDelete(props) {
      // console.log(123)
      // props.RowUid = null
      // this.$emit('click-main', props)
      this.$deleteConfirm(this.$i18n.t('Dictionary.Operate.Isdelete')).then(() => {
        const list = this.multipleSelection
        if (!this.Delete) return console.log('【刪除方法未引用】')
        this.Delete(list).then(res => {
          this.fetchList()
          props = null
          this.$emit('click-main', props)
        })
      })
    }

  }
}
</script>
