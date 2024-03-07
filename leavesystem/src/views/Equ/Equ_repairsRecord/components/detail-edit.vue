/* eslint-disable no-empty */
<template>
  <el-dialog v-el-drag-dialog :visible.sync="visible" title="編輯" width="80%">
    <el-form ref="form" :model="form" :rules="rules" label-width="30%" size="small">
      <el-row>
        <el-col :span="8">
          <el-form-item label="大類" prop="ItmsGrpCod">
            <el-input v-model="form.ItmsGrpCod" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="項目號碼" prop="ItemCode">
            <el-input v-model="form.ItemCode" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="項目名稱" prop="ItemName">
            <el-input v-model="form.ItemName" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="數量" prop="Quantity">
            <el-input v-model="form.Quantity" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="單位" prop="UomCode">
            <el-input v-model="form.UomCode" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="瑕疵原因(初判)" prop="Cause1">
            <el-input v-model="form.Cause1" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="固定資產編號" prop="ItemCode">
            <el-input v-model="form.ItemCode" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item v-if="checkPermission(['Equ.Permission.Money'])" label="總部提報維修原因" prop="Cause2">
            <el-input v-model="form.Cause2" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item v-if="checkPermission(['Equ.Permission.Money'])" label="外部修繕/內部維修" prop="Deal">
            <el-input v-model="form.Deal" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item v-if="checkPermission(['Equ.Permission.Money'])" label="最終處理結果" prop="Final">
            <el-input v-model="form.Final" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item v-if="checkPermission(['Equ.Permission.Money'])" label="維修內容描述" prop="Remark">
            <el-input v-model="form.Remark" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <!--<el-col :span="8">
          <el-form-item label="工序" prop="ProcedureCode">
            <el-select v-model="form.ProcedureCode" class="filter-item-input" placeholder="工序" clearable @change="changeProcedureCode">
              <el-option v-for="item in dicts.procedures" :key="item.ProcessCode" :label="item.ProcessName" :value="item.ProcessCode" />
            </el-select>
          </el-form-item>
        </el-col>-->
        <!-- <el-col :span="8">
          <el-form-item label="是否需要報工" prop="IsFeedBack">
            <el-switch v-model="form.IsFeedBack" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="是否需要質檢" prop="IsQC">
            <el-switch v-model="form.IsQC" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="是否尾工序" prop="IsLast">
            <el-switch v-model="form.IsLast" />
          </el-form-item>
        </el-col> -->
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
import { UpdateDetail as Edit, InsertDetailEntity as Add } from '@/api/MD/MD_ProcessRoute'
// import { fetchListByWorkshopCode as fetchProcessList } from '@/api/MD/MD_Process'
import permission from '@/directive/permission/index.js' // 权限判断指令
import checkPermission from '@/utils/permission' // 权限判断函数

export default {
  directives: {
    permission
  },
  mixins: [formSubmit],
  data() {
    return {
      // 開關
      visible: false,
      form: {
        // RowUid: '',
        SortNo: '', // 順序
        RouteCode: '',
        RouteName: '',
        IsFeedBack: true, // 是否需要報工
        IsQC: true, // 是否需要質檢
        IsLast: false, // 是否尾工序
        Remark: ''// 備註
      },
      dicts: {
        metalist: [], // 元資料（車間）
        procedures: [] // 工序列表
      },
      types: [{ type: 1, func: Add }, { type: 2, func: Edit }],
      mainData: {}
      // Edit // 編輯接口
    }
  },
  mounted() {
    // location.replace('https://www.google.com.tw/?hl=zh_TW')
  },
  methods: {
    checkPermission,
    /**
     * 重寫-彈窗打開前
     */
    openBefore(props) {
      const { data, main } = props
      // 工段字典加載
      /* this.dicts.sections = this._getParentList(main.WorkShopCode) */
      if (data) {
        // 工序字典加載

        /* // 下游工序字典加載
        this.dicts.procedure2 = this._getSublevelList(data.NextSectionCode) */
      // eslint-disable-next-line no-empty
      } else {}
      /* this.dicts.procedures = fetchProcessList() */
      this.mainData = main
    },
    /**
     * 重寫-彈窗打開完成
     */
    openLater() {
      const { mainData } = this
      console.log(mainData)
      this.form.RouteCode = mainData.RouteCode
      this.form.RouteName = mainData.RouteName
      this.form.Headid = mainData.Headid
      // this.form.ItemCode = mainData.ItemCode
      // this.form.ItemName = mainData.ItemName

      // 工序元資料

      // fetchProcessList({ WorkshopCode: this.mainData.WorkshopCode }).then(res => {
      //  this.dicts.procedures = res.Data
      // })
    },
    /**
     * 改變工序
     */
    changeProcedureCode(e) {
      const searchItem = this.dicts.procedures.find(itm => itm.ProcessCode === e)
      this.form.ProcedureName = searchItem ? searchItem.ProcessName : ''
    }
  }
}
</script>

<style lang="scss" scoped>
.el-form {
  margin-top: 10px;
}
.filter-item-input {
  width:100%;
}
</style>
