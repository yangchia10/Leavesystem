<template>
  <el-dialog
    :title="formTitle"
    :visible="dialogFormVisible"
    top="10vh"
    width="65%"
    @close="CloseHandel('关闭')"
  >
    <el-form
      ref="dataForm"
      :rules="rules"
      :model="formData"
      label-position="left"
      label-width="100px"
    >
      <el-row :gutter="15">
        <el-col :span="12">
          <el-form-item :label="$t('ui.Sys.Sys_User.UserName')" prop="UserName">
            <el-input v-model="formData.UserName" autocomplete="off"/>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item :label="$t('ui.Sys.Sys_User.LoginAccount')" prop="LoginAccount">
            <el-input v-model="formData.LoginAccount" autocomplete="off"/>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="所属类型" prop="Types">
            <el-select v-model="formData.Types" class="filter-item">
              <el-option
                v-for="item in optionsType"
                :key="item"
                :label="item"
                :value="item"
              />
            </el-select>
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <el-form-item label="产品">
            <el-input placeholder="请输入内容" v-model="chaInput" class="input-with-select">
              <el-button slot="append" icon="el-icon-zoom-in" @click="ChaHanlde">查询并添加</el-button>
            </el-input>
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>

    <div class="TableData">
      <el-table
        :data="rowDataList"
        border
        fit
        :header-cell-style="{ background: '#eef1f6', color: '#606266' }"
        highlight-current-row
        style="width: 100%"
        max-height="250"
      >
        <el-table-column label="#" type="index" align="center" width="40" fixed/>
        <el-table-column label="产品编码" prop="MaterialCode" align="center" show-overflow-tooltip/>
        <el-table-column label="产品名称" prop="MaterialName" align="left" show-overflow-tooltip/>
        <el-table-column label="规格型号" prop="Spec" align="center" show-overflow-tooltip/>
        <el-table-column label="图号" prop="DrawingNo" align="center" show-overflow-tooltip/>
        <el-table-column
          label="操作"
        >
          <template slot-scope="scope">
            <el-button type="primary" @click="DeleteHandle(scope.row)">删除</el-button>
          </template>
        </el-table-column>

      </el-table>
    </div>


    <div slot="footer" class="dialog-footer">
      <el-button @click="CloseHandel('关闭')">
        {{$t('Common.cancel') }}
      </el-button>
      <el-button
        type="primary"
        @click="UserMaterialHandle"
      >{{ $t('Common.confirm') }}
      </el-button>
    </div>
  </el-dialog>

</template>

<script>
import { UserMaterialAdd, GetMaterialList2 } from '@/api/Sys/Sys_User'

export default {
  name: 'UserMaterialList',
  props: {
    formTitle: String,
    dialogFormVisible: Boolean,
    formData: Object
  },
  data() {
    return {
      rules: {
        UserName: [
          { required: true, message: this.$t('Common.ValidatorMessage.MustInput'), trigger: 'change' }
        ],
        LoginAccount: [
          { required: true, message: this.$t('Common.ValidatorMessage.MustInput'), trigger: 'change' }
        ],
        Types: [
          { required: true, message: this.$t('Common.ValidatorMessage.MustInput'), trigger: 'change' }
        ]
      },
      optionsType: ['主机厂', '仓库配料'],

      rowDataList: [],

      chaInput: ''
    }
  },
  watch: {
    dialogFormVisible(item) {
      if (item) {
        this.rowDataList = []
        this.$nextTick(() => {
          this.$refs['dataForm'].clearValidate() // 清除校验
        })
      }
    }
  },
  methods: {
    UserMaterialHandle() {
      this.$refs['dataForm'].validate(valid => {
        if (valid) {
          if (this.rowDataList.length === 0) {
            this.$message.error('请添加数据')
            return
          }
          let dataList = this.rowDataList.map(item => {
            return {
              UserName: this.formData.UserName,
              LoginAccount: this.formData.LoginAccount,
              FID: item.FID,
              MaterialCode: item.MaterialCode,
              MaterialName: item.MaterialName,
              Type: this.formData.Types
            }
          })
          // 确认添加
          UserMaterialAdd(dataList).then(response => {
            if (response.Code === 2000) {
              this.$notify({
                title: this.$t('Common.success'),
                message: this.$t('Common.operationSuccess'),
                type: 'success',
                duration: 2000
              })
              this.CloseHandel('确定')
              this.rowDataList = []
            }

          })
        }
      })
    },
    // 查询产品
    ChaHanlde() {
      GetMaterialList2({ MaterialCode: this.chaInput }).then(res => {
        if (res.Code === 2000) {
          let findData = this.rowDataList.find(item => item.FID === res.Data[0].FID)
          if (findData) {
            this.$message.error('请勿重复添加')
            this.chaInput = ''
            return
          }
          this.rowDataList.push(res.Data[0])
          this.chaInput = ''
        }
      }).catch(res => {
        this.chaInput = ''
      })
    },
    DeleteHandle(row) {
      this.rowDataList.splice(this.rowDataList.indexOf(row), 1)
    },

    CloseHandel(data) {
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate() // 清除校验
      })
      this.$emit('CloseHandel', data)
    }
  }
}
</script>

<style scoped>

</style>
