import { mapActions, mapState } from "vuex";
import { Fold, Expand } from "@element-plus/icons-vue";
export default {
  name: "homePage",
  components: {
    Fold,
    Expand,
  },
  data() {
    return {
      dropdowns: {
        Sys_Role: false,
        Sys_Personal: false,
        Sys_LeaveApplicationForm: false,
        isSidebarOpen: true, // 控制側邊欄是否打開
      },
      isSidebarOpen: true, // 控制側邊欄是否打開
    };
  },
  computed: {
    ...mapState(["userInfo"]), // 映射 Vuex 中的 userInfo 到組件的 computed 屬性
    userRole() {
      // 直接從 userInfo 對象中獲取 role
      return this.userInfo ? this.userInfo.role : null;
    },
    isAdmin() {
      return this.userInfo && this.userInfo.role === 1;
    },
    userName() {
      // Compute the user name for display in the sidebar header
      return this.userInfo ? this.userInfo.name : "Guest";
    },
  },

  watch: {
    // 監聽 userInfo 的變化
    userInfo: {
      immediate: true,
      handler(newValue) {
        if (newValue && newValue.role !== undefined) {
          console.log("Updated user role:", newValue.role);
        } else {
          console.log("No role information available in the userInfo");
        }
      },
    },
  },
  methods: {
    toggleSidebar() {
      this.isSidebarOpen = !this.isSidebarOpen; // 切換側邊欄狀態
    },
    ...mapActions(["fetchUserInfo"]), // 映射 Vuex actions

    toggleDropdown(dropdownName) {
      this.dropdowns[dropdownName] = !this.dropdowns[dropdownName];
    },

    async fetchAndPrintUserInfo() {
      console.log("Fetching user info");
      await this.fetchUserInfo();
      console.log("Fetched user role:", this.userRole);
      this.$nextTick(() => {
        // 確保組件已經更新
        if (this.userInfo && typeof this.userInfo.role !== "undefined") {
          console.log("Fetched user role:", this.userInfo.role);
        } else {
          console.log("No role information available in the userInfo");
        }
      });
    },
    async logoutAndRedirect() {
      console.log("Logout button clicked");
      try {
        await this.logout(); // 等待登出操作完成
        this.$router.push("/login"); // 登出成功後重定向到登錄頁面
      } catch (error) {
        console.error("Logout and redirect failed", error);
        // 可以在這里處理錯誤，比如顯示一個錯誤消息
      }
    },
    async logout() {
      try {
        console.log("Attempting to logout");
        const response = await fetch("http://localhost:5163/api/Login/logout", {
          method: "POST",
          credentials: "include", // 確保發送 cookie
          headers: {
            "Content-Type": "application/json",
          },
        });
        console.log("Logout response", response);

        if (!response.ok) {
          throw new Error("Failed to logout");
        }
      } catch (error) {
        console.error("Logout error", error);
      }
    },
  },
  created() {
    if (!this.userInfo || this.userInfo.role === null) {
      // 如果 userInfo 不存在或 role 為 null，則嘗試重新獲取
      this.fetchAndPrintUserInfo();
    }
  },
};
