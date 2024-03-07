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
        placeholder="数据库名称 | 备份名称"
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
        v-permission="{name:'Sys.Sys_DbBackup.Backup'}"
        v-loading.fullscreen.lock="fullscreenLoading"
        class="filter-item"
        type="success"
        icon="el-icon-plus"
        @click="handleBackupDB"
      >{{ $t('route.Sys.Sys_Permission.Backup') }}
      </el-button>

      <!--      <el-button-->
      <!--        v-waves-->
      <!--        v-permission="{name:'Sys.Sys_DbBackup.Reduction'}"-->
      <!--        class="filter-item"-->
      <!--        type="primary"-->
      <!--        :disabled="disableRestore"-->
      <!--        icon="el-icon-back"-->
      <!--        @click="handleRestoreDB"-->
      <!--      >{{ $t('route.Sys.Sys_Permission.Reduction') }}</el-button>-->

      <el-button
        v-waves
        v-permission="{name:'Sys.Sys_DbBackup.Delete'}"
        class="filter-item"
        type="danger"
        :disabled="disableDelete"
        icon="el-icon-delete"
        @click="handleDelete"
      >{{ $t('Common.delete') }}
      </el-button>

      <el-button
        v-waves
        v-permission="{name:'Sys.Sys_DbBackup.Set'}"
        class="filter-item"
        type="primary"
        icon="el-icon-setting"
        @click="handleDbBackupSetting"
      >{{ $t('route.Sys.Sys_Permission.Set') }}
      </el-button>

      <!--<el-button
        v-waves
        class="filter-item"
        type="danger"
        icon="el-icon-delete"
        @click="handleDelete"
      >删除</el-button>-->
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
      :height="tableHeight"
      @selection-change="handleSelectChange"
    >
      <el-table-column type="selection" width="40" fixed/>
      <el-table-column
        label="#"
        type="index"
        align="center"
        width="40"
        fixed
      />
      <el-table-column
        v-if="false"
        width="160px"
        align="center"
        prop="DbBackupID"
        :label="$t('ui.Sys.Sys_DbBackup.DbBackupID')"
      />
      <el-table-column
        width="160px"
        align="center"
        prop="DbName"
        :label="$t('ui.Sys.Sys_DbBackup.DbName')"
      />
      <el-table-column
        width="160px"
        align="center"
        prop="DbServer"
        :label="$t('ui.Sys.Sys_DbBackup.DbServer')"
      />
      <el-table-column
        width="260px"
        align="center"
        prop="BackupName"
        :label="$t('ui.Sys.Sys_DbBackup.BackupName')"
      />
      <el-table-column
        width="260px"
        align="center"
        prop="BackupFilePath"
        :label="$t('ui.Sys.Sys_DbBackup.BackupFilePath')"
      />
      <!-- <el-table-column
        width="160px"
        align="center"
        prop="IsCompressed"
        :label="$t('ui.Sys.Sys_DbBackup.IsCompressed')"
      ></el-table-column> -->

      <el-table-column
        v-if="false"
        width="160px"
        align="center"
        prop="Remark"
        :label="$t('Common.Remark')"
      />
      <el-table-column
        width="160px"
        align="center"
        prop="CUser"
        :label="$t('Common.CUser')"
      />
      <el-table-column
        width="200px"
        align="center"
        prop="CTime"
        :label="$t('Common.CTime')"
        :formatter="formatDateTime"
      />
      <!-- <el-table-column
        width="160px"
        align="center"
        prop="MUser"
        :label="$t('ui.Sys.Sys_DbBackup.MUser')"
      ></el-table-column>
      <el-table-column
        width="160px"
        align="center"
        prop="MTime"
        :label="$t('ui.Sys.Sys_DbBackup.MTime')"
      ></el-table-column> -->
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
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="formData"
        label-position="left"
        label-width="120px"
      >
        <el-form-item :label="$t('ui.Sys.Sys_DbBackupConfig.DbName')">
          <el-input v-model="formData.DbName" autocomplete="off" :disabled="false"/>
        </el-form-item>

        <el-form-item :label="$t('ui.Sys.Sys_DbBackupConfig.DbServer')">
          <el-input v-model="formData.DbServer" autocomplete="off" :disabled="false"/>
        </el-form-item>

        <el-form-item :label="$t('ui.Sys.Sys_DbBackupConfig.BackupFilePath')" prop="BackupFilePath">
          <el-input v-model="formData.BackupFilePath" autocomplete="off"/>
        </el-form-item>

        <!--        <el-form-item :label="$t('ui.Sys.Sys_DbBackupConfig.RestoreFilePath')" prop="RestoreFilePath">-->
        <!--          <el-input v-model="formData.RestoreFilePath" autocomplete="off" />-->
        <!--        </el-form-item>-->
        <!-- <el-form-item :label="$t('ui.Sys.Sys_DbBackupConfig.DbPassword')">
          <el-input v-model="formData.DbPassword" autocomplete="off" />
        </el-form-item> -->
        <!-- <el-form-item :label="$t('ui.Sys.Sys_DbBackupConfig.IsCompressed')">
          <el-checkbox v-model="formData.IsCompressed">压缩</el-checkbox>
        </el-form-item> -->
        <el-form-item :label="$t('Common.Remark')">
          <el-input v-model="formData.Remark" :autosize="{minRows: 3, maxRows: 5}" type="textarea"/>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">{{ $t('Common.cancel') }}</el-button>
        <el-button
          type="primary"
          @click="saveDbBackupSetting()"
        >{{ $t('Common.confirm') }}
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { fetchList, backUpDB, restoreDb, batchDelete } from '@/api/Sys/Sys_DbBackup'
import { fetchDbBackupConfig, saveDbBackupConfig } from '@/api/Sys/Sys_DbBackupConfig'
import { convertToKeyValue } from '@/utils'
import waves from '@/directive/waves' // waves directive 特效
import Pagination from '@/components/Pagination' // 分页
import { MessageBox } from 'element-ui' // 提示框
import { formatDate, formatDateTime } from '@/utils' // 列表内容格式化
import permission from '../../../directive/permission/permission'

export default {
  name: 'Sys.Sys_DbBackup',
  components: { Pagination },
  directives: { waves, permission },
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
      fullscreenLoading: false,
      list: null,
      selectedRowsData: [],
      total: 0,
      listLoading: true,
      listQuery: {
        keyword: '', // 用户姓名 | 登录账号
        dateValue: [new Date(new Date().setDate(new Date().getDate() - 30)), new Date()], // 最后需要加入查询条件listQuery
        PageNumber: 1,
        PageSize: 10
      },
      formData: {
        // 表单数据
        DbBackupSettingID: '',
        DbName: '',
        BackupFilePath: '',
        IsCompressed: true,
        DbServer: '',
        DbAccount: '',
        DbPassword: '',
        Remark: ''
      },
      formTitle: '', // 弹窗标题
      formMode: '',
      dialogFormVisible: false, // 表单窗口是否显示标志
      rules: {
        // 表单校验逻辑
        BackupFilePath: [
          { required: true, message: i18n.t('Common.ValidatorMessage.MustInput'), trigger: 'blur' }
        ],
        RestoreFilePath: [
          { required: true, message: i18n.t('Common.ValidatorMessage.MustInput'), trigger: 'blur' }
        ]
      },
      downloadLoading: false,
      tableHeight: 330
    }
  },
  computed: {
    disableRestore() {
      var selectRows = this.selectedRowsData || []
      return selectRows.length !== 1
    },
    disableDelete() {
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
        DbBackupSettingID: '',
        DbName: '',
        BackupFilePath: '',
        IsCompressed: true,
        DbServer: '',
        DbAccount: '',
        DbPassword: '',
        Remark: ''
      }
    },
    handleSelectChange(selection) {
      this.selectedRowsData = selection
    },
    handleBackupDB() {
      this.fullscreenLoading = true
      backUpDB().then(response => {
          this.fullscreenLoading = false
          this.getList()
          this.$notify({
            title: this.$t('Common.success'),
            message: this.$t('Common.operationSuccess'),
            type: 'success',
            duration: 2000
          })
        },
        reject => {
          this.fullscreenLoading = false
        })
    },
    handleRestoreDB() {
      this.fullscreenLoading = true
      restoreDb({ DbBackupID: this.selectedRowsData[0].DbBackupID }).then(response => {
          this.fullscreenLoading = false
          this.$notify({
            title: this.$t('Common.success'),
            message: this.$t('Common.operationSuccess'),
            type: 'success',
            duration: 2000
          })
        },
        reject => {
          this.fullscreenLoading = false
        })
    },
    handleDbBackupSetting() {
      this.resetFormData()
      fetchDbBackupConfig().then((response) => {
        this.formData = Object.assign({}, response.Data) // 对象拷贝
        this.formTitle = this.$t('ui.Sys.Sys_DbBackupConfig.BackupSetting')
        this.dialogFormVisible = true
        this.$nextTick(() => {
          this.$refs['dataForm'].clearValidate() // 清除校验
        })
      })
    },
    saveDbBackupSetting() {
      this.$refs['dataForm'].validate(valid => {
        if (valid) {
          saveDbBackupConfig(this.formData)
            .then(() => {
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
          return v.DbBackupID
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
    }
  }
}
</script>
