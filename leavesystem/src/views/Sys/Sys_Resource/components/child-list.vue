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
      <!-- <up-file v-permission="{ name:'MD.MD_Permission.onloadDetail'}" right :action="property.action" @fetc-list="fetchList" /> -->
      <el-button
        v-waves
        class="filter-item"
        icon="el-icon-plus"
        type="success"
        :disabled="!mainData"
        @click="openAdd()"
      >
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
      <el-table-column v-if="value1" width="160px" align="center" prop="ItemCode" label="使用材料/工資" /> <!-- :label="$t('Common.select')" -->
      <el-table-column v-if="value2" width="160px" align="center" prop="Quantity" label="數量" />
      <el-table-column v-if="value3" width="160px" align="center" prop="Price" label="金額" />
      <el-table-column v-if="value4" width="160px" align="center" prop="Total" label="列總計" />

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
    <child-edit ref="childEdit" @fetch-list="fetchList" />

  </div>
</template>

<script>
// import upFile from '@/views/Components/upfile'
import childEdit from './child-edit.vue'
import permission from '@/directive/permission/index.js' // 权限判断指令
import { tableQuery } from '@/mixins'
import { GetchildPageList as GetList, Deletechild as Delete } from '@/api/MD/MD_ProcessRoute'

/* import { fetchList as getDicts } from '@/api/MD/MD_Process' */

/**
 * 生產管理/銷售需求計劃
 */
export default {

  name: 'Plan_BreakSchedule',
  components: { childEdit },
  directives: {
    permission
  },

  mixins: [tableQuery],
  data() {
    return {
      showWorkflow: true,
      // 選中上層資料
      mainData: null,

      // 權限
      permission: {
        // onload: 'Equ.Permission.Test', // 導入
        // edit: 'MD.MD_ProcessRoute.edit' // 導入
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

      GetList, // 查詢接口
      Delete // 刪除
    }
  },
  computed: {
    // canNotUpdate() {
    //   var selectRows = this.selectedRowsData || []
    //   return selectRows.length !== 1
    // }
    value1: function() {
      return true
    },
    value2: function() {
      return true
    },
    value3: function() {
      return true
    },
    value4: function() {
      return true
    }

  },
  created() {

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
      if (props == null) {
        this.query.Detailid = ''
        this.query.Headid = ''
        this.fetchList()
        const userff = this.$store.state.user.userinfo ? this.$store.state.user.userinfo : JSON.parse(sessionStorage.getItem('userinfo'))
        console.log(userff)
      } else {
        this.query.Detailid = props.RowUid
        this.query.Headid = props.Headid
        this.fetchList()
        const userff = this.$store.state.user.userinfo ? this.$store.state.user.userinfo : JSON.parse(sessionStorage.getItem('userinfo'))
        console.log(userff)
      }
    },
    /**
     * 點擊編輯
     */
    openAdd() {
      const main = {
        Headid: this.mainData.Headid,
        Detailid: this.mainData.RowUid
      }
      this.$refs.childEdit.open({ type: 1, main })
    },
    openEdit() {
      // console.log('openEdit')
      const data = this.multipleSelection[0]
      // console.log(data, 'openEdit')
      const main = {
        Headid: this.mainData.Headid,
        Detailid: this.mainData.RowUid
      }
      this.$refs.childEdit.open({ type: 2, data, main })
    }
  }
}
</script>
