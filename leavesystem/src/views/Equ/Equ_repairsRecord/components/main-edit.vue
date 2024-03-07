<template>
  <el-dialog v-el-drag-dialog :visible.sync="visible" title="編輯" width="80%">
    <el-form ref="form" :model="form" :rules="rules" label-width="20%" size="small">
      <el-row>
        <el-col :span="8">
          <el-form-item label="門市代號" prop="CardCode">
            <!-- <el-input v-model="form.CardCode" class="filter-item-input" /> -->
            <el-select v-model="form.CardCode" class="filter-item filter-item-input" clearable placeholder="門市代號" @change="changeWorkShopCode">
              <el-option v-for="item in dicts" :key="item.CardCode" :label="item.CardCode" :value="item.CardCode" />
            </el-select>
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
            <el-input v-model="form.DocNum" class="filter-item-input" readonly="true" background-color="#cccccc" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="審核人員" prop="USER_CODE">
            <el-input v-model="form.USER_CODE" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="審核狀態" prop="CheckStatus">
            <!-- <el-input v-model="form.CheckStatus" class="filter-item-input" /> -->
            <el-select v-model="form.CheckStatus" class="filter-item">
              <el-option v-for="item in CheckOptions" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="SAP單號" prop="SAPDocNum">
            <el-input v-model="form.SAPDocNum" class="filter-item-input" readonly="true" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="叫修日期" prop="CTime">
            <el-input v-model="form.CTime" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="叫修單狀態" prop="DocStatus">
            <!-- <el-input v-model="form.DocStatus" class="filter-item filter-item-input" clearable /> -->
            <el-select v-model="form.DocStatus" class="filter-item">
              <el-option v-for="item in genderOptions" :key="item.value" :label="item.label" :value="item.value" />
            </el-select>

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
import { UpdateMain as Edit, InsertMainEntity as Add } from '@/api/MD/MD_ProcessRoute'
import { GetWorkshop as getDicts } from '@/api/MD/MD_Process'
import { GetOCRDList } from '@/api/MD/MD_ProcessRoute'

export default {
  mixins: [formSubmit],
  data() {
    return {
      // 開關
      visible: false,
      dicts: [],
      form: {
        RowUid: '',
        RouteCode: '',
        RouteName: '',
        ItemCode: '',
        ItemName: '',
        WorkShopCode: '',
        WorkShopName: ''
      },
      genderOptions: [{
        value: 1,
        label: this.$t('Dictionary.OC.Open')
      }, //
      {
        value: 0,
        label: this.$t('Dictionary.OC.Close')
      } //
      ],
      CheckOptions: [{
        value: 1,
        label: '已審核'
      }, //
      {
        value: 0,
        label: '未審核'
      } //
      ],

      types: [{ type: 1, func: Add }, { type: 2, func: Edit }]
    }
  },
  created() {
    this.GetWarehouseAll()
  },
  mounted() {
    // location.replace('https://www.google.com.tw/?hl=zh_TW')
    getDicts().then(res => {
      const list = res.Data || []
      this.dicts.level1 = list
        .map(e => ({
          value: e.WorkshopCode,
          label: e.WorkshopName
        }))
    })
  },
  methods: {
    /**
     * 改變車間
     */
    changeWorkShopCode(val) {
      this.dicts.filter((item, index) => {
        if (item.CardCode === val) {
          console.log(item)
          this.form.CardName = item.CardName
        }
      })
    },
    /**
     * 提交資料後
     */
    handleSaveLater(res) {
      if (res.Code === 200) {
        this.$emit('fetch-list')
      }
    },
    // 获取仓库列表
    GetWarehouseAll() {
      GetOCRDList().then(res => {
        if (res.Code === 2000) {
          this.dicts = res.Data
        }
      })
    }
  }
}
</script>

<style lang="scss" scoped>
.filter-item-input {
  width: 80%;
}
</style>
