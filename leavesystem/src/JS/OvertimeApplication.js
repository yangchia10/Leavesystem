import axios from "axios";
import { mapState } from "vuex";
export default {
  name: "OvertimeApplicationForm",
  data() {
    return {
      overtimeDate: "",
      overtimeStartTime: "19:00", // 設置默認開始加班時間為19:00
      overtimeEndTime: "",
      reason: "",
      overtimeDuration: 0, // 初始化為0
      isOvertimeExceeded: false, // 新增數據屬性
      nextDayStartTime: new Date(`1970-01-02T08:30`),
      crossMidnight: null, // 是否跨夜加班（'yes' 或 '否'）
      isNextDayWorkTimeExceeded: false, // 新增屬性，檢查是否超過次日上班時間

      // 其他數據...
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
    // 監聽 overtimeStartTime 和 overtimeEndTime
    overtimeStartTime() {
      this.calculateOvertimeDuration();
    },
    overtimeEndTime() {
      this.calculateOvertimeDuration();
    },
  },
  methods: {
    calculateDuration() {
      if (!this.overtimeStartTime || !this.overtimeEndTime) {
        return 0;
      }

      let startTime = new Date(`1970-01-01T${this.overtimeStartTime}`);
      let endTime = new Date(`1970-01-01T${this.overtimeEndTime}`);
      const nextDayStartTime = new Date(`1970-01-02T08:30`);

      if (this.crossMidnight === "是") {
        endTime =
          endTime <= startTime
            ? new Date(endTime.setDate(endTime.getDate() + 1))
            : endTime;
      } else if (this.crossMidnight === "否" && endTime < startTime) {
        endTime = new Date(endTime.setDate(endTime.getDate() + 1));
      }

      let duration = (endTime - startTime) / 60000;
      duration = duration < 0 ? duration + 1440 : duration;

      if (this.crossMidnight === "是") {
        if (endTime > nextDayStartTime) {
          this.isNextDayWorkTimeExceeded = true;
          duration = Math.min(
            duration,
            (nextDayStartTime - startTime) / 60000 + 1440
          );
        } else {
          this.isNextDayWorkTimeExceeded = false;
        }
        return duration;
      } else {
        this.isOvertimeExceeded = duration >= 300;
        return Math.min(duration, 300);
      }
    },

    calculateOvertimeDuration() {
      this.overtimeDuration = 0;
      this.isOvertimeExceeded = false;

      const duration = this.calculateDuration();

      this.overtimeDuration = Math.max(duration, 0);
      this.isOvertimeExceeded =
        this.crossMidnight === "是" ? false : this.isOvertimeExceeded;
    },

    submitForm() {
      // 驗證表單字段
      if (
        !this.crossMidnight ||
        !this.overtimeDate ||
        !this.overtimeStartTime ||
        !this.overtimeEndTime ||
        !this.reason
      ) {
        alert("請填寫所有地方！");
        return;
      }

      const duration = this.calculateDuration();

      if (this.crossMidnight === "否" && duration > 5 * 60 + 1) {
        this.isOvertimeExceeded = true;
        return; // 不提交表單
      }

      // 處理表單提交
      alert(
        `加班日期: ${this.overtimeDate}\n加班開始時間: ${this.overtimeStartTime}\n加班結束時間: ${this.overtimeEndTime}\n加班原因: ${this.reason}`
      );

      // 使用映射轉換 leaveType 的值
      const overtimeApplication = {
        employee_id: this.userInfo.employee_id, // 這里應該是當前登錄用戶的ID，暫時硬編碼為1
        overdate: this.overtimeDate, // 加班日期
        overnight: this.crossMidnight, //是否跨夜
        overstart_time: this.overtimeStartTime, // 開始加班時間
        overend_time: this.overtimeEndTime, // 結束加班時間
        description: this.reason,
        Overtime_hours: duration, // 加班時長
        Over_status: "待呈核", // 初始狀態
      };
      console.log("Sending overtime application:", overtimeApplication);
      console.log("加班開始時間:", this.overtimeStartTime);
      console.log("加班結束時間:", this.overtimeEndTime);
      axios
        .post("http://localhost:5163/api/Overtime", overtimeApplication) // 確保URL與後端控制器的路由匹配
        .then((response) => {
          console.log(response.data);
          alert("加班申請已提交！");
          this.clearForm();

          // 添加以下行來打印加班時長
          // 計算加班時長（小時和分鐘）
          const hours = Math.floor(duration / 60);
          const minutes = duration % 60;

          // 打印加班時長
          console.log("加班時長（小時）:", hours);
          console.log("加班時長（分鐘）:", minutes);
        })
        .catch((error) => {
          console.error("提交加班申請時出錯:", error);
          alert("提交失敗，請稍後再試。");
        });
    },

    handleCrossMidnightChange() {
      this.calculateOvertimeDuration();
      // 清空時間選擇
      this.overtimeDate = "";
      this.overtimeStartTime = "19:00";
      this.overtimeEndTime = "";
    },

    clearForm() {
      // 清空表單數據
      this.overtimeDate = "";
      this.overtimeStartTime = "19:00"; // 設置默認開始加班時間為19:00
      this.overtimeEndTime = "";
      this.reason = "";
      this.overtimeDuration = 0; // 清空加班時長
      this.isOvertimeExceeded = false;
    },
  },
  mounted() {
    console.log("OvertimeApplicationForm mounted");
  },
};
