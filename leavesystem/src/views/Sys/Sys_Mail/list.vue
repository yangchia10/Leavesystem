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
        :placeholder="$t('Common.keyword')"
        style="width: 220px;"
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
      @selection-change="handleSelectChange"
    >
      <el-table-column type="selection" width="40" />

      <!-- <el-table-column v-if= "false" width="160px" align="center" prop="MailID" :label="$t('ui.Sys.Sys_Mail.MailID')"></el-table-column>
      <el-table-column  width="160px" align="center" prop="MessageID" :label="$t('ui.Sys.Sys_Mail.MessageID')"></el-table-column>
      <el-table-column  width="160px" align="center" prop="UserID" :label="$t('ui.Sys.Sys_Mail.UserID')"></el-table-column>-->
      <el-table-column
        width="100px"
        align="center"
        prop="UserName"
        :label="$t('ui.Sys.Sys_Mail.UserName')"
      />
      <!-- <el-table-column  width="160px" align="center" prop="MessageTypeID" :label="$t('ui.Sys.Sys_Mail.MessageTypeID')"></el-table-column> -->
      <el-table-column
        width="160px"
        align="center"
        prop="MessageTypeDesc"
        :label="$t('ui.Sys.Sys_Mail.MessageTypeDesc')"
      />
      <el-table-column
        width="200px"
        align="center"
        prop="MailSubject"
        :label="$t('ui.Sys.Sys_Mail.MailSubject')"
      />
      <el-table-column
        width="300px"
        align="center"
        prop="MailBody"
        :label="$t('ui.Sys.Sys_Mail.MailBody')"
      />
      <!-- <el-table-column  width="160px" align="center" prop="SenderMail" :label="$t('ui.Sys.Sys_Mail.SenderMail')"></el-table-column> -->
      <el-table-column
        width="300px"
        align="center"
        prop="ReceiverMail"
        :label="$t('ui.Sys.Sys_Mail.ReceiverMail')"
      />
      <el-table-column
        width="160px"
        align="center"
        prop="SendTime"
        :label="$t('ui.Sys.Sys_Mail.SendTime')"
        :formatter="formatDateTime"
      />
      <el-table-column width="160px" align="center" prop="Remark" :label="$t('Common.Remark')" />
    </el-table>
    <!-- 分页组件 -->
    <pagination
      v-show="total>0"
      :total="total"
      :page.sync="listQuery.PageNumber"
      :limit.sync="listQuery.PageSize"
      @pagination="getList"
    />
  </div>
</template>

<script>
import { fetchList, add, update, batchDelete } from "@/api/Sys/Sys_Mail";
import { convertToKeyValue } from "@/utils";
import waves from "@/directive/waves"; // waves directive 特效
import Pagination from "@/components/Pagination"; // 分页
import { MessageBox } from "element-ui"; // 提示框
import { formatDate, formatDateTime } from "@/utils"; // 列表内容格式化

export default {
  name: "Sys.Sys_Mail",
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
      selectedRowsData: [],
      total: 0,
      listLoading: true,
      listQuery: {
        keyword: "", // 用户姓名 | 登录账号
        dateValue: [
          new Date(new Date().setDate(new Date().getDate() - 7)),
          new Date(new Date().setDate(new Date().getDate() + 7))
        ], //
        PageNumber: 1,
        PageSize: 10
      },
      formData: {
        // 表单数据
        MailID: "",
        MessageID: "",
        UserID: "",
        UserName: "",
        MessageTypeID: "",
        MessageTypeDesc: "",
        MailSubject: "",
        MailBody: "",
        SenderMail: "",
        ReceiverMail: "",
        SendTime: "",
        Remark: ""
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
  computed: {
    canNotUpdate() {
      var selectRows = this.selectedRowsData || [];
      return selectRows.length !== 1;
    }
  },
  created() {
    this.getList();
  },
  methods: {
    formatDateTime,
    getList() {
      // 获取数据
      this.listLoading = false;
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
        MailID: "",
        MessageID: "",
        UserID: "",
        UserName: "",
        MessageTypeID: "",
        MessageTypeDesc: "",
        MailSubject: "",
        MailBody: "",
        SenderMail: "",
        ReceiverMail: "",
        SendTime: "",
        Remark: ""
      };
    },
    handleSelectChange(selection) {
      this.selectedRowsData = selection;
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
      this.resetFormData();
      this.formData = Object.assign({}, selectRows[0]); // 对象拷贝
      this.formTitle = "更新";
      this.formMode = "Update";
      this.dialogFormVisible = true;
      this.$nextTick(() => {
        this.$refs["dataForm"].clearValidate(); // 清除校验
      });
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
            return v.MailID;
          });

          // 删除逻辑处理
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
