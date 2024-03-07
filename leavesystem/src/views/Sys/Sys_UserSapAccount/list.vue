<template>
  <div class="app-container">
    <!-- 检索以及工具条区域 -->
    <div class="filter-container">
      <el-input
        v-model="listQuery.keyword"
        :placeholder="$t('Common.keyword')"
        style="width: 300px;"
        class="filter-item"
        @keyup.enter.native="handleFilter"
      />

      <el-button
        v-waves
        class="filter-item"
        type="primary"
        icon="el-icon-search"
        @click="handleFilter"
      />
      <hr />

      <el-button
        v-waves
        class="filter-item"
        type="primary"
        icon="el-icon-plus"
        @click="handleCreate"
      >添加</el-button>

      <el-button
        v-waves
        class="filter-item"
        type="primary"
        icon="el-icon-edit"
        @click="handleUpdate"
      >编辑</el-button>

      <el-button
        v-waves
        class="filter-item"
        type="danger"
        icon="el-icon-delete"
        @click="handleDelete"
      >删除</el-button>

      <el-button
        v-waves
        :loading="downloadLoading"
        class="filter-item"
        type="primary"
        icon="el-icon-download"
      >导出</el-button>
    </div>

    <!-- 数据列表 -->
    <el-table
      ref="multipleTable"
      v-loading="listLoading"
      :data="list"
      border
      :header-cell-style="{background:'#eef1f6',color:'#606266'}"
      fit
      highlight-current-row
      style="width: 100%;"
      max-height="450"
    >
      <el-table-column type="selection" width="40" />

      <el-table-column
        v-if="false"
        align="center"
        :label="$t('ui.Sys.Sys_UserSapAccount.UserSapAccountID')"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.UserSapAccountID }}</span>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>
</el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_UserSapAccount.UserID')">
        <template slot-scope="scope">
  <span>{{ scope.row.UserID }}</span>
</template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_UserSapAccount.SapAccount')">
        <template slot-scope="scope">
  <span>{{ scope.row.SapAccount }}</span>
</template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_UserSapAccount.SapPassword')">
        <template slot-scope="scope">
  <span>{{ scope.row.SapPassword }}</span>
</template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_UserSapAccount.CUser')">
        <template slot-scope="scope">
  <span>{{ scope.row.CUser }}</span>
</template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_UserSapAccount.CTime')">
        <template slot-scope="scope">
  <span>{{ scope.row.CTime }}</span>
</template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_UserSapAccount.MUser')">
        <template slot-scope="scope">
  <span>{{ scope.row.MUser }}</span>
</template>
      </el-table-column>
      <el-table-column width="120px" align="center" :label="$t('ui.Sys.Sys_UserSapAccount.MTime')">
        <template slot-scope="scope">
  <span>{{ scope.row.MTime }}</span>
</template>
      </el-table-column>



    </el-table>
    <!-- 分页组件 -->
    <pagination
      v-show="total>0"
      :total="total"
      :page.sync="listQuery.PageNumber"
      :limit.sync="listQuery.PageSize"
      @pagination="getList"
    />

    <!-- 表单对话框 -->
    <el-dialog :title="formTitle" :visible.sync="dialogFormVisible" top="10vh">
      <el-form ref="dataForm" :rules="rules" :model="formData" label-position="left" label-width="70px">

                <el-form-item :label="$t('ui.Sys.Sys_UserSapAccount.UserID')">
          <el-input v-model="formData.UserID" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_UserSapAccount.SapAccount')">
          <el-input v-model="formData.SapAccount" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_UserSapAccount.SapPassword')">
          <el-input v-model="formData.SapPassword" autocomplete="off" />
        </el-form-item>

      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">
          {{ $t('Common.cancel') }}
        </el-button>
        <el-button type="primary" @click="formMode==='Create'?createData():updateData()">
          {{ $t('Common.confirm') }}
        </el-button>
      </div>
    </el-dialog>

  </div>
</template>

<script>
import {
  fetchList,
  add,
  update,
  batchDelete
} from "@/api/Sys/Sys_UserSapAccount";
import { convertToKeyValue } from "@/utils";
import waves from "@/directive/waves"; // waves directive 特效
import Pagination from "@/components/Pagination"; // 分页
import { MessageBox } from "element-ui"; // 提示框

export default {
  name: "Sys_UserSapAccount",
  components: { Pagination },
  directives: { waves },
  filters: {
    statusFilter(status) {
      const statusMap = {
        true: "正常",
        false: "冻结"
      };
      return statusMap[status];
    }
  },
  data() {
    return {
      list: null,
      total: 0,
      listLoading: true,
      listQuery: {
        keyword: "", // 用户姓名 | 登录账号
        PageNumber: 1,
        PageSize: 10
      },
      formData: {
        // 表单数据
        UserSapAccountID: "",
        UserID: "",
        SapAccount: "",
        SapPassword: ""
      },
      formTitle: "", // 弹窗标题
      formMode: "",
      dialogFormVisible: false, // 表单窗口是否显示标志
      rules: {
        // 表单校验逻辑
        UserName: [
          { required: true, message: "用户名必须输入", trigger: "change" }
        ]
      },
      downloadLoading: false
    };
  },
  created() {
    this.getList();
  },
  methods: {
    getList() {
      // 获取数据
      this.listLoading = true;
      fetchList(this.listQuery).then(response => {
        this.list = response.Data.items;
        this.total = response.Data.total;
        this.listLoading = false;
      });
    },
    handleFilter() {
      this.getList();
    },
    resetFormData() {
      // 重置表单数据
      this.formData = {
        UserSapAccountID: "",
        UserID: "",
        SapAccount: "",
        SapPassword: ""
      };
    },
    handleCreate() {
      this.resetFormData();
      this.formTitle = "添加";
      this.formMode = "Create";
      this.dialogFormVisible = true;
      this.$nextTick(() => {
        this.$refs["dataForm"].clearValidate(); // 清除校验
      });
    },
    createData() {
      // 添加
      add(this.formData).then(response => {
        // this.list.unshift(this.temp)
        this.dialogFormVisible = false;
        this.$notify({
          title: "成功",
          message: "操作成功",
          type: "success",
          duration: 2000
        });
        this.getList();
      });
    },
    handleUpdate() {
      var selectRows = this.$refs.multipleTable.store.states.selection;
      if (selectRows.length > 1) {
        this.$notify({
          title: "提示",
          message: "编辑时只能选中一行数据",
          type: "info",
          duration: 2000
        });
        return;
      } else if (selectRows.length == 0) {
        this.$notify({
          title: "提示",
          message: "编辑时请先选中一行数据",
          type: "info",
          duration: 2000
        });
        return;
      } else {
        this.resetFormData();
        this.formData = Object.assign({}, selectRows[0]); // 对象拷贝
        this.formTitle = "更新";
        this.formMode = "Update";
        this.dialogFormVisible = true;
        this.$nextTick(() => {
          this.$refs["dataForm"].clearValidate(); // 清除校验
        });
      }
    },
    updateData() {
      update(this.formData).then(() => {
        this.getList();
        this.dialogFormVisible = false;
        this.$notify({
          title: "成功",
          message: "操作成功",
          type: "success",
          duration: 2000
        });
      });
    },
    handleDelete() {
      var selectRows = this.$refs.multipleTable.store.states.selection;

      if (selectRows.length < 1) {
        this.$notify({
          title: "提示",
          message: "请至少选择一行数据进行删除",
          type: "info",
          duration: 2000
        });
        return;
      } else {
        console.log("batchDelete");
        this.$confirm("是否确认要删除该行数据？", "提示", {
          confirmButtonText: "确定",
          cancelButtonText: "取消",
          type: "warning"
        }).then(() => {
          var arrRowsID = selectRows.map(function(v) {
            return v.UserSapAccountID;
          });

          //删除逻辑处理
          batchDelete(arrRowsID).then(() => {
            this.$notify({
              title: "成功",
              message: "操作成功",
              type: "success",
              duration: 2000
            });
            this.getList();
          });
        });
      }
    }
  }
};
</script>

