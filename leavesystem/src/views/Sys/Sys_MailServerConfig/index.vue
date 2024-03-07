<template>
  <div class="app-container">
    <div style="width:600px">
      <el-form
        ref="dataForm"
        :rules="rules"
        :model="formData"
        label-position="left"
        label-width="120px"
      >
        <el-form-item
          :label="$t('ui.Sys.Sys_MailServerConfig.MailServerHost')"
          prop="MailServerHost"
        >
          <el-input v-model="formData.MailServerHost" autocomplete="off" />
        </el-form-item>
        <el-form-item
          :label="$t('ui.Sys.Sys_MailServerConfig.MailServerPort')"
          prop="MailServerPort"
        >
          <!-- <el-input v-model="formData.MailServerPort" autocomplete="off" /> -->
          <el-input-number v-model="formData.MailServerPort" :min="1" :max="65536" :step="2"></el-input-number>
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_MailServerConfig.SenderDisplayName')">
          <el-input v-model="formData.SenderDisplayName" autocomplete="off" />
        </el-form-item>
        <el-form-item
          :label="$t('ui.Sys.Sys_MailServerConfig.MailServerAccount')"
          prop="MailServerAccount"
        >
          <el-input v-model="formData.MailServerAccount" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('ui.Sys.Sys_MailServerConfig.MailServerPassword')">
          <el-input v-model="formData.MailServerPassword" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('Common.Remark')">
          <el-input v-model="formData.Remark" :autosize="{minRows: 3, maxRows: 5}" type="textarea" />
        </el-form-item>
      </el-form>
      <el-button
        sty
        type="primary"
        style="width:600px"
        @click="saveMailServerConfig()"
      >{{ $t('Common.save') }}</el-button>
    </div>
  </div>
</template>

<script>
import {
  fetchMailServerConfig,
  update,
} from "@/api/Sys/Sys_MailServerConfig";
import waves from "@/directive/waves"; // waves directive 特效
import { MessageBox } from "element-ui"; // 提示框
import { validateEMail } from "@/utils/form-validator"  // 表单自定义校验
import { request } from 'http';


export default {
  name: "Sys.Sys_MailServerConfig",
  directives: { waves },
  filters: {
    statusFilter (status) {
      const statusMap = {
        true: "正常",
        false: "冻结"
      };
      return statusMap[status];
    }
  },
  data () {
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
        MailServerID: "",
        MailServerHost: "",
        MailServerPort: 25,
        MailServerAccount: "",
        MailServerPassword: "",
        Remark: ""
      },
      rules: {
        // 表单校验逻辑
        MailServerHost: [{
          required: true,
          message: i18n.t("Common.ValidatorMessage.MustInput"),
          trigger: "blur"
        }],
        MailServerPort: [{
          required: true,
          message: i18n.t("Common.ValidatorMessage.MustInput"),
          trigger: "blur"
        }],
        MailServerAccount: [
          {
            validator: validateEMail,
            message: this.$t("ui.Sys.Sys_User.FormValidator.ValidateEmail"),
            trigger: "blur"
          }
        ]
      },
      downloadLoading: false
    };
  },
  created () {
    this.getMailServerConfig();
  },
  methods: {
    getMailServerConfig () {
      // 获取数据
      this.listLoading = true;
      fetchMailServerConfig().then(response => {
        // console.log('formData',response)
        this.formData = Object.assign({}, response.Data)
        this.listLoading = false;
      });
    },
    saveMailServerConfig () {

      this.$refs["dataForm"].validate(valid => {
        if (valid) {
          update(this.formData).then(() => {
            this.$notify({
              title: this.$t("Common.success"),
              message: this.$t("Common.operationSuccess"),
              type: "success",
              duration: 2000
            });
          });
        }

      })

    },

  }
};
</script>
