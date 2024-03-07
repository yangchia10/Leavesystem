<template>
  <el-form :model="PasswordInfo" :rules="rules" ref="dataForm">
    <el-form-item :label="$t('Platform.Layout.Navbar.UpdatePassword.LoginAccount')">
      <el-input v-model="PasswordInfo.LoginAccount" :disabled="true" />
    </el-form-item>
    <el-form-item
      :label="$t('Platform.Layout.Navbar.UpdatePassword.OldPassword')"
      prop="OldPassword"
    >
      <el-input v-model="PasswordInfo.OldPassword" show-password />
    </el-form-item>
    <el-form-item
      :label="$t('Platform.Layout.Navbar.UpdatePassword.NewPassword')"
      prop="NewPassword"
    >
      <el-input v-model="PasswordInfo.NewPassword" show-password />
    </el-form-item>
    <el-form-item
      :label="$t('Platform.Layout.Navbar.UpdatePassword.ConfirmNewPassword')"
      prop="ConfirmNewPassword"
    >
      <el-input v-model="PasswordInfo.ConfirmNewPassword" show-password />
    </el-form-item>

    <el-form-item>
      <el-button type="primary" @click="updatePassword">{{ $t('Common.confirm') }}</el-button>
    </el-form-item>
  </el-form>
</template>

<script>
import { modifyPassword } from "@/api/Sys/Sys_User";
import md5 from "js-md5";

export default {
  name: "updatepassword",
  props: {
    user: {
      type: Object,
      default: () => {
        return {
          name: "",
          userinfo: {},
          avatar: "",
          roles: ""
        };
      }
    }
  },
  data() {
    return {
      PasswordInfo: {
        LoginAccount: this.user.userinfo.LoginAccount,
        OldPassword: "",
        NewPassword: "",
        ConfirmNewPassword: ""
      },
      rules: {
        // 表单校验逻辑
        OldPassword: [
          {
            required: true,
            message: i18n.t("Common.ValidatorMessage.MustInput"),
            trigger: "blur"
          }
        ],
        NewPassword: [
          {
            required: true,
            min: 3,
            max: 15,
            message: this.$t("Common.ValidatorMessage.LengthRange", {
              Range: "[3-15]"
            }),
            trigger: "blur"
          }
        ],
        ConfirmNewPassword: [
          {
            required: true,
            min: 3,
            max: 15,
            message: this.$t("Common.ValidatorMessage.LengthRange", {
              Range: "[3-15]"
            }),
            trigger: "blur"
          }
        ]
      }
    };
  },
  methods: {
    updatePassword() {
      if (
        this.PasswordInfo.NewPassword !== this.PasswordInfo.ConfirmNewPassword
      ) {
        this.$message({
          title: this.$t("Common.Warning"),
          message: this.$t(
            "Platform.Layout.Navbar.UpdatePassword.NotConsistent"
          ),
          type: "warning",
          duration: 5 * 1000
        });
      } else {
        this.$refs["dataForm"].validate(valid => {
          if (valid) {
            modifyPassword({
              oldPassword: md5(this.PasswordInfo.OldPassword),
              newPassword: md5(this.PasswordInfo.NewPassword)
            }).then(() => {
              this.$notify({
                title: this.$t("Common.success"),
                message: this.$t("Common.operationSuccess"),
                type: "success",
                duration: 2000
              });
            });
          }
        });
      }
    }
  }
};
</script>
