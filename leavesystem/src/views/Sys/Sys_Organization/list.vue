<template>
  <div class="app-container">
    <!-- 检索以及工具条区域 -->
    <div class="filter-container">
      <!-- <el-input
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
      />-->
      <!-- <hr > -->
      <!-- v-permission="{ name: 'Sys.Sys_Organization.Add' }" -->
      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_Organization.Add' }"
        class="filter-item"
        type="primary"
        icon="el-icon-plus"
        @click="handleCreate"
      >{{ $t('Common.add') }}
      </el-button>

      <!-- v-permission="{ name: 'Sys.Sys_Organization.Edit' }" -->
      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_Organization.Edit' }"
        class="filter-item"
        type="primary"
        :disabled="canNotUpdate"
        icon="el-icon-edit"
        @click="handleUpdate"
      >{{ $t('Common.edit') }}
      </el-button>
      <!-- v-permission="{ name: 'Sys.Sys_Organization.Delete' }" -->
      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_Organization.Delete' }"
        class="filter-item"
        type="danger"
        icon="el-icon-delete"
        :disabled="deleteButtonDisable"
        @click="handleDelete"
      >{{ $t('Common.delete') }}
      </el-button>
    </div>

    <!-- 数据列表.filter(data => !listQuery.keyword || data.OrganizationDesc.toLowerCase().includes(listQuery.keyword.toLowerCase())) -->
    <el-table
      ref="multipleTable"
      v-loading="listLoading"
      :data="list"
      row-key="OrganizationID"
      :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
      border
      :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
      fit
      highlight-current-row
      style="width: 100%;margin-bottom: 20px;"
      :height="tableHeight"
      @selection-change="handleSelectChange"
      @row-click="handelSelectRow"
    >
      <el-table-column type="selection" width="40"/>
      <el-table-column
        label="#"
        type="index"
        align="center"
        width="40"
        fixed
      />
      <el-table-column
        v-if="false"
        align="center"
        prop="OrganizationID"
        :label="$t('ui.Sys.Sys_Organization.OrganizationID')"
      />
      <el-table-column
        v-if="false"
        width="120px"
        align="center"
        prop="FathOrganizationID"
        :label="$t('ui.Sys.Sys_Organization.FathOrganizationID')"
      />
      <el-table-column
        width="150px"
        align="center"
        prop="OrganizationCode"
        :label="$t('ui.Sys.Sys_Organization.OrganizationCode')"
      />
      <el-table-column
        v-if="false"
        width="120px"
        align="center"
        prop="OrganizationLevel"
        :label="$t('ui.Sys.Sys_Organization.OrganizationLevel')"
      />
      <el-table-column
        width="280px"
        align="center"
        prop="OrganizationDesc"
        :label="$t('ui.Sys.Sys_Organization.OrganizationDesc')"
      />
      <el-table-column width="120px" align="center" prop="Remark" :label="$t('Common.Remark')"/>
      <el-table-column width="120px" align="center" prop="CUser" :label="$t('Common.CUser')"/>
      <el-table-column
        width="160px"
        align="center"
        prop="CTime"
        :label="$t('Common.CTime')"
        :formatter="formatDateTime"
      />
    </el-table>

    <!-- 分页组件 -->
    <!-- <pagination
      v-show="total>0"
      :total="total"
      :page.sync="listQuery.PageNumber"
      :limit.sync="listQuery.PageSize"
      @pagination="getList"
    />-->

    <!-- 表单对话框 -->
    <el-dialog
      v-el-drag-dialog
      :title="formTitle"
      :visible.sync="dialogFormVisible"
      top="8vh"
      width="65%"
    >
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="formData"
        label-position="left"
        label-width="120px"
      >
        <el-form-item :label="$t('ui.Sys.Sys_Organization.FathOrganizationID')">
          <!-- <el-input v-model="formData.FathOrganizationID" autocomplete="off" /> -->
          <!-- <el-tree class="filter-tree" :data="list" :props="defaultProps" :filter-node-method="filterNode" ref="FathOrganizationTree"></el-tree> -->

          <!-- <treeselect v-model="treeSelectedValue" :multiple="false" :options="treeOptions" placeholder="请选择上级部门" /> -->
          <treeselect
            v-model="formData.FathOrganizationID"
            :default-expand-level="1"
            :multiple="false"
            :options="treeOptions"
          />
        </el-form-item>
        <el-form-item
          :label="$t('ui.Sys.Sys_Organization.OrganizationCode')"
          prop="OrganizationCode"
        >
          <el-input v-model="formData.OrganizationCode" autocomplete="off"/>
        </el-form-item>
        <!-- <el-form-item :label="$t('ui.Sys.Sys_Organization.OrganizationLevel')">
          <el-input v-model="formData.OrganizationLevel" autocomplete="off" />
        </el-form-item>-->
        <el-form-item
          :label="$t('ui.Sys.Sys_Organization.OrganizationDesc')"
          prop="OrganizationDesc"
        >
          <el-input v-model="formData.OrganizationDesc" autocomplete="off"/>
        </el-form-item>
        <el-form-item :label="$t('Common.Remark')">
          <el-input
            v-model="formData.Remark"
            :autosize="{ minRows: 3, maxRows: 5 }"
            type="textarea"
          />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">
          {{
          $t('Common.cancel')
          }}
        </el-button>
        <el-button
          type="primary"
          @click="formMode === 'Create' ? createData() : updateData()"
        >{{ $t('Common.confirm') }}
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import {
  fetchList,
  add,
  update,
  batchDelete
} from '@/api/Sys/Sys_Organization'

import { listToTree, convertToSelectTree } from '@/utils/index'
import waves from '@/directive/waves' // waves directive 特效
// import Pagination from "@/components/Pagination"; // 分页
// import { MessageBox } from 'element-ui' // 提示框
import Treeselect from '@riophae/vue-treeselect'
import '@riophae/vue-treeselect/dist/vue-treeselect.css'
import elDragDialog from '@/directive/el-drag-dialog' // base on element-ui
import permission from '@/directive/permission/index.js' // 权限判断指令
import { formatDateTime } from '../../../utils'

// new Vue.components("SelectTree")

export default {
  name: 'Sys.Sys_Organization',
  components: {
    Treeselect
  },
  directives: { waves, elDragDialog, permission },
  // filters: {
  //   statusFilter (status) {
  //     const statusMap = {
  //       true: '正常',
  //       false: '冻结'
  //     }
  //     return statusMap[status]
  //   }
  // },
  data() {
    return {
      // define options
      treeOptions: null,
      list: [],
      selectedRowsData: [],
      total: 0,
      listLoading: true,
      listQuery: {
        keyword: '', //
        PageNumber: 1,
        PageSize: 10
      },
      formData: {
        // 表单数据
        OrganizationID: '',
        FathOrganizationID: null,
        OrganizationCode: '',
        OrganizationLevel: '',
        OrganizationDesc: '',
        Remark: ''
      },
      formTitle: '', // 弹窗标题
      formMode: '',
      dialogFormVisible: false, // 表单窗口是否显示标志
      rules: {
        // 表单校验逻辑
        OrganizationCode: [
          {
            required: true,
            message: this.$t('Common.ValidatorMessage.MustInput'),
            trigger: 'change'
          }
        ],
        OrganizationDesc: [
          {
            required: true,
            message: this.$t('Common.ValidatorMessage.MustInput'),
            trigger: 'change'
          }
        ]
      },
      downloadLoading: false,

      tableHeight: 330
    }
  },
  computed: {
    canNotUpdate() {
      var selectRows = this.selectedRowsData || []
      return selectRows.length !== 1
    },
    deleteButtonDisable() {
      var selectRows = this.selectedRowsData || []
      return selectRows.length < 1
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
        this.list = listToTree(
          response.Data.items,
          'OrganizationID',
          'FathOrganizationID'
        )

        this.treeOptions = convertToSelectTree(
          response.Data.items,
          'OrganizationID',
          'FathOrganizationID',
          'OrganizationDesc'
        )
        console.log(this.list, this.treeOptions)
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
        OrganizationID: '',
        FathOrganizationID: null,
        OrganizationCode: '',
        OrganizationLevel: '',
        OrganizationDesc: '',
        Remark: ''
      }
    },
    handleSelectChange(selection) {
      console.log('selection', selection)
      this.selectedRowsData = selection
      // selection.forEach(row => {
      //   if (row.children && row.children.length > 0) {
      //     this.toggleRowSelection(row.children);
      //   }
      // });
    },
    handelSelectRow(row, column, event) {
      // console.log("row", row);
      // console.log("column", column);
      // console.log("event", event);
      // if (row.children && row.children.length > 0) {
      //   this.toggleRowSelection(row.children);
      // }
    },
    toggleRowSelection(rows) {
      if (rows) {
        for (let i = 0; i < rows.length; i++) {
          let row = rows[i]
          this.$refs.multipleTable.toggleRowSelection(row)
        }
      }
    },
    handleCreate() {
      this.resetFormData()
      this.formTitle = this.$t('Common.add')
      this.formMode = 'Create'
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate() // 清除校验
      })
    },
    createData() {
      // this.$notify({
      //     title: "成功",
      //     message: "操作成功"+this.treeSelectedValue,
      //     type: "success",
      //     duration: 2000
      //   });
      // console.log('formData',this.formData)

      // 添加
      add(this.formData).then(response => {
        // this.list.unshift(this.temp)
        this.dialogFormVisible = false
        this.$notify({
          title: this.$t('Common.success'),
          message: this.$t('Common.operationSuccess'),
          type: 'success',
          duration: 2000
        })
        this.getList()
      })
    },
    handleUpdate() {
      this.resetFormData()
      this.formData = Object.assign(this.formData, this.selectedRowsData[0]); // 对象拷贝
      (this.formTitle = this.$t('Common.edit')), (this.formMode = 'Update')
      this.dialogFormVisible = true
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate() // 清除校验
      })
    },
    updateData() {
      update(this.formData).then(() => {
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
    handleDelete() {
      this.$confirm(
        this.$i18n.t('Common.batchDeletingConfirm'),
        this.$t('Common.tip'),
        {
          confirmButtonText: this.$t('Common.confirm'),
          cancelButtonText: this.$t('Common.cancel'),
          type: 'warning'
        }
      ).then(() => {
        var arrRowsID = this.selectedRowsData.map(v => v.OrganizationID)
        console.log(arrRowsID)
        // 删除逻辑处理
        batchDelete(arrRowsID).then(() => {
          this.$notify({
            title: this.$t('Common.success'),
            message: this.$t('Common.deleteSuccess'),
            type: 'success',
            duration: 2000
          })
          this.getList()
        })
      })
    }
  }
}
</script>
