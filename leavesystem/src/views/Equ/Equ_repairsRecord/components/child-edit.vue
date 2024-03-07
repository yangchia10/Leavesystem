/* eslint-disable no-empty */
<template>
  <el-dialog v-el-drag-dialog :visible.sync="visible" title="編輯" width="80%">
    <el-form ref="form" :model="form" :rules="rules" label-width="30%" size="small">
      <el-row>
        <el-col :span="8">
          <el-form-item label="使用材料/工資" prop="ItemCode">
            <el-input v-model="form.ItemCode" class="filter-item-input" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="數量" prop="Quantity">
            <el-input v-model="form.Quantity" class="filter-item-input" />
          </el-form-item>
        </el-col>

        <el-col :span="8">
          <el-form-item label="金額" prop="Price">
            <el-input v-model="form.Price" class="filter-item-input" />
          </el-form-item>
        </el-col>

        <el-col :span="8">
          <el-form-item label="列總計" prop="Total">
            <el-input v-model="form.Total" class="filter-item-input" />
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
import { Updatechild as Edit, InsertchildEntity as Add } from '@/api/MD/MD_ProcessRoute'
// import { fetchListByWorkshopCode as fetchProcessList } from '@/api/MD/MD_Process'

export default {
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

  },
  methods: {
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
      // this.form.RouteCode = mainData.RouteCode
      // this.form.RouteName = mainData.RouteName
      this.form.Headid = mainData.Headid
      this.form.Detailid = mainData.Detailid
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
  width: 100%;
}
</style>
