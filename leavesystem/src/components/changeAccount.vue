<template>
  <div class="password-change-container">
    <h2>修改密碼</h2>
    <el-form @submit.prevent="changeAccount">
      <!-- 舊密碼輸入框 -->
      <el-form-item label="舊密碼" prop="oldPassword">
        <el-input
          v-model="oldPassword"
          type="password"
          show-password
          placeholder="舊密碼"
        ></el-input>
      </el-form-item>

      <!-- 新密碼輸入框 -->
      <el-form-item label="新密碼" prop="newPassword">
        <el-input
          v-model="newPassword"
          type="password"
          show-password
          placeholder="新密碼"
        ></el-input>
      </el-form-item>

      <!-- 確認新密碼輸入框 -->
      <el-form-item label="確認新密碼" prop="confirmPassword">
        <el-input
          v-model="confirmPassword"
          type="password"
          show-password
          placeholder="確認新密碼"
        ></el-input>
      </el-form-item>

      <el-button type="primary" native-type="submit">更新密碼</el-button>
    </el-form>
  </div>
</template>

<script>
import { mapState } from "vuex";
export default {
  data() {
    return {
      oldPassword: "",
      newPassword: "",
      confirmPassword: "",
      showOldPassword: false,
      showNewPassword: false,
      showConfirmPassword: false,
    };
  },
  computed: {
    ...mapState(["userInfo"]),
  },
  methods: {
    async changeAccount() {
      if (!this.userInfo || !this.userInfo.username) {
        console.error("用戶信息未加載或用戶名為空");
        alert("用戶信息未加載，請稍後重試。");
        return;
      }
      if (this.newPassword !== this.confirmPassword) {
        alert("新密碼和確認密碼不匹配。");
        return;
      }
      try {
        const response = await fetch(
          "http://localhost:5163/api/Employee/change-password",
          {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
              username: this.userInfo.username,
              oldPassword: this.oldPassword,
              newPassword: this.newPassword,
            }),
          }
        );
        const data = await response.json();
        if (data.success) {
          alert("密碼已更新");
        } else {
          alert("更新密碼失敗" + data.message);
        }
      } catch (error) {
        console.error("密碼更新失敗", error);
        alert("更新密碼時發生錯誤。請查看控制台獲取更多信息。");
      }
    },
    toggleShowOldPassword() {
      this.showOldPassword = !this.showOldPassword;
    },
    toggleShowNewPassword() {
      this.showNewPassword = !this.showNewPassword;
    },
    toggleShowConfirmPassword() {
      this.showConfirmPassword = !this.showConfirmPassword;
    },
  },
};
</script>

<style scoped>
.password-change-container {
  max-width: 600px; /* 容器最大寬度調整為600px */
  margin: 40px auto;
  padding: 40px; /* 內邊距調整為40px */
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.05), 0 6px 20px rgba(0, 0, 0, 0.1);
  border-radius: 8px; /* 輕微調整邊框圓角 */
  background-color: #fff;
  transition: all 0.3s ease; /* 添加過渡效果 */
}

h2 {
  color: #333;
  font-size: 28px; /* 標題字體大小調整 */
  margin-bottom: 25px; /* 標題與表單的間距 */
}

.el-form-item {
  margin-bottom: 25px; /* 表單項目間距調整 */
}

.el-input__inner {
  font-size: 18px; /* 輸入框字體大小 */
  height: 50px; /* 輸入框高度 */
  padding: 12px 20px; /* 輸入框內邊距 */
  border: 1px solid #dcdfe6; /* 邊框樣式調整 */
  border-radius: 4px; /* 邊框圓角 */
  background-color: #f9f9f9; /* 輕微調整背景顏色 */
}

.el-button {
  width: auto; /* 按鈕寬度自適應內容 */
  padding: 10px 30px; /* 按鈕內邊距 */
  font-size: 16px; /* 按鈕字體大小 */
  border-radius: 4px; /* 按鈕圓角 */
  margin-top: 20px; /* 與上一個元素的間距 */
}

.el-button--primary {
  background-color: #409eff; /* 主要按鈕背景顏色 */
  border-color: #409eff; /* 主要按鈕邊框顏色 */
}

.el-form-item__label {
  font-size: 20px; /* 標籤字體大小 */
  color: #606266; /* 標籤顏色 */
}
</style>
