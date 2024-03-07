<template>
  <div class="app-container">
    <div class="filter-container">
      <el-input
        v-model="query.keyword"
        class="filter-item"
        placeholder="标题 | 内容"
        style="width: 250px"
        @keydown.enter.native="handleFilter"
      />
      <el-button class="filter-item" type="primary" icon="el-icon-search" @click="handleFilter"/>
      <hr>
      <el-button
        v-waves
        class="filter-item"
        type="success"
        icon="el-icon-plus"
        @click="handleAdd"
      >添加
      </el-button>
      <!--      <el-button-->
      <!--        v-waves-->
      <!--        v-permission="{ name: 'PP.PP_HalfWarehousing.Delete' }"-->
      <!--        class="filter-item"-->
      <!--        type="danger"-->
      <!--        icon="el-icon-delete"-->
      <!--        :disabled="isDelete"-->
      <!--        @click="handleDelete('HalfWarehousingID',true)"-->
      <!--      >{{ $t('Common.delete') }}-->
      <!--      </el-button>-->

      <!--      <el-button-->
      <!--        v-waves-->
      <!--        v-permission="{ name: 'PP.PP_HalfWarehousing.Export' }"-->
      <!--        class="filter-item"-->
      <!--        type="primary"-->
      <!--        icon="el-icon-download"-->
      <!--        @click="handleExport"-->
      <!--      >{{ $t('Common.export') }}-->
      <!--      </el-button>-->
    </div>
    <el-table
      ref="multipleTable"
      v-loading="table.loading"
      :data="table.data"
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
        fixed
      />
      <el-table-column
        label="#"
        type="index"
        align="center"
        width="40"
        fixed
      />
      <el-table-column
        label="消息类型"
        prop="MessageTypeDesc"
        sortable="custom"
        align="center"
        width="150"
        show-overflow-tooltip
      />
      <el-table-column
        label="标题"
        prop="MessageTitle"
        sortable="custom"
        align="left"
        width="300"
        show-overflow-tooltip
      />
      <el-table-column
        label="内容"
        prop="MessageBody"
        sortable="custom"
        align="left"
        width="300"
        show-overflow-tooltip
      />
      <el-table-column
        :label="$t('Common.CUser')"
        prop="CUser"
        sortable="custom"
        align="center"
        width="150"
        show-overflow-tooltip
      />
      <el-table-column
        :label="$t('Common.CTime')"
        prop="CTime"
        sortable="custom"
        align="center"
        width="160"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.CTime| datetime }}</span>
        </template>
      </el-table-column>
    </el-table>
    <Pagination
      v-show="table.total > 0"
      :total="table.total"
      :page.sync="query.pageNumber"
      :limit.sync="query.pageSize"
      @pagination="fetchList"
    />


    <el-dialog
      title="添加消息通知"
      :visible.sync="dialogFormVisible"
      top="10vh"
      width="65%"
    >
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="formData"
        label-position="left"
        label-width="100px"
      >
        <el-row :gutter="15">
          <el-col :span="24">
            <el-form-item label="标题" prop="MessageTitle">
              <el-input v-model="formData.MessageTitle" autocomplete="off"/>
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item label="内容" prop="MessageBody">
              <el-input
                v-model="formData.MessageBody"
                :autosize="{ minRows: 3, maxRows: 5 }"
                type="textarea"
              />
            </el-form-item>
          </el-col>
          <el-col :span="24">
            <el-form-item label="所属类型" prop="MessageTypeID">
              <el-select v-model="formData.MessageTypeID" class="filter-item">
                <el-option
                  v-for="item in messageTypeList"
                  :key="item.MessageTypeID"
                  :label="item.MessageTypeDesc"
                  :value="item.MessageTypeID"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
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
          {{$t('Common.cancel') }}
        </el-button>
        <el-button
          type="primary"
          @click="updateData"
        >{{ $t('Common.confirm') }}
        </el-button>
      </div>
    </el-dialog>


  </div>
</template>

<script>
// 页面混入
import pageMixin from '@/mixins/pageMixin'
import { GetPageList, Add } from '../../../api/Sys/Sys_PostMessage'
import { fetchList as fetchMessageTypeList } from '@/api/Sys/Sys_MessageType'

export default {
  name: 'Sys.Sys_PostMessage',
  mixins: [pageMixin],
  components: {},
  filters: {},
  data() {
    return {
      query: {
        keyword: ''
      },

      dialogFormVisible: false,
      formData: {
        MessageTypeID: '',
        MessageTitle: '',
        MessageBody: '',
        Remark: ''
      },
      rules: {
        MessageTitle: [
          {
            required: true,
            message: this.$t('Common.ValidatorMessage.MustInput'),
            trigger: 'blur'
          }
        ],
        MessageBody: [
          {
            required: true,
            message: this.$t('Common.ValidatorMessage.MustInput'),
            trigger: 'blur'
          }
        ],
        MessageTypeID: [
          {
            required: true,
            message: this.$t('Common.ValidatorMessage.MustInput'),
            trigger: 'blur'
          }
        ]
      },

      messageTypeList: []

    }
  },
  computed: {},
  created() {
    this.fetchList()
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
    GetPageList,
    updateData() {
      this.$refs['dataForm'].validate(valid => {
        if (valid) {
          Add(this.formData).then(res => {
            if (res.Code === 2000) {
              this.showNotify('success', 'Common.operationSuccess')
              this.fetchList(0)
              this.fetchList(0)
            } else {
              this.showNotify('error', res.Message)
            }
            this.dialogFormVisible = false
          })

        }
      })
    },

    handleAdd() {
      this.formData = {
        MessageTypeID: '',
        MessageTitle: '',
        MessageBody: '',
        Remark: ''
      }
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate() // 清除校验
      })
      this.getMessageTypeList()
      this.dialogFormVisible = true
    },

    getMessageTypeList() {
      fetchMessageTypeList().then(response => {
        this.messageTypeList = response.Data
      })
    }
  }
}
</script>
