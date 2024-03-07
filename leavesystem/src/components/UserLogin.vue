<template>
  <div class="login-container">
    <el-card class="login-card">
      <h1 class="login-title">請假系統登入</h1>
      <el-form
        ref="loginForm"
        :model="loginForm"
        :rules="rules"
        class="login-form"
        label-position="left"
        size="large"
      >
        <el-form-item prop="username">
          <el-input
            v-model="loginForm.username"
            prefix-icon="el-icon-user"
            placeholder="用戶名(員工編號)"
          ></el-input>
        </el-form-item>
        <el-form-item prop="password">
          <el-input
            v-model="loginForm.password"
            prefix-icon="el-icon-lock"
            placeholder="密碼"
            show-password
          ></el-input>
        </el-form-item>
        <el-form-item class="login-form-button">
          <el-button type="primary" @click="handleLogin" block>登入</el-button>
          <el-button type="text" @click="goToRegister" block
            >註冊新帳號</el-button
          >
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script>
import UserLogin from "@/JS/UserLogin.js"; // 引入獨立的JS文件

export default {
  mixins: [UserLogin], // 關聯引入的JS文件
  name: "UserRegister",
};
</script>
