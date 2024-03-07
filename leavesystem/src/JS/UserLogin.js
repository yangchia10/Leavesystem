import { mapActions } from "vuex";

export default {
  name: "UserLogin",
  data() {
    return {
      loginForm: {
        username: "",
        password: "",
      },
      rules: {
        username: [
          { required: true, message: "請輸入用戶名", trigger: "blur" },
        ],
        password: [{ required: true, message: "請輸入密碼", trigger: "blur" }],
      },
    };
  },
  methods: {
    ...mapActions(["fetchUserInfo"]),

    async handleLogin() {
      this.$refs.loginForm.validate(async (valid) => {
        if (valid) {
          const response = await fetch("http://localhost:5163/api/Login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(this.loginForm),
            credentials: "include", // 確保發送Cookie
          });

          const data = await response.json();

          if (response.ok) {
            //alert(`${this.loginForm.username} 登入成功`);
            console.log("登入成功", data);

            // 登錄成功後，立即獲取用戶信息
            await this.fetchUserInfo();
            this.$router.push({ name: "homePage" });
          } else {
            alert(`${this.loginForm.username} 登入失敗 ${data.message}`);
            console.log("登入失敗", data.message);
          }
        } else {
          console.log("登入表單驗證失敗!");
        }
      });
    },

    goToRegister() {
      this.$router.push({ name: "UserRegister" });
    },
  },
};
