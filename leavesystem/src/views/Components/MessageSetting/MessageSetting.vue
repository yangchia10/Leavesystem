<template>
  <el-dialog
    :title="formTitle"
    :visible="dialogFormVisible"
    top="10vh"
    width="65%"
    @close="CloseHandel('关闭')"
  >
    <div class="TableData">

      <el-tag
        v-for="(tag,index) in TagsList"
        closable
        :key="index"
        @close="DeleteHandle(tag)">
        {{tag.UserName}}
      </el-tag>
      <el-table
        :data="rowDataList"
        border
        fit
        :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
        highlight-current-row
        style="width: 100%"
        max-height="250"
        @row-click="rowHandle"
      >
        <el-table-column
          label="#"
          type="index"
          align="center"
          width="40"
          fixed
        />
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
          show-overflow-tooltip
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


    <div slot="footer" class="dialog-footer">
      <el-button @click="CloseHandel('关闭')">
        {{$t('Common.cancel') }}
      </el-button>
      <el-button
        type="primary"
        @click="confirmHandle"
      >{{ $t('Common.confirm') }}
      </el-button>
    </div>
  </el-dialog>

</template>

<script>
import { fetchList, UserMaterialAdd, GetMaterialList2 } from '@/api/Sys/Sys_User'
import Pagination from '@/components/Pagination'
import { fetchList as fetchOrganizationList } from '@/api/Sys/Sys_Organization'
import { SaveMessageSetting } from '@/api/Sys/Sys_MessageType'

export default {
  name: 'MessageSetting',
  props: {
    formTitle: String,
    dialogFormVisible: Boolean,
    dataList: Array,

    dataId: String
  },
  components: { Pagination },
  data() {
    return {
      rules: {},
      rowDataList: [],
      total: 0,
      listLoading: false,

      listQuery: {
        keyword: '', // 用户姓名 | 登录账号
        PageNumber: 1,
        PageSize: 10
      },

      TagsList: []
    }
  },
  watch: {
    dialogFormVisible(item) {
      if (item) {
        this.getOrganizationList()
        this.rowDataList = []

        if (this.dataList.length !== 0) {
          this.TagsList = this.dataList
        }
      }
    }
  },
  methods: {

    getList() {
      // 获取数据
      this.listLoading = true
      fetchList(this.listQuery).then(response => {
        if (response.Code === 2000) {
          this.rowDataList = response.Data.items
          this.total = response.Data.total
          this.listLoading = false
        }
      })
    },
    formatOrganization(row, column, currentValue) {
      const findedOrgation = this.organizationList.find((element) => {
        return element.OrganizationID === currentValue
      })
      return findedOrgation != null ? findedOrgation.OrganizationDesc : ''
    },
    getOrganizationList() {
      // 获取数据
      this.listLoading = true
      fetchOrganizationList(this.listQuery).then(response => {
        // console.log('getOrganizationList',response)
        this.organizationList = response.Data.items
        this.listLoading = false
        this.getList()
      })
    },
    confirmHandle() {
      // this.$refs['dataForm'].validate(valid => {
      //   if (valid) {
      if (this.TagsList.length === 0) {
        this.$message.error('请添加数据')
        return
      }
      let dataList = {
        MessageTypeID: this.dataId,
        IsNotifyByEmail: true,
        UserIDS: this.TagsList.map(item => item.UserID)
      }
      // 确认添加
      SaveMessageSetting(dataList).then(response => {
        if (response.Code === 2000) {
          this.$notify({
            title: this.$t('Common.success'),
            message: this.$t('Common.operationSuccess'),
            type: 'success',
            duration: 2000
          })
          this.CloseHandel('确定')
        }

      })
      // }
      //   })
    },

    DeleteHandle(tag) {
      this.TagsList.splice(this.TagsList.indexOf(tag), 1)
    }
    ,

    rowHandle(row) {
      let findData = this.TagsList.find(item => item.UserID === row.UserID)
      if (findData) {
        this.$message.error('请勿重新添加')
        return
      }
      this.TagsList.push(row)
    }
    ,

    CloseHandel(data) {
      this.TagsList = []
      this.listQuery.pageNumber = 1
      this.rowDataList = []
      this.$emit('CloseHandel', data)
    }
  }
}
</script>

<style scoped>

</style>
