export default {
  name: "UserRegister",
  data() {
    return {
      registerForm: {
        EmployeeName: "",
        username: "",
        password: "",
        email: "",
        phone: "",
      },
      rules: {
        EmployeeName: [
          { required: true, message: "請輸入姓名", trigger: "blur" },
        ],
        username: [
          { required: true, message: "請輸入原工編號", trigger: "blur" },
          {
            pattern: /^\d+$/,
            message: "帳號(員工編號)只能是數字",
            trigger: "blur",
          },
        ],
        password: [{ required: true, message: "請輸入密碼", trigger: "blur" }],
        email: [
          { required: true, message: "請輸入信箱", trigger: "blur" },
          {
            type: "email",
            message: "信箱格式不正確",
            trigger: ["blur", "change"],
          },
        ],
        phone: [
          { required: true, message: "請輸入手機", trigger: "blur" },
          {
            pattern: /^\d{10}$/,
            message: "手機號碼必須是10位數字",
            trigger: "blur",
          },
        ],
        // 你可以根據需要添加更多的驗證規則
      },
    };
  },
  methods: {
    async handleRegister() {
      // 驗證表單
      this.$refs.registerForm.validate(async (valid) => {
        if (valid) {
          console.log("要發送的數據:", this.registerForm);
          try {
            alert(`${this.registerForm.EmployeeName} 註冊成功`);
            const response = await fetch("http://localhost:5163/api/Register", {
              method: "POST",
              headers: {
                "Content-Type": "application/json",
              },
              body: JSON.stringify(this.registerForm),
            });

            if (response.ok) {
              console.log("註冊成功");
              this.$router.push({ name: "UserLogin" });
              // 這里可以添加一些注冊成功後的操作，比如跳轉到登錄頁面
            } else {
              console.log("註冊失敗");
              // 這里可以添加一些注冊失敗後的操作
            }
          } catch (error) {
            console.error("請求錯誤", error);
          }
        } else {
          console.log("表單驗證失敗");
        }
      });
    },
  },
};
