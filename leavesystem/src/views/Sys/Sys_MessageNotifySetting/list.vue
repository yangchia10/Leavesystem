<template>
  <div class="app-container">
    <el-container>
      <el-aside style="width:260px;background:#fff;border-right:solid 1px #ddd">
        <!-- <el-header style="background:#DDD">消息分类</el-header> -->
        <el-menu
          style="border:0px"
          :default-openeds="['1']"
          :default-active="defaultActiveMessageTypeId"
          @select="handleSelectMessageType"
        >
          <el-submenu index="1">
            <template slot="title">
              <i class="el-icon-menu"/>
              <span>{{$t('ui.Sys.Sys_MessageNotifySetting.MessageType')}}</span>
            </template>
            <el-menu-item
              v-for="subItem in messageTypeList"
              :key="subItem.MessageTypeID"
              :index="subItem.MessageTypeID"
            >
              <!-- <i class="el-icon-menu"></i> -->
              <span slot="title">{{ subItem.MessageTypeDesc }}</span>
            </el-menu-item>
          </el-submenu>
        </el-menu>
      </el-aside>

      <el-container style="padding-left:20px">
        <el-main>
          <el-form label-position="left" label-width="0px">
            <el-form-item>
              <!--              <el-checkbox v-model="IsNotifyByEmail">{{-->
              <!--                $t('ui.Sys.Sys_MessageNotifySetting.IsNotifyByEMail')-->
              <!--                }}-->
              <!--              </el-checkbox>-->
              <!--              <el-button-->
              <!--                type="primary"-->
              <!--                plain-->
              <!--                @click="handleSaveMessageNotifySetting"-->
              <!--              >{{ $t('Common.save') }}-->
              <!--              </el-button>-->

              <el-button
                type="primary"
                plain
                icon="el-icon-plus"
                @click="handleAdd"
              >添加
              </el-button>
            </el-form-item>
          </el-form>

          <el-table
            ref="multipleUserTable"
            v-loading="listLoading"
            :data="list"
            border
            :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
            highlight-current-row
            style="width: 100%;"
            max-height="450"
          >
            <!--            <el-table-column type="selection" width="40" fixed/>-->
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
              :label="$t('ui.Sys.Sys_User.UserID')"
              width="150"
              show-overflow-tooltip
            >
              <template slot-scope="scope">
                <span>{{ scope.row.UserID }}</span>
              </template>
            </el-table-column>

            <el-table-column
              width="150px"
              align="center"
              prop="UserName"
              :label="$t('ui.Sys.Sys_User.UserName')"
              show-overflow-tooltip
            />

            <el-table-column
              width="120px"
              align="center"
              prop="FrgnName"
              :label="$t('ui.Sys.Sys_User.FrgnName')"
            >
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

            <el-table-column
              width="120px"
              align="center"
              prop="LoginAccount"
              :label="$t('ui.Sys.Sys_User.LoginAccount')"
            >
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

            <el-table-column
              width="260px"
              align="center"
              prop="Email"
              :label="$t('ui.Sys.Sys_User.Email')"
            />

            <el-table-column
              width="120px"
              align="center"
              prop="Mobile"
              :label="$t('ui.Sys.Sys_User.Mobile')"
            />

            <el-table-column
              width="120px"
              align="center"
              prop="Telphone"
              :label="$t('ui.Sys.Sys_User.Telphone')"
            />

            <el-table-column
              width="60px"
              align="center"
              prop="Gender"
              :label="$t('ui.Sys.Sys_User.Gender')"
              :formatter="formatGender"
            />

            <el-table-column
              width="90px"
              align="center"
              :label="$t('ui.Sys.Sys_User.IsEnable')"
            >
              <template slot-scope="{ row }">
                <el-tag v-if="row.IsEnable" type="success">{{
                  row.IsEnable | statusFilter
                  }}
                </el-tag>
                <el-tag v-else type="danger">{{
                  row.IsEnable | statusFilter
                  }}
                </el-tag>
              </template>
            </el-table-column>

            <el-table-column
              width="100px"
              align="center"
              prop="Remark"
              :label="$t('Common.Remark')"
            />

            <el-table-column
              label="操作"
              fixed="right"
            >
              <template slot-scope="scope">
                <el-button type="danger" @click="DeleteList(scope.row)">删除</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-main>
      </el-container>
    </el-container>


    <MessageSettings
      form-title="分配用户消息分类"
      :dialog-form-visible="visible"
      :data-list="list"
      :data-id="defaultActiveMessageTypeId"
      @CloseHandel="visible=false,GetMessage(defaultActiveMessageTypeId)"></MessageSettings>
  </div>
</template>

<script>

import { fetchAllUser } from '@/api/Sys/Sys_User'
import { fetchList as fetchOrganizationList } from '@/api/Sys/Sys_Organization'
import { fetchList as fetchMessageTypeList, GetMessageDetail, Delete } from '@/api/Sys/Sys_MessageType'
import { GetMessageNotifySettingDetailInfo, SaveMessageNotifySetting } from '@/api/Sys/Sys_MessageNotifySetting'

import { convertToKeyValue } from '@/utils'
import waves from '@/directive/waves' // waves directive 特效
import Pagination from '@/components/Pagination' // 分页
import { MessageBox } from 'element-ui' // 提示框

import { formatDate, formatDateTime } from '@/utils' // 列表内容格式化

import MessageSettings from '../../Components/MessageSetting/MessageSetting'

const statusMapArr = [
  { value: true, label: i18n.t('Dictionary.AccountIsEnableMap.TrueValue') }, // 正常
  { value: false, label: i18n.t('Dictionary.AccountIsEnableMap.FalseValue') } // 冻结
]

export default {
  name: 'SysMessageNotifySetting',
  components: { Pagination, MessageSettings },
  directives: { waves },
  filters: {
    statusFilter(status) {
      const statusMap = convertToKeyValue(statusMapArr)
      return statusMap[status]
    }
  },
  data() {
    return {
      list: [],
      organizationList: [],
      messageTypeList: null,
      IsNotifyByEmail: false, // 邮件
      defaultActiveMessageTypeId: '', // 默认激活消息分类项
      genderOptions: [
        { value: 1, label: this.$t('Dictionary.GenderMap.One') }, // 男
        { value: 2, label: this.$t('Dictionary.GenderMap.Two') } // 女
      ],
      selectedRowsData: [],
      selectedMessageTypeID: '',
      listLoading: true,
      listQuery: {
        keyword: '', // 用户姓名 | 登录账号
        PageNumber: 1,
        PageSize: 10
      },
      downloadLoading: false,

      visible: false
    }
  },
  computed: {
    canNotUpdate() {
      var selectRows = this.selectedRowsData || []
      return selectRows.length !== 1
    }
  },
  created() {
    // this.getUserList()
    this.getOrganizationList() // 组织机构Formatter
    this.getMessageTypeList()
  },
  methods: {
    formatDate,
    formatDateTime,
    formatGender(row, column, currentValue) {
      const opt = this.genderOptions.find((element) => {
        return element.value === currentValue
      })
      return opt === undefined ? '' : opt.label
    },
    formatOrganization(row, column, currentValue) {
      const findedOrgation = this.organizationList.find((element) => {
        return element.OrganizationID === currentValue
      })
      return findedOrgation != null ? findedOrgation.OrganizationDesc : ''
    },
    getUserList() {
      // 获取数据
      this.listLoading = true
      fetchAllUser().then(response => {
        this.list = response.Data
        this.listLoading = false
      })
    },
    getOrganizationList() {
      // 获取数据
      this.listLoading = true
      fetchOrganizationList(this.listQuery).then(response => {
        // console.log('getOrganizationList',response)
        this.organizationList = response.Data.items
        this.listLoading = false
      })
    },
    getMessageTypeList() {
      this.listLoading = true
      fetchMessageTypeList().then(response => {
        this.messageTypeList = response.Data
        this.defaultActiveMessageTypeId = response.Data[0].MessageTypeID
        this.selectedMessageTypeID = response.Data[0].MessageTypeID
        // this.initSelectMessageType(this.defaultActiveMessageTypeId)
        this.GetMessage(this.defaultActiveMessageTypeId)
        this.listLoading = false
      })
    },
    handleFilter() {
      this.getList()
    },
    handleSelectMessageType(key, keyPath) {
      // 消息分类点击响应事件处理（右侧信息初始化）
      // this.initSelectMessageType(key)
      this.defaultActiveMessageTypeId = key
      this.GetMessage(key)
    },
    // initSelectMessageType(messageTypeID) {
    //   this.selectedMessageTypeID = messageTypeID
    //   GetMessageNotifySettingDetailInfo({ MessageTypeID: messageTypeID })
    //     .then((response) => {
    //       this.IsNotifyByEmail = response.Data.IsNotifyByEmail
    //       this.$refs.multipleUserTable.clearSelection()
    //       response.Data.UserIDS.forEach(element => {
    //         var item = this.list.find(function(x) {
    //           return x.UserID == element
    //         })
    //         if (item) {
    //           console.log('initSelectMessageType', item)
    //           this.$refs.multipleUserTable.toggleRowSelection(item, true)
    //         }
    //       })
    //     })
    // },

    // 获取分类详情
    GetMessage(messageTypeID) {
      this.listLoading = true
      GetMessageDetail({ MessageTypeID: messageTypeID }).then(res => {
        if (res.Code === 2000) {
          this.list = res.Data.UserIDS
          this.listLoading = false
        }
      })
    },

    handleSaveMessageNotifySetting() {
      // 选中用户列表
      // const selectUserRows = this.$refs.multipleUserTable.store.states.selection
      const selectUserRows = this.$refs.multipleUserTable.selection
      console.log('selectUserRows11111', selectUserRows)
      let userIds = []
      if (selectUserRows !== null && selectUserRows.length > 0) {
        userIds = selectUserRows.map(function(item) {
          return item.UserID
        })
      }
      console.log('selectUserRows112', userIds)
      SaveMessageNotifySetting({
        MessageTypeID: this.selectedMessageTypeID,
        IsNotifyByEmail: this.IsNotifyByEmail,
        UserIDS: userIds
      })
        .then((res) => {
          this.$notify({
            title: this.$t('Common.success'),
            message: this.$t('Common.operationSuccess'),
            type: 'success',
            duration: 2000
          })
        })

    },

    //   添加
    handleAdd() {
      this.visible = true
    },

    DeleteList(row) {
      this.$confirm(
        this.$i18n.t('Common.batchDeletingConfirm'),
        this.$i18n.t('Common.tip'),
        {
          confirmButtonText: this.$i18n.t('Common.affirm'),
          cancelButtonText: this.$i18n.t('Common.cancel'),
          type: 'warning'
        }
      ).then(() => {
        // const arrRowsID = this.multipleSelection.map(v => v.HTFID)
        // 删除逻辑处理
        Delete([row.UserID]).then(res => {
          if (res.Code === 2000) {
            this.showNotify('success', 'Common.deleteSuccess')
          } else {
            this.showNotify('error', res.Message)
          }
          this.GetMessage(this.defaultActiveMessageTypeId)
        })
      })

    }

  }
}
</script>

