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
        :placeholder="$t('ui.Sys.Sys_UserMessage.Placeholder.Keyword')"
        style="width: 220px;"
        class="filter-item"
        @keyup.enter.native="handleFilter"
      />

      <!-- <el-checkbox v-model="listQuery.notRead">未读</el-checkbox> -->

      <el-select
        v-model="listQuery.IsReaded"
        :placeholder="$t('ui.Sys.Sys_UserMessage.IsReaded')"
        style="width: 150px"
        class="filter-item"
        clearable
      >
        <el-option
          v-for="item in IsReadedOptions"
          :key="item.key"
          :label="item.label"
          :value="item.key"
        />
      </el-select>

      <el-button
        v-waves
        class="filter-item"
        type="primary"
        icon="el-icon-search"
        @click="handleFilter"
      />


      <hr/>
      <el-button
        v-waves
        class="filter-item"
        type="warning"
        icon="el-icon-plus"
        @click="handleSetIsReaded"
      >{{ $t('ui.Sys.Sys_UserMessage.SetIsReaded') }}
      </el-button>

    </div>

    <!-- 数据列表 -->
    <el-table
      :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
      ref="multipleTable"
      v-loading="listLoading"
      :data="list"
      border
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
        width="150"
        align="center"
        prop="MessageTypeDesc"
        :label="$t('ui.Sys.Sys_UserMessage.MessageTypeDesc')"
      />

      <el-table-column
        width="150"
        align="center"
        prop="UserName"
        :label="$t('ui.Sys.Sys_UserMessage.UserName')"
      />
      <el-table-column
        width="150"
        align="center"
        prop="MessageTitle"
        :label="$t('ui.Sys.Sys_UserMessage.MessageTitle')"
      />
      <el-table-column
        width="250"
        align="center"
        prop="MessageContent"
        :label="$t('ui.Sys.Sys_UserMessage.MessageContent')"
        show-overflow-tooltip
      />
      <el-table-column
        width="150"
        align="center"
        prop="NotifyType"
        :label="$t('ui.Sys.Sys_UserMessage.NotifyType')"
      />
      <!-- <el-table-column
        width="160px"
        align="center"
        prop="Publisher"
        :label="$t('ui.Sys.Sys_UserMessage.Publisher')"
      ></el-table-column> -->
      <el-table-column
        width="150"
        align="center"
        prop="CTime"
        :label="$t('ui.Sys.Sys_UserMessage.PublishTime')"
        :formatter="formatDateTime"
      />
      <el-table-column
        width="80"
        align="center"
        :label="$t('ui.Sys.Sys_UserMessage.IsReaded')"
        fixed
      >
        <template slot-scope="{ row }">
          <el-tag v-if="row.IsReaded" type="success">{{ row.IsReaded | isReadedFilter}}
          </el-tag>
          <el-tag v-else type="danger">{{ row.IsReaded | isReadedFilter}}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column
        width="150"
        align="center"
        prop="ReadTime"
        :label="$t('ui.Sys.Sys_UserMessage.ReadTime')"
        :formatter="formatDateTime"
      />
      <el-table-column
        width="150"
        align="center"
        prop="Remark"
        :label="$t('ui.Sys.Sys_UserMessage.Remark')"
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
  </div>
</template>

<script>
import { fetchList, add, update, batchDelete, batchReadMessage } from '@/api/Sys/Sys_UserMessage'
import { convertToKeyValue } from '@/utils'
import waves from '@/directive/waves' // waves directive 特效
import Pagination from '@/components/Pagination' // 分页
import { MessageBox } from 'element-ui' // 提示框
import { formatDate, formatDateTime } from '@/utils' // 列表内容格式化

export default {
  name: 'Sys.Sys_UserMessage',
  components: { Pagination },
  directives: { waves },
  filters: {
    isReadedFilter(read) {
      const IsReadedDic = [
        { value: true, label: i18n.t('Dictionary.IsReadedMap.TrueValue') }, // 正常
        { value: false, label: i18n.t('Dictionary.IsReadedMap.FalseValue') } // 冻结
      ]
      const isReadedMap = convertToKeyValue(IsReadedDic)
      return isReadedMap[read]
    }
  },
  data() {
    return {
      list: null,
      selectedRowsData: [],
      total: 0,
      listLoading: true,
      listQuery: {
        keyword: '', // 用户姓名 | 登录账号
        IsReaded: false,
        dateValue: [new Date(new Date().setDate(new Date().getDate() - 7)), new Date()], //
        // notRead:true,
        PageNumber: 1,
        PageSize: 10
      },
      formData: {
        // 表单数据
        UserMessageID: '',
        MessageID: '',
        MessageTypeID: '',
        MessageTypeDesc: '',
        UserID: '',
        UserName: '',
        MessageTitle: '',
        MessageContent: '',
        NotifyType: '',
        Publisher: '',
        PublishTime: '',
        MessageStatus: '',
        ReadTime: '',
        Remark: ''
      },
      IsReadedOptions: [
        {
          label: this.$i18n.t('Common.all'),
          key: ''
        },
        {
          label: this.$i18n.t('Dictionary.IsReadedMap.TrueValue'),
          key: true
        },
        {
          label: this.$i18n.t('Dictionary.IsReadedMap.FalseValue'),
          key: false
        }
      ],
      formTitle: '', // 弹窗标题
      formMode: '',
      dialogFormVisible: false, // 表单窗口是否显示标志
      rules: {
        // 表单校验逻辑
        UserName: [
          { required: true, message: '用户名必须输入', trigger: 'change' }
        ]
      },
      downloadLoading: false,
      tableHeight: 330
    }
  },
  computed: {
    // canNotUpdate() {
    //   var selectRows = this.selectedRowsData || []
    //   return selectRows.length !== 1
    // }
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
      // console.log('Sys_UserMessage', this.listQuery)
      fetchList(this.listQuery).then(response => {
        this.list = response.Data.items
        this.total = response.Data.total
        this.listLoading = false
        this.$store.dispatch('Message/getMessageNum')
      })
    },
    handleFilter() {
      this.getList()
    },
    resetFormData() {
      // 重置表单数据
      this.formData = {
        UserMessageID: '',
        MessageID: '',
        MessageTypeID: '',
        MessageTypeDesc: '',
        UserID: '',
        UserName: '',
        MessageTitle: '',
        MessageContent: '',
        NotifyType: '',
        Publisher: '',
        PublishTime: '',
        MessageStatus: '',
        ReadTime: '',
        Remark: ''
      }
    },
    handleSelectChange(selection) {
      this.selectedRowsData = selection
    },
    handleSetIsReaded() {
      console.log(this.selectedRowsData)
      var arrRowsID = this.selectedRowsData.map(v => v.UserMessageID)
      batchReadMessage(arrRowsID).then(res => {
        if (res.Code === 2000) {
          this.showNotify('success', 'Common.operationSuccess')
        } else {
          this.showNotify('error', res.Message)
        }
        this.getList()
      })

    }
  }
}
</script>

