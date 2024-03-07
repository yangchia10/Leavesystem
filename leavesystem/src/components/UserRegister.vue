<template>
  <div class="register-container">
    <el-card class="register-card">
      <h1 class="register-title">用戶註冊</h1>
      <el-form
        ref="registerForm"
        :model="registerForm"
        :rules="rules"
        class="register-form"
        label-position="left"
        size="large"
      >
        <el-form-item prop="EmployeeName">
          <el-input
            v-model="registerForm.EmployeeName"
            prefix-icon="el-icon-user"
            placeholder="姓名"
          ></el-input>
        </el-form-item>
        <el-form-item prop="username">
          <el-input
            v-model="registerForm.username"
            prefix-icon="el-icon-user"
            placeholder="帳號(員工編號)"
          ></el-input>
        </el-form-item>
        <el-form-item prop="password">
          <el-input
            v-model="registerForm.password"
            prefix-icon="el-icon-lock"
            placeholder="密碼"
            show-password
          ></el-input>
        </el-form-item>
        <el-form-item prop="email">
          <el-input
            v-model="registerForm.email"
            prefix-icon="el-icon-message"
            placeholder="信箱"
          ></el-input>
        </el-form-item>
        <el-form-item prop="phone">
          <el-input
            v-model="registerForm.phone"
            prefix-icon="el-icon-phone"
            placeholder="手機"
          ></el-input>
        </el-form-item>
        <el-form-item class="register-form-button">
          <el-button type="primary" @click="handleRegister" block
            >註冊</el-button
          >
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script>
import UserRegister from "@/JS/UserRegister.js"; // 引入獨立的JS文件

export default {
  mixins: [UserRegister], // 關聯引入的JS文件
  name: "UserRegister",
};
</script>
