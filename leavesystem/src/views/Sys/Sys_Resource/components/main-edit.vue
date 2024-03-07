<template>
  <el-dialog v-el-drag-dialog :visible.sync="visible" title="編輯" width="80%">
    <el-form ref="form" :model="form" :rules="rules" label-width="20%" size="small">
      <el-row>
        <el-col :span="8">
          <el-form-item label="門市代號" prop="CardCode">
            <el-input v-model="form.CardCode" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="門市名稱" prop="CardName">
            <el-input v-model="form.CardName" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="叫修人員" prop="CUser">
            <el-input v-model="form.CUser" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="叫修單號" prop="DocNum">
            <el-input v-model="form.DocNum" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="審核人員" prop="USER_CODE">
            <el-input v-model="form.USER_CODE" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="審核狀態" prop="CheckStatus">
            <el-input v-model="form.CheckStatus" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="SAP單號" prop="SAPDocNum">
            <el-input v-model="form.SAPDocNum" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="叫修日期" prop="CTime">
            <el-input v-model="form.CTime" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="叫修單狀態" prop="DocStatus">
            <el-input v-model="form.DocStatus" class="filter-item-input" />
          </el-form-item>
        </el-col>

        <!--<el-col :span="12">
          <el-form-item label="車間" prop="WorkShopCode">
            <el-select v-model="form.WorkShopCode" class="filter-item-input" placeholder="車間" clearable @change="changeWorkShopCode">
              <el-option v-for="item in dicts.level1" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
        </el-col>-->
      </el-row>
      <el-row>
        <el-col :span="20">
          <el-form-item label="備註" prop="Remark">
            <el-input v-model="form.Remark" type="textarea" />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <div slot="footer" class="dialog-footer">
      <el-button type="text" @click="close">取消</el-button>
      <el-button type="primary" @click="handleSave()">確認</el-button>
    </div>
  </el-dialog>
</template>

<script>
import { formSubmit } from '@/mixins'
import { UpdateMain as Edit, InsertMainEntity as Add } from '@/api/Sys/Sys_Resource'

export default {
  mixins: [formSubmit],
  data() {
    return {
      // 開關
      visible: false,
      form: {
        RowUid: '',
        RouteCode: '',
        RouteName: '',
        ItemCode: '',
        ItemName: '',
        WorkShopCode: '',
        WorkShopName: ''
      },
      types: [{ type: 1, func: Add }, { type: 2, func: Edit }]
    }
  },
  mounted() {

  },
  methods: {
    /**
     * 提交資料後
     */
    handleSaveLater(res) {
      console.log(res, 89)
      if (res.Code === 200) {
        this.$emit('fetch-list')
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.filter-item-input {
  width: 80%;
}
</style>
