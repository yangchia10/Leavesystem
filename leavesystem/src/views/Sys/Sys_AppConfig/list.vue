<template>
  <div class="app-container">
    <div class="filter-container">
      <el-input
        v-model="listQuery.keyword"
        class="filter-item"
        placeholder="版本号"
        style="width: 220px"
        @keydown.enter.native="handleFilter"
      />
      <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter"/>
      <hr/>

      <el-button
        v-waves
        v-permission="{ name: 'Sys.Sys_AppConfig.Add' }"
        class="filter-item"
        type="primary"
        icon="el-icon-plus"
        @click="handleCreate"
      >{{ $t('Common.add') }}
      </el-button>

    </div>
    <el-table
      ref="multipleTable"
      v-loading="listLoading"
      :data="list"
      border
      fit
      :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
      highlight-current-row
      style="width: 100%"
      :height="tableHeight"
      @sort-change="sortChange"
      @selection-change="handleSelectionChange"
    >
      <el-table-column
        :label="$t('Common.select')"
        type="selection"
        align="center"
        width="40"
        fixed="left"
      />
      <el-table-column
        label="#"
        type="index"
        align="center"
        width="40"
        fixed
      />
      <el-table-column
        label="版本号"
        prop="VersionCode"
        sortable="custom"
        align="center"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.VersionCode }}</span>
        </template>
      </el-table-column>
      <el-table-column
        label="文件名"
        prop="PathName"
        sortable="custom"
        align="center"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.PathName }}</span>
        </template>
      </el-table-column>
      <el-table-column
        label="文件路径"
        prop="PathUrl"
        sortable="custom"
        align="center"
        show-overflow-tooltip
      >
        <template slot-scope="scope">
          <span>{{ scope.row.PathUrl }}</span>
        </template>
      </el-table-column>
      <el-table-column
        :label="$t('Common.CUser')"
        prop="CUser"
        sortable="custom"
        align="center"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.CUser }}</span>
        </template>
      </el-table-column>
      <el-table-column
        :label="$t('Common.CTime')"
        prop="CUser"
        sortable="custom"
        align="center"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.CTime|datetime }}</span>
        </template>
      </el-table-column>
      <el-table-column
        :label="$t('table.actions')"
        align="center"
        width="200"
      >
        <template slot-scope="scope">
          <el-button
            size="mini"
            type="danger"
            v-permission="{ name: 'Sys.Sys_AppConfig.Delete' }"
            @click="handleDelete(scope.row,'deleted')">
            {{ $t('table.delete') }}
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <Pagination
      v-show="total > 0"
      :total="total"
      :page.sync="listQuery.PageNumber"
      :limit.sync="listQuery.PageSize"
      @pagination="getList"
    />

    <el-dialog :title="title" :visible.sync="dialogVisible" width="50%" :before-close="handleClose">
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="temp"
        label-position="left"
        label-width="100px"
      >
        <el-form-item label="版本号" prop="versionCode">
          <el-input v-model.number="temp.versionCode" placeholder="请输入版本号"></el-input>
        </el-form-item>
        <el-form-item label="选择文件" prop="file">
          <el-upload
            class="upload-demo"
            action="#"
            :headers="headers"
            :limit="1"
            :auto-upload="false"
            :before-upload="beforeAvatarUpload"
            :on-change="handleChange"
            :on-remove="handleRemove"
            :file-list="fileList"
            accept=".APK,.apk"
          >
            <el-button size="small" type="primary">点击上传</el-button>
            <div slot="tip" class="el-upload__tip">只能上传apk文件</div>
          </el-upload>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button v-waves @click="handleClose">
          {{$t('Common.cancel')}}
        </el-button>
        <el-button v-waves type="primary" @click="handleSave">
          {{$t('Common.save')}}
        </el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import waves from '../../../directive/waves/waves'
import Pagination from '../../../components/Pagination/index'
import {
  Add,
  getPageList,
  batchDelete
} from '../../../api/Sys/Sys_AppConfig'
// 当然你也可以为了方便使用，将它注册到全局
import permission from '@/directive/permission/index.js' // 权限判断指令
export default {
  name: 'Sys.Sys_AppConfig',
  components: {
    Pagination
  },
  directives: {
    waves,
    permission
  },
  data() {
    return {
      listLoading: false,
      list: [],
      total: 0,
      title: '',
      dialogVisible: false,
      //
      listQuery: {
        keyword: '',
        PageNumber: 1,
        PageSize: 10
      },
      edit: false,
      multipleSelection: [],
      temp: {
        versionCode: '',
        file: null
      },
      rules: {
        versionCode: [
          {
            required: true,
            message: '请输入版本号'
          },
          { type: 'number', message: '版本号必须为数字值' }
        ],
        file: [
          {
            required: true,
            message: '请选择文件',
            trigger: 'blur'
          }
        ]
      },
      fileList: [],
      headers: {
        AppID: 'P0001',
        'X-Token':
        this.$store.getters.token
      },

      tableHeight: 330
    }
  },
  filters: {},
  computed: {
    selective() {
      return this.multipleSelection.length !== 1
    },
    deletable() {
      return this.multipleSelection.length === 0
    },
    Select() {
      return this.multipleRegion.length === 0 || this.multipleRegion.length > 1
    },
    printDisable() {
      return this.multipleSelection.length <= 0
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
  activated() {
    this.$nextTick(() => {
      this.$refs.multipleTable.doLayout()
    })
  },
  methods: {
    getList() {
      // 获取库位信息表格数据
      this.listLoading = true
      getPageList(this.listQuery).then(res => {
        this.list = res.Data.items
        this.total = res.Data.total
        this.listLoading = false
      })
    },
    handleFilter() {
      this.getList()
    },
    resetFormData() {
      this.temp = {
        versionCode: '',
        file: ''
      }
      this.fileList = []
    },
    handleCreate() {
      // 点击新增按钮之后的操作
      this.resetFormData()
      this.title = this.$t('Common.add')
      this.formMode = 'Create'
      this.dialogVisible = true
      this.edit = false
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    handleEdit() {
      // 点击编辑按钮之后的操作
      // this.resetFormData()
      // this.GetCQ()
      // this.title = this.$t('Common.edit')
      // this.formMode = 'Edit'
      // this.dialogVisible = true
      // this.edit = true
      // this.temp = this.multipleSelection[0]
      // this.$nextTick(() => {
      //   this.$refs['dataForm'].clearValidate()
      // })
    },
    handleSave() {
      this.$refs.dataForm.validate(valid => {
        if (valid) {
          if (this.formMode === 'Create') {
            let formData = new FormData()
            // 普通表单数据
            formData.append('pathName', this.temp.pathName)
            formData.append('versionCode', this.temp.versionCode)
            // 文件数据
            formData.append('file', this.fileList[0].raw)

            Add(formData).then(res => {
              if (res.Code === 2000) {
                this.showNotify('success', 'Common.createSuccess')
              } else {
                this.showNotify('error', res.Message)
              }
              this.handleFilter()
            })
          } else {
          }
          this.dialogVisible = false
        } else {
          return false
        }
      })
    },
    handleDelete(row) {
      // 点击删除按钮之后的操作
      // const selectRows = this.multipleSelection
      this.$confirm(
        this.$i18n.t('Common.batchDeletingConfirm'),
        this.$i18n.t('Common.tip'),
        {
          confirmButtonText: this.$i18n.t('Common.affirm'),
          cancelButtonText: this.$i18n.t('Common.cancel'),
          type: 'warning'
        }
      ).then(() => {
        // const arrRowsID = selectRows.map(v => v.BinID)
        // 删除逻辑处理
        batchDelete([row.AppConfigID]).then(res => {
          if (res.Code === 2000) {
            this.showNotify('success', 'Common.deleteSuccess')
          } else {
            this.showNotify('error', res.Message)
          }
          this.getList()
        })
      })
    },
    handleClose() {
      this.dialogVisible = false
      this.handleFilter()
    },
    sortChange(data) {
      const { prop, order } = data
      if (prop && order) {
        if (order === 'ascending') {
          this.listQuery.sort = prop + ' asc'
        } else {
          this.listQuery.sort = prop + ' desc'
        }
      } else {
        this.listQuery.sort = ''
      }
      this.handleFilter()
    },
    handleSelectionChange(val) {
      this.multipleSelection = val
    },

    beforeAvatarUpload(file) {
      // const isJPG = file.type === 'APL' || file.type === 'image/png' || file.type === 'image/bmp' || file.type === 'image/gif'
      // const isLt50M = file.size / 1024 / 1024 < 5
      // console.log(file)
      // if (!isJPG && this.formData.FileType === '2') {
      //   this.$message.error('上传只能是 JPG/PNG/BMP/GIF 格式!')
      //   return false
      // }
    },
    // 改变时
    handleChange(file, fileList) {
      this.fileList = fileList
      this.temp.file = file
      // console.log(file)
    },
    // 删除事件
    handleRemove(file, fileList) {
      console.log(file)
      this.fileList = fileList
      this.temp.file = ''
    }

  }
}
</script>
