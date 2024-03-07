<template>
  <div class="app-container">
    <!-- 检索以及工具条区域 -->
    <div class="filter-container">
      <el-input
        v-model="listQuery.keyword"
        placeholder="角色描述"
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

      <hr>
      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_Role.Add' }"
        class="filter-item"
        type="success"
        icon="el-icon-plus"
        @click="handleCreate"
      >{{ $t('Common.add') }}
      </el-button>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_Role.Edit' }"
        class="filter-item"
        type="primary"
        :disabled="canNotUpdate"
        icon="el-icon-edit"
        @click="handleUpdate"
      >{{ $t('Common.edit') }}
      </el-button>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_Role.Delete' }"
        class="filter-item"
        type="danger"
        :disabled="canNotDelete"
        icon="el-icon-delete"
        @click="handleDelete"
      >{{ $t('Common.delete') }}
      </el-button>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_Role.Permission' }"
        class="filter-item"
        type="primary"
        :disabled="canNotUpdate"
        icon="el-icon-setting"
        @click="handlePermissionSetting"
      >{{ $t('route.Sys.Sys_Permission.Permission') }}
      </el-button>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_Role.Export' }"
        :loading="downloadLoading"
        class="filter-item"
        type="primary"
        icon="el-icon-download"
        @click="handleExport"
      >{{ $t('Common.export') }}
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
      <el-table-column type="selection" width="40" fixed />
      <el-table-column
        label="#"
        type="index"
        align="center"
        width="40"
        fixed
      />
      <el-table-column v-if="false" align="center" :label="$t('ui.Sys.Sys_Role.RoleID')">
        <template slot-scope="scope">
          <span>{{ scope.row.RoleID }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_Role.RoleDesc')">
        <template slot-scope="scope">
          <span>{{ scope.row.RoleDesc }}</span>
        </template>
      </el-table-column>
      <el-table-column width="250px" align="center" :label="$t('Common.Remark')">
        <template slot-scope="scope">
          <span>{{ scope.row.Remark }}</span>
        </template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('Common.CUser')">
        <template slot-scope="scope">
          <span>{{ scope.row.CUser }}</span>
        </template>
      </el-table-column>
      <el-table-column width="160px" align="center" :label="$t('Common.CTime')">
        <template slot-scope="scope">
          <span>{{ scope.row.CTime|datetime }}</span>
        </template>
      </el-table-column>
    </el-table>
    <!-- 分页组件 -->
    <pagination
      v-show="total > 0"
      :total="total"
      :page.sync="listQuery.PageNumber"
      :limit.sync="listQuery.PageSize"
      @pagination="getList"
    />

    <!-- 表单对话框 -->
    <el-dialog
      v-el-drag-dialog
      :title="formTitle"
      :visible.sync="dialogFormVisible"
      top="10vh"
      width="50%"
    >
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="formData"
        label-position="left"
        label-width="100px"
      >
        <el-form-item :label="$t('ui.Sys.Sys_Role.RoleDesc')" prop="RoleDesc">
          <el-input v-model="formData.RoleDesc" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('Common.Remark')" prop="Remark">
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

    <!-- 权限设置对话框 -->
    <el-dialog
      v-el-drag-dialog
      :title="formPermissionTitle"
      :visible.sync="dialogPermissionSettingVisible"
      top="10vh"
    >
      <!-- <el-dropdown>
        <el-button type="primary">
          选择客户端APP
          <i class="el-icon-arrow-down el-icon--right"></i>
        </el-button>
        <el-dropdown-menu slot="dropdown">
          <el-dropdown-item @click.native="handleInitAppMenus('P0001')">后台管理系统</el-dropdown-item>
          <el-dropdown-item @click.native="handleInitAppMenus('M001')">PDA手持客户端</el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>

      <br />

      <el-tree
        ref="permissionTree"
        :data="permissionData"
        show-checkbox
        node-key="id"
        :default-checked-keys="roleHadPermissions"
        :props="defaultProps"
      ></el-tree>-->
      <!--  @tab-click="handleInitAppMenus" 页面初始化一次性处理，防止重复加载请求后台 -->
      <el-tabs v-model="defaultActiveName">
        <el-tab-pane :label="$t('ui.Sys.Sys_Role.PanelTab.Admin')" name="P0001">
          <el-tree
            ref="adminPermissionTree"
            :data="adminPermissionData"
            show-checkbox
            node-key="id"
            :default-checked-keys="roleHadAdminPermissions"
            :props="defaultProps"
          />
        </el-tab-pane>
        <el-tab-pane :label="$t('ui.Sys.Sys_Role.PanelTab.PDA')" name="M001">
          <el-tree
            ref="pdaPermissionTree"
            :data="pdaPermissionData"
            show-checkbox
            node-key="id"
            :default-checked-keys="roleHadPdaPermissions"
            :props="defaultProps"
          />
        </el-tab-pane>
      </el-tabs>

      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogPermissionSettingVisible = false">
          {{
            $t('Common.cancel')
          }}
        </el-button>
        <el-button type="primary" @click="setRolePermission()">
          {{
            $t('Common.confirm')
          }}
        </el-button>
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
} from '@/api/Sys/Sys_Role'
import { exportToExcel } from '@/utils/excel-export'
import {
  GetResourcesByAppID,
  GetResourceByAppRole
} from '@/api/Sys/Sys_Resource'
import { ResetRoleResources } from '@/api/Sys/Sys_RoleResource'

import { validateIP } from '@/utils/form-validator'

import { convertToKeyValue, convertToSelectTree } from '@/utils'
import waves from '@/directive/waves' // waves directive 特效
import Pagination from '@/components/Pagination' // 分页
import { MessageBox } from 'element-ui' // 提示框

// import Treeselect from '@riophae/vue-treeselect'
// import '@riophae/vue-treeselect/dist/vue-treeselect.css'
import permission from '../../../directive/permission/permission'
import { formatDateTime } from '../../../utils'

export default {
  // eslint-disable-next-line vue/name-property-casing
  name: 'Sys.Sys_Role',
  components: { Pagination },
  directives: { waves, elDragDialog, permission },
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
      defaultActiveName: 'P0001',
      roleHadAdminPermissions: [], // 角色当前拥有的权限(后台管理)
      roleHadPdaPermissions: [], // 角色当前拥有的权限(PDA)
      adminPermissionData: [],
      pdaPermissionData: [],

      defaultProps: {
        children: 'children',
        label: 'label'
      },
      list: null,
      total: 0,
      listLoading: true,
      listQuery: {
        keyword: '', // 角色描述
        PageNumber: 1,
        PageSize: 10
      },
      formData: {
        // 表单数据
        RoleID: '',
        RoleDesc: '',
        Remark: ''
      },
      formTitle: '', // 弹窗标题
      formPermissionTitle: this.$t(
        'ui.Sys.Sys_Role.ResourceElement.PermissionSettingDialogTitle'
      ), //
      formMode: '',
      dialogFormVisible: false, // 表单窗口是否显示标志
      dialogPermissionSettingVisible: false, // 权限设置窗口
      downloadLoading: false,
      selectedRowsData: [],
      rules: {
        // 表单校验逻辑
        RoleDesc: [
          {
            required: true,
            message: this.$t('Common.ValidatorMessage.MustInput'),
            trigger: 'change'
          }
        ]
      },
      tableHeight: 330
    }
  },
  computed: {
    canNotUpdate() {
      var selectRows = this.selectedRowsData || []
      return selectRows.length !== 1
    },
    canNotDelete() {
      return this.selectedRowsData.length < 1
    }
  },
  created() {
    this.getList()
  },
  mounted() {
    // this.$router.push('https://www.google.com.tw/?hl=zh_TW')
    // location.replace('https://www.google.com.tw/?hl=zh_TW')
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
      })
    },
    handleFilter() {
      this.getList()
    },
    resetFormData() {
      // 重置表单数据
      this.formData = {
        RoleID: '',
        RoleDesc: '',
        Remark: ''
      }
    },
    handleSelectChange(selection) {
      this.selectedRowsData = selection
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
      this.$refs['dataForm'].validate(valid => {
        if (valid) {
          // 添加
          add(this.formData).then(response => {
            this.dialogFormVisible = false
            this.$notify({
              title: this.$t('Common.success'),
              message: this.$t('Common.operationSuccess'),
              type: 'success',
              duration: 2000
            })
            this.getList()
          })
        }
      })
    },
    handleUpdate() {
      this.resetFormData()
      this.formData = Object.assign({}, this.selectedRowsData[0]) // 对象拷贝
      this.formTitle = this.$t('Common.edit')
      this.formMode = 'Update'
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
        var arrRowsID = this.selectedRowsData.map(function(v) {
          return v.RoleID
        })

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
    },
    handlePermissionSetting() {
      // 权限设置窗口
      // this.resetFormData()
      // this.formData = Object.assign({}, this.selectedRowsData[0]); // 对象拷贝
      // (this.formTitle = this.$t("Common.Update")),
      //   (this.dialogPermissionSettingVisible = true);
      this.formTitle = this.$t('ui.Sys.Sys_Role.Buttons.SetPermission')
      this.dialogPermissionSettingVisible = true
      this.defaultActiveName = 'P0001'

      // 后台管理部分权限
      this.adminPermissionData = []
      this.roleHadAdminPermissions = []
      this.pdaPermissionData = []
      this.roleHadPdaPermissions = []

      this.initAdminAppMenus()
      this.initPdaAppMenus()
      // PDA客户端部分权限
      // this.$nextTick(() => {
      //   this.$refs['dataForm'].clearValidate() // 清除校验
      // })
    },
    initAdminAppMenus() {
      const adminAppId = 'P0001'
      GetResourcesByAppID({ appid: adminAppId, roleSet: 'roleSet' }).then(
        res => {
          GetResourceByAppRole({
            appid: adminAppId,
            roleid: this.selectedRowsData[0].RoleID
          }).then(resData => {
            const resDataMap = res.Data.map(item => {
              item.ResourceTitle = this.$t('route.' + item.ResourceTitle) // 翻译菜单名称
              return item
            })
            this.adminPermissionData = convertToSelectTree(
              resDataMap,
              'ResourceID',
              'FathResourceID',
              'ResourceTitle'
            )

            this.roleHadAdminPermissions = [] // 重置
            resData.Data.forEach((value, index, arr) => {
              // console.log('resData.Data', index, value)
              // 如果存在以当前元素为父元素的节点，则
              const firstChildrenNode = resData.Data.find(currentValue => {
                return currentValue.FathResourceID == value.ResourceID
              })
              // console.log('isLiefNode', firstChildrenNode)
              if (firstChildrenNode == undefined) {
                // console.log('dfadfa',value.FathResourceID)
                this.roleHadAdminPermissions.push(value.ResourceID)
              } // 只要末枝节点
            })

            // console.log(
            //   'this.roleHadAdminPermissions',
            //   this.roleHadAdminPermissions
            // )
          })
        }
      )
    },
    initPdaAppMenus() {
      const pdaAppID = 'M001'
      GetResourcesByAppID({ appid: pdaAppID, roleSet: 'roleSet' }).then(res => {
        GetResourceByAppRole({
          appid: pdaAppID,
          roleid: this.selectedRowsData[0].RoleID
        }).then(resData => {
          const resDataMap = res.Data.map(item => {
            item.ResourceTitle = this.$t('route.' + item.ResourceTitle) // 翻译菜单名称
            return item
          })
          this.pdaPermissionData = convertToSelectTree(
            resDataMap,
            'ResourceID',
            'FathResourceID',
            'ResourceTitle'
          )

          this.roleHadPdaPermissions = [] // 重置
          resData.Data.forEach((value, index, arr) => {
            const firstChildrenNode = resData.Data.find(currentValue => {
              return currentValue.FathResourceID == value.ResourceID
            })
            if (firstChildrenNode == undefined) {
              this.roleHadPdaPermissions.push(value.ResourceID)
            } // 只要末枝节点
          })
        })
      })
    },
    setRolePermission() {
      // 设置权限
      const selectedAdminNodeKeys =
        this.$refs.adminPermissionTree.getCheckedKeys() || []
      const halfSelectedAdminNodeKeys =
        this.$refs.adminPermissionTree.getHalfCheckedKeys() || []

      const selectedPdaNodeKeys =
        this.$refs.pdaPermissionTree.getCheckedKeys() || []
      const halfSelectedPdaNodeKeys =
        this.$refs.pdaPermissionTree.getHalfCheckedKeys() || []

      const selectedNodeKeys = selectedAdminNodeKeys.concat(
        halfSelectedAdminNodeKeys,
        selectedPdaNodeKeys,
        halfSelectedPdaNodeKeys
      )
      // console.log(selectedNodeKeys)

      ResetRoleResources({
        id: this.selectedRowsData[0].RoleID,
        ids: selectedNodeKeys
      }).then(res => {
        this.dialogPermissionSettingVisible = false
        this.$notify({
          title: this.$t('Common.success'),
          message: this.$t('Common.operationSuccess'),
          type: 'success',
          duration: 2000
        })
      })
    },
    handleExport() {
      exportExcelFile({ Keyword: this.listQuery.keyword }).then(res =>
        exportToExcel(res.data, res.fileName)
      )
    }
  }
}
</script>
