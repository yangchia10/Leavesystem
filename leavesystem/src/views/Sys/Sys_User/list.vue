/* eslint-disable vue/name-property-casing */
<template>
  <div class="app-container">
    <!-- 检索以及工具条区域 -->
    <div class="filter-container">
      <el-input
        v-model="listQuery.keyword"
        :placeholder="$t('ui.Sys.Sys_User.Placeholder.Keyword')"
        style="width: 220px;"
        class="filter-item"
        @keyup.enter.native="handleFilter"
      />

      <el-button v-waves class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter" />
      <hr>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_User.Add' }"
        class="filter-item"
        type="success"
        icon="el-icon-plus"
        @click="handleCreate"
      >{{ $t('Common.add') }}
      </el-button>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_User.Edit' }"
        class="filter-item"
        type="primary"
        icon="el-icon-edit"
        :disabled="canNotUpdate"
        @click="handleUpdate"
      >{{ $t('Common.edit') }}
      </el-button>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_User.Delete' }"
        class="filter-item"
        type="danger"
        icon="el-icon-delete"
        :disabled="canNotDelete"
        @click="handleDelete"
      >{{ $t('Common.delete') }}
      </el-button>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_User.SetRole' }"
        class="filter-item"
        type="primary"
        icon="el-icon-user"
        :disabled="canNotUpdate"
        @click="handleSetUserRole"
      >{{
        $t('ui.Sys.Sys_User.Buttons.SetUserRole') }}
      </el-button>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_User.ResetPassword' }"
        class="filter-item"
        type="danger"
        icon="el-icon-setting"
        :disabled="canNotDelete"
        @click="handleResetPassword"
      >{{
        $t('ui.Sys.Sys_User.Buttons.ResetPassword') }}
      </el-button>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_User.Export' }"
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
      highlight-current-row
      style="width: 100%;"
      height="450"
      :row-style="{ height: '30px' }"
      :cell-style="{ padding: '0' }"
      @selection-change="handleSelectChange"
    >
      <el-table-column type="selection" align="center" width="40" fixed />

      <el-table-column
        width="120px"
        align="center"
        prop="UserName"
        :label="$t('ui.Sys.Sys_User.UserName')"
        show-overflow-tooltip
      />

      <el-table-column width="120px" align="center" prop="FrgnName" :label="$t('ui.Sys.Sys_User.FrgnName')">
        <template slot-scope="scope">
          <span>{{ scope.row.FrgnName }}</span>
        </template>
      </el-table-column>

      <el-table-column
        width="120px"
        align="center"
        prop="OrganizationID"
        :label="$t('ui.Sys.Sys_User.OrganizationID')"
        :formatter="formatOrganization"
      />

      <el-table-column width="120px" align="center" prop="LoginAccount" :label="$t('ui.Sys.Sys_User.LoginAccount')">
        <template slot-scope="scope">
          <span>{{ scope.row.LoginAccount }}</span>
        </template>
      </el-table-column>

      <el-table-column
        width="120px"
        align="center"
        prop="Birthday"
        :label="$t('ui.Sys.Sys_User.Birthday')"
        :formatter="formatDate"
      />

      <el-table-column width="260px" align="center" prop="Email" :label="$t('ui.Sys.Sys_User.Email')" />

      <el-table-column width="120px" align="center" prop="Mobile" :label="$t('ui.Sys.Sys_User.Mobile')" />

      <el-table-column width="120px" align="center" prop="Telphone" :label="$t('ui.Sys.Sys_User.Telphone')" />

      <el-table-column
        width="60px"
        align="center"
        prop="Gender"
        :label="$t('ui.Sys.Sys_User.Gender')"
        :formatter="formatGender"
      />

      <el-table-column
        v-if="false"
        width="100px"
        align="center"
        prop="Gender"
        :label="$t('ui.Sys.Sys_User.IsSupplier')"
        fixed="right"
      >
        <template slot-scope="{ row }">{{ row.IsSupplier | yesnoFilter }}</template>
      </el-table-column>

      <el-table-column width="80px" align="center" :label="$t('ui.Sys.Sys_User.IsEnable')" fixed="right">
        <template slot-scope="{ row }">
          <el-tag v-if="row.IsEnable" type="success">
            {{
              row.IsEnable | statusFilter
            }}
          </el-tag>
          <el-tag v-else type="danger">
            {{
              row.IsEnable | statusFilter
            }}
          </el-tag>
        </template>
      </el-table-column>

      <el-table-column width="120px" align="center" prop="Remark" :label="$t('Common.Remark')" />

      <el-table-column width="120px" align="center" prop="CUser" :label="$t('Common.CUser')">
        <template slot-scope="scope">
          <span>{{ scope.row.CUser }}</span>
        </template>
      </el-table-column>

      <el-table-column
        width="180px"
        align="center"
        prop="CTime"
        :label="$t('Common.CTime')"
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

    <!-- 表单对话框 -->
    <el-dialog v-el-drag-dialog :title="formTitle" :visible.sync="dialogFormVisible" top="10vh" width="65%">
      <el-form ref="dataForm" :rules="rules" :model="formData" label-position="left" label-width="100px">
        <el-row :gutter="15">
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.UserName')" prop="UserName">
              <el-input v-model="formData.UserName" autocomplete="off" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.FrgnName')">
              <el-input v-model="formData.FrgnName" autocomplete="off" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="15">
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.LoginAccount')" prop="LoginAccount">
              <el-input v-model="formData.LoginAccount" autocomplete="off" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.Gender')">
              <el-select v-model="formData.Gender" class="filter-item">
                <!-- <el-option v-for="item in genderOptions" :key="item.value" :label="item.label" :value="item.value" /> -->
                <el-option v-for="item in genderOptions" :key="item.value" :label="item.label" :value="item.value" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="15">
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.OrganizationID')">
              <treeselect
                v-model="formData.OrganizationID"
                :multiple="false"
                :default-expand-level="1"
                :options="treeOptions"
                :placeholder="
                  $t('Common.select') + $t('ui.Sys.Sys_User.OrganizationID')
                "
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.Birthday')">
              <el-date-picker v-model="formData.Birthday" type="date" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="15">
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.Email')" prop="Email">
              <el-input v-model="formData.Email" autocomplete="off" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.Mobile')" prop="Mobile">
              <el-input v-model="formData.Mobile" autocomplete="off" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="15">
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.Telphone')">
              <el-input v-model="formData.Telphone" autocomplete="off" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.IsEnable')">
              <el-switch v-model="formData.IsEnable" active-color="#13ce66" inactive-color="#ff4949" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row v-if="false" :gutter="15">
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.IsSupplier')" prop="IsSupplier">
              <el-switch v-model="formData.IsSupplier" active-color="#13ce66" inactive-color="#ff4949" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item :label="$t('ui.Sys.Sys_User.UserRole')">
              <el-select v-model="formData.UserRole" class="filter-item">
                <!-- <el-option v-for="item in genderOptions" :key="item.value" :label="item.label" :value="item.value" /> -->
                <el-option v-for="item in userRoleOptions" :key="item.value" :label="item.label" :value="item.value" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item :label="$t('Common.Remark')">
          <el-input v-model="formData.Remark" :autosize="{ minRows: 3, maxRows: 5 }" type="textarea" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">
          {{
            $t('Common.cancel')
          }}
        </el-button>
        <el-button type="primary" @click="formMode === 'Create' ? createData() : updateData()">{{ $t('Common.confirm')
        }}
        </el-button>
      </div>
    </el-dialog>

    <!--角色设置-->
    <el-dialog v-el-drag-dialog :title="formSetRoleTitle" :visible.sync="dialogFormRoleVisible" top="10vh">
      <!-- <div class="filter-container">
      <el-input v-model="sublistQuery.keyword" class="filter-item" style="width: 200px" :placeholder="$t('Common.keyword')" @keyup.enter.native="handleFilterRole" />
      <el-button v-waves class="filter-item" type="primary" icon="el-icon-search" @click="handleFilterRole"></el-button>
    </div>-->
      <el-table
        ref="multipleRoleTable"
        :data="roleList"
        border
        highlight-current-row
        :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
        style="width: 100%;height: 45vh;overflow: auto;"
        @selection-change="handleRoleSelectionChange"
      >
        <el-table-column :label="$t('Common.select')" type="selection" align="center" width="40" />
        <el-table-column v-if="false" :label="$t('ui.Sys.Sys_Role.RoleID')" align="center" width="150">
          <template slot-scope="scope">
            <span>{{ scope.row.RoleID }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('ui.Sys.Sys_Role.RoleDesc')" align="center" width="150">
          <template slot-scope="scope">
            <span>{{ scope.row.RoleDesc }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="$t('Common.Remark')" align="center" width="300">
          <template slot-scope="scope">
            <span>{{ scope.row.Remark }}</span>
          </template>
        </el-table-column>
      </el-table>
      <span slot="footer" class="dialog-footer">
        <el-button
          v-waves
          type="primary"
          icon="el-icon-check"
          @click="setUserRole"
        >{{ $t('Common.select') }}</el-button>
        <el-button v-waves @click="dialogFormRoleVisible = false">
          {{
            $t('Common.close')
          }}
        </el-button>
      </span>
    </el-dialog>
    <!-- 重置密码 -->
    <el-dialog
      v-el-drag-dialog
      :title="$t('ui.Sys.Sys_User.Buttons.ResetPassword')"
      :visible.sync="dialogFormResetVisible"
      top="10vh"
      width="30%"
      @close="closeResetPassword"
    >
      <el-form ref="dataForm" :model="PasswordInfo" :rules="rules">
        <el-form-item :label="$t('ui.Sys.Sys_User.ResetPasswordDlg.LoginAccount')">
          <el-input v-model="PasswordInfo.LoginAccount" :disabled="true" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_User.ResetPasswordDlg.NewPassword')" prop="NewPassword">
          <el-input v-model="PasswordInfo.NewPassword" show-password />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_User.ResetPasswordDlg.ConfirmNewPassword')" prop="ConfirmNewPassword">
          <el-input v-model="PasswordInfo.ConfirmNewPassword" show-password />
        </el-form-item>

        <el-form-item>
          <el-button type="primary" @click="saveResetPassword">{{ $t('Common.confirm') }}</el-button>
        </el-form-item>
      </el-form>
    </el-dialog>
  </div>
</template>

<script>
// 获取用户基本信息--增删改查

import Treeselect from '@riophae/vue-treeselect'
import '@riophae/vue-treeselect/dist/vue-treeselect.css'
import elDragDialog from '@/directive/el-drag-dialog' // base on element-ui

import {
  fetchList,
  add,
  update,
  batchDelete,
  resetPassword,
  exportExcelFile
} from '@/api/Sys/Sys_User'
import {
  fetchList as fetchOrganizationList
} from '@/api/Sys/Sys_Organization'
import {
  fetchList as fetchRoleList,
  getUserRoles
} from '@/api/Sys/Sys_Role'
import {
  resetUserRoles
} from '@/api/Sys/Sys_UserRole'
import {
  exportToExcel
} from '@/utils/excel-export'

// 当然你也可以为了方便使用，将它注册到全局
import permission from '@/directive/permission/index.js' // 权限判断指令

import {
  convertToKeyValue,
  listToTree,
  convertToSelectTree
} from '@/utils'
import waves from '@/directive/waves' // waves directive 特效
import Pagination from '@/components/Pagination' // 分页
import {
  MessageBox
} from 'element-ui' // 提示框

import {
  validateEMail,
  validatePhone
} from '@/utils/form-validator' // 表单自定义校验
import {
  formatDate,
  formatDateTime
} from '@/utils' // 列表内容格式化

import md5 from 'js-md5'

const statusMapArr = [{
  value: true,
  label: i18n.t('Dictionary.AccountIsEnableMap.TrueValue')
}, // 正常
{
  value: false,
  label: i18n.t('Dictionary.AccountIsEnableMap.FalseValue')
} // 冻结
]

export default {
  // eslint-disable-next-line vue/name-property-casing
  name: 'Sys.Sys_User',
  components: {
    Pagination,
    Treeselect
  },
  directives: {
    waves,
    elDragDialog,
    permission
  },
  filters: {
    statusFilter(status) {
      const statusMap = convertToKeyValue(statusMapArr)
      return statusMap[status]
    }
  },
  data() {
    return {
      handleDisable: false,
      // define the default value
      treeSelectedValue: null,
      // define options
      treeOptions: [],
      organizationList: [],
      list: null,
      total: 0,
      listLoading: true,
      listQuery: {
        keyword: '', // 用户姓名 | 登录账号
        PageNumber: 1,
        PageSize: 10
      },
      roleList: [],
      selectedRowsData: [],
      selectedUserRolesData: [],
      genderOptions: [{
        value: 1,
        label: this.$t('Dictionary.GenderMap.One')
      }, // 男
      {
        value: 2,
        label: this.$t('Dictionary.GenderMap.Two')
      } // 女
      ],
      userRoleOptions: [{
        value: 1,
        label: this.$t('Dictionary.UserRoleMap.One')
      }, // 系统管理员
      {
        value: 2,
        label: this.$t('Dictionary.UserRoleMap.Two')
      } // 业务人员
      ],
      formData: {
        // 表单数据
        UserID: null,
        OrganizationID: null,
        OrganizationDesc: '',
        UserName: '',
        FrgnName: '',
        LoginAccount: '',
        LoginPassword: '',
        Gender: '',
        Birthday: new Date(),
        Email: '',
        Telphone: '',
        Mobile: '',
        UserRole: 2, // 默认业务用户
        IsSupplier: false,
        IsEnable: true,
        Remark: '',
        CUser: '',
        CDate: new Date(),
        MUser: '',
        MDate: new Date()
      },
      PasswordInfo: {
        LoginAccount: '',
        OldPassword: '',
        NewPassword: '',
        ConfirmNewPassword: ''
      },
      formTitle: '', // 弹窗标题
      formSetRoleTitle: this.$t('ui.Sys.Sys_User.Titles.SetUserRole'), // "设置用户角色", // 设置角色
      formMode: '',
      dialogFormResetVisible: false,
      dialogFormVisible: false, // 表单窗口是否显示标志
      dialogFormRoleVisible: false, // 设置角色窗口是否显示标志
      rules: {
        // 表单校验逻辑
        UserName: [{
          required: true,
          message: this.$t('Common.ValidatorMessage.MustInput'),
          trigger: 'blur'
        }],
        LoginAccount: [{
          required: true,
          message: this.$t('Common.ValidatorMessage.MustInput'),
          trigger: 'blur'
        },
        {
          required: true,
          min: 3,
          max: 15,
          message: this.$t('Common.ValidatorMessage.LengthRange', {
            Range: '[3-15]'
          }),
          trigger: 'blur'
        }
        ],
        Mobile: [{
          validator: validatePhone,
          message: this.$t('ui.Sys.Sys_User.FormValidator.ValidateMobile'),
          trigger: 'blur'
        }],
        Email: [{
          validator: validateEMail,
          message: this.$t('ui.Sys.Sys_User.FormValidator.ValidateEmail'),
          trigger: 'blur'
        }],
        // OldPassword: [
        //   {
        //     required: true,
        //     message: i18n.t("Common.ValidatorMessage.MustInput"),
        //     trigger: "blur"
        //   }
        // ],
        NewPassword: [{
          required: true,
          min: 3,
          max: 15,
          message: this.$t('Common.ValidatorMessage.LengthRange', {
            Range: '[3-15]'
          }),
          trigger: 'blur'
        }],
        ConfirmNewPassword: [{
          required: true,
          min: 3,
          max: 15,
          message: this.$t('Common.ValidatorMessage.LengthRange', {
            Range: '[3-15]'
          }),
          trigger: 'blur'
        }]
      },
      downloadLoading: false
    }
  },
  computed: {
    canNotUpdate() {
      return this.selectedRowsData.length !== 1
    },
    canNotDelete() {
      return this.selectedRowsData.length !== 1
    }
  },
  created() {
    this.getList()
    this.getOrganizationList() // 组织机构Formatter
  },
  methods: {
    formatDate,
    formatDateTime,
    formatGender(row, column, currentValue) {
      const opt = this.genderOptions.find(element => {
        return element.value === currentValue
      })
      return opt === undefined ? '' : opt.label
    },
    deltailSortChange(data) {
      const {
        prop,
        order
      } = data
      if (prop && order) {
        if (order === 'ascending') {
          this.listQuery.sort = prop + ' asc'
        } else {
          this.listQuery.sort = prop + ' desc'
        }
      } else {
        this.listQuery.sort = ''
      }
      this.getList()
    },
    // getGenderMapObject,
    // getIsEnableMapObject,
    formatOrganization: function(row, column, currentValue) {
      const findedOrgation = this.organizationList.find(element => {
        return element.OrganizationID === currentValue
      })
      return findedOrgation != null ? findedOrgation.OrganizationDesc : ''
    },
    getList() {
      // 获取数据
      this.listLoading = true
      fetchList(this.listQuery).then(response => {
        // console.log(response);
        this.list = response.Data.items
        this.total = response.Data.total
        this.listLoading = false
      })
    },
    getOrganizationList() {
      // 获取数据
      this.listLoading = true
      fetchOrganizationList({ keyword: '' }).then(response => {
        // console.log(response);
        this.organizationList = response.Data
        this.listLoading = false
      })
    },
    initOrganizationTree() {
      fetchOrganizationList({ keyword: '' }).then(response => {
        this.treeOptions = convertToSelectTree(
          response.Data,
          'OrganizationID',
          'FathOrganizationID',
          'OrganizationDesc'
        )
      })
    },
    getUserRoleList() {
      this.listLoading = true
      getUserRoles({
        userid: this.selectedRowsData[0].UserID
      }).then(res => {
        // console.log('Sys_User.list-getUserRoleList', res)
        this.roleList = res.Data.RoleList
        if (res.Data.UserRoleList != null) {
          // 设置用户角色的默认选中项
          this.$refs.multipleRoleTable.clearSelection() // 清空当前选择
          this.$nextTick(function() {
            const userRoleList = res.Data.UserRoleList
            if (userRoleList != null && userRoleList.length > 0) {
              userRoleList.forEach(element => {
                const item = this.roleList.findIndex(function(x) {
                  return x.RoleID === element.RoleID
                })
                console.log('userroleitem', item, this.roleList[item])

                if (item != null) {
                  console.log('ttsdfa')
                  this.$refs.multipleRoleTable.toggleRowSelection(
                    this.roleList[item],
                    true
                  )
                }
              })
            }
          })
        }
        this.listLoading = false
      })
    },
    handleFilter() {
      // 检索处理
      this.getList()
    },
    resetFormData() {
      // 重置表单数据
      this.formData = {
        UserID: '',
        OrganizationID: null,
        OrganizationDesc: '',
        UserName: '',
        FrgnName: '',
        LoginAccount: '',
        LoginPassword: '',
        Gender: '',
        Birthday: new Date(),
        Email: '',
        Telphone: '',
        Mobile: '',
        UserRole: 2, // 默认业务用户
        IsEnable: true,
        Remark: '',
        CUser: '',
        CDate: new Date(),
        MUser: '',
        MDate: new Date()
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
      this.initOrganizationTree()
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate() // 清除校验
      })
    },
    createData() {
      // 添加
      this.$refs['dataForm'].validate(valid => {
        if (valid) {
          // 确认添加

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
      this.initOrganizationTree()
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate() // 清除校验
      })
    },
    updateData() {
      this.$refs['dataForm'].validate(valid => {
        if (valid) {
          update(this.formData).then(res => {
            if (res.Code === 2000) {
              this.showNotify('success', 'Common.operationSuccess')
            } else {
              this.showNotify('error', res.Message)
            }
            this.getList()
            this.dialogFormVisible = false
          })
        }
      })
    },
    closeResetPassword() {
      this.PasswordInfo = {
        LoginAccount: '',
        OldPassword: '',
        NewPassword: '',
        ConfirmNewPassword: ''
      }
    },
    saveResetPassword() {
      this.$refs['dataForm'].validate(valid => {
        if (valid) {
          if (
            this.PasswordInfo.NewPassword !==
            this.PasswordInfo.ConfirmNewPassword
          ) {
            this.$message({
              title: this.$t('Common.Warning'),
              message: this.$t(
                'ui.Sys.Sys_User.ResetPasswordDlg.NotConsistent'
              ),
              type: 'warning',
              duration: 5 * 1000
            })
            return
          }
          var arrRowsID = this.selectedRowsData.map(function(v) {
            return v.UserID
          })
          var querylist = {
            ids: arrRowsID,
            newPassword: md5(this.PasswordInfo.NewPassword)
          }
          // 重置密码
          console.log(querylist)
          resetPassword(querylist).then(res => {
            console.log('handleResetPassword', res)
            this.$notify({
              title: this.$t('Common.success'),
              message: this.$t('Common.operationSuccess'),
              type: 'success',
              duration: 2000
            })
            this.dialogFormResetVisible = false
            this.getList()
          })
        }
      })
    },
    handleResetPassword() {
      this.dialogFormResetVisible = true
      this.PasswordInfo.LoginAccount = this.selectedRowsData[0].LoginAccount
      // // 重置密码
      // this.$confirm(
      //   this.$t("ui.Sys.Sys_User.TipMessage.ResetPasswordConfirm"),
      //   this.$t("Common.tip"),
      //   {
      //     confirmButtonText: this.$t("Common.confirm"),
      //     cancelButtonText: this.$t("Common.cancel"),
      //     type: "warning"
      //   }
      // ).then(() => {
      //   var arrRowsID = this.selectedRowsData.map(function(v) {
      //     return v.UserID;
      //   });
      //   // 重置密码
      //   resetPassword(arrRowsID).then(res => {
      //     console.log("handleResetPassword", res);
      //     this.$notify({
      //       title: this.$t("Common.success"),
      //       message: this.$t("Common.operationSuccess"),
      //       type: "success",
      //       duration: 2000
      //     });
      //     this.getList();
      //   });
      // });
    },
    handleSetUserRole() {
      this.dialogFormRoleVisible = true
      this.getUserRoleList()
    },
    setUserRole() {
      // var userRoles = this.selectedRowsData.map(function(v){return v.RoleID})
      if (this.selectedRowsData.length == 1) {
        // console.log('setUserRole',this.selectedRowsData,this.selectedUserRolesData)

        var UserID = this.selectedRowsData[0].UserID // 用户只能单选
        var RoleIDS = this.selectedUserRolesData
          .map(function(v) {
            return v.RoleID
          })
          .join('|')
        resetUserRoles({
          userid: UserID,
          roleids: RoleIDS
        }).then(() => {
          this.dialogFormRoleVisible = false
          this.$notify({
            title: this.$t('Common.success'),
            message: this.$t('Common.operationSuccess'),
            type: 'success',
            duration: 2000
          })
        })
      }
    },
    handleRoleSelectionChange(selection) {
      this.selectedUserRolesData = selection
    },
    handleDelete() {
      this.$confirm(
        this.$i18n.t('Common.batchDeletingConfirm'),
        this.$i18n.t('Common.tip'), {
          confirmButtonText: this.$i18n.t('Common.affirm'),
          cancelButtonText: this.$i18n.t('Common.cancel'),
          type: 'warning'
        }
      ).then(() => {
        var arrRowsID = this.selectedRowsData.map(v => v.UserID)

        // 删除逻辑处理
        batchDelete(arrRowsID).then(res => {
          if (res.Code === 2000) {
            this.showNotify('success', 'Common.deleteSuccess')
          } else {
            this.showNotify('error', res.Message)
          }
          this.getList()
        })
      })
    },
    handleExport() {
      exportExcelFile({
        Keyword: this.listQuery.keyword
      }).then(res =>
        exportToExcel(res.data, res.fileName)
      )
    }
  }
}
</script>

<style></style>
