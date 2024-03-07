<template>
  <div>
    <h2>申請請假</h2>
    <form @submit.prevent="submitForm">
      <div class="date-time-container">
        <label for="leave-start-date">請假起始日期：</label>
        <input type="date" id="leave-start-date" v-model="leaveStartDate" />
        <input type="time" id="leave-start-time" v-model="leaveStartTime" />
      </div>

      <div class="date-time-container">
        <label for="leave-end-date">請假結束日期：</label>
        <input type="date" id="leave-end-date" v-model="leaveEndDate" />
        <input type="time" id="leave-end-time" v-model="leaveEndTime" />
      </div>

      <label for="leave-type">假別：</label>
      <select id="leave-type" v-model="leaveType">
        <option value="">請選擇假別</option>
        <option value="sick">病假</option>
        <option value="personal">事假</option>
        <option value="paid">特休</option>
        <option value="compensatory">補休</option>
        <!-- <option value="annual">年假</option> -->
        <option value="typhoon">自然災害假</option>
        <!-- 其他假別選項 -->
      </select>
      <p v-if="isCompensatorySelected && remainingOvertime !== null">
        剩餘的補休時間: {{ adjustedRemainingOvertime }}
      </p>
      <p
        v-if="leaveType === 'compensatory' && isOverRemainingTime"
        class="warning"
      >
        選擇的時間超過剩餘的補休時數
      </p>

      <label for="reason">請假原因：</label>
      <textarea id="reason" v-model="reason"></textarea>

      <button
        type="submit"
        :disabled="isOverRemainingTime"
        class="submit-button"
      >
        提交申請
      </button>
      <button class="btn-clearForm" type="button" @click="clearForm">
        清除表單
      </button>
    </form>
  </div>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";

export default {
  name: "LeaveApplicationForm",
  data() {
    return {
      leaveStartDate: "",
      leaveStartTime: "08:30", // 設置默認時間，例如 "09:00"
      leaveEndDate: "",
      leaveEndTime: "17:30", // 設置默認時間為下午
      leaveType: "",
      leaveTypeText: "", // 用於存儲選中的假別文本
      reason: "",
      reasonText: "",
      remainingOvertime: 0, // 或者 null，取決於你的邏輯

      leaveTypes: {
        sick: "病假",
        personal: "事假",
        paid: "特休",
        compensatory: "補休",
        typhoon: "自然災害假",
        // ... 其他映射
      },
    };
  },
  watch: {
    leaveType(newValue) {
      if (newValue === "compensatory") {
        // 當假別選擇為補休時
        this.fetchOvertimeAndRepairData(); // 調用方法獲取剩余的加班時間
      }
    },
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
    formattedRemainingOvertime() {
      if (this.remainingOvertime !== null) {
        const hours = Math.floor(this.remainingOvertime / 60);
        let minutes = this.remainingOvertime % 60;
        minutes = minutes >= 30 ? 30 : 0; // 大於等於30顯示30，小於30顯示0
        return `${hours} 小時 ${minutes} 分鐘`;
      }
      return null;
    },
    // 計算選擇的補休時間
    selectedRepairTimeInMinutes() {
      const startDateTime = new Date(
        this.leaveStartDate + "T" + this.leaveStartTime
      );
      const endDateTime = new Date(this.leaveEndDate + "T" + this.leaveEndTime);
      const differenceInMinutes = (endDateTime - startDateTime) / 60000;
      return differenceInMinutes > 0 ? differenceInMinutes : 0;
    },

    // 判斷是否超過剩餘補休時間
    isOverRemainingTime() {
      // 只有選擇了補休假別並且補休時數不足時才判斷是否超過
      if (this.leaveType === "compensatory") {
        return this.selectedRepairTimeInMinutes > this.remainingOvertime;
      }
      return false; // 其他情況下不做判斷
    },
    adjustedRemainingOvertime() {
      const initialRemainingMinutes = this.remainingOvertime;
      let usedMinutes = this.isCompensatorySelected
        ? this.selectedRepairTimeInMinutes
        : 0;
      // const usedMinutes = this.isCompensatorySelected
      //   ? this.selectedRepairTimeInMinutes
      //   : 0;
      const lunchBreak = 90; // 午休時間，以分鐘為單位
      // 判斷所選時間是否跨越了午休時間，並且確保只在計算中扣除一次午休時間
      if (usedMinutes > 0 && this.doesOverlapWithLunchBreak()) {
        usedMinutes -= lunchBreak;
      }
      const adjustedRemaining = initialRemainingMinutes - usedMinutes;

      // 將剩余時間轉換為小時和分鐘格式
      const hours = Math.floor(adjustedRemaining / 60);
      let minutes = adjustedRemaining % 60;
      minutes = minutes >= 30 ? 30 : 0; // 大於等於30顯示30，小於30顯示0
      return `${hours} 小時 ${minutes} 分鐘`;
    },

    // 新增計算屬性，判斷是否選擇了補休
    isCompensatorySelected() {
      return this.leaveType === "compensatory";
    },
  },
  methods: {
    async submitForm() {
      // 驗證表單字段
      if (
        !this.leaveStartDate ||
        !this.leaveStartTime ||
        !this.leaveEndDate ||
        !this.leaveEndTime ||
        !this.leaveType ||
        !this.reason
      ) {
        alert("請填寫所有地方！");
        return;
      }
      // 計算本地時間偏移（以分鐘為單位）
      const timezoneOffset = new Date().getTimezoneOffset() * 60000;

      const totalMinutes = this.calculateTotalLeaveMinutes();

      console.log("請假總時長（分鐘）:", totalMinutes);

      // 創建日期對象並應用偏移
      const startDate = new Date(
        new Date(this.leaveStartDate + "T" + this.leaveStartTime).getTime() -
          timezoneOffset
      );
      const endDate = new Date(
        new Date(this.leaveEndDate + "T" + this.leaveEndTime).getTime() -
          timezoneOffset
      );

      //將調整後的時間轉換為 ISO 格式字符串
      const formattedStartDate = startDate.toISOString().slice(0, 19);
      const formattedEndDate = endDate.toISOString().slice(0, 19);

      console.log("轉換後的開始日期:", formattedStartDate);
      console.log("轉換後的結束日期:", formattedEndDate);

      // 驗證結束日期是否在起始日期之前或同一天，同時結束時間是否在起始時間之前
      if (new Date(formattedEndDate) <= new Date(formattedStartDate)) {
        alert("日期時間不正確");
        return;
      }
      // 處理表單提交
      alert(
        `請假起始日期: ${this.leaveStartDate} ${this.leaveStartTime}\n請假結束日期: ${this.leaveEndDate} ${this.leaveEndTime}\n假別: ${this.leaveType}\n原因: ${this.reason}`
      );

      // 使用映射轉換 leaveType 的值
      const leaveApplication = {
        employee_id: this.userInfo.employee_id, // 這里應該是當前登錄用戶的ID，暫時硬編碼為1
        type: this.leaveTypes[this.leaveType], // 使用映射轉換 leaveType 的值
        start_date: this.leaveStartDate, // 已經格式化的開始日期時間
        end_date: this.leaveEndDate, // 已經格式化的結束日期時間
        leavestart_time: this.leaveStartTime,
        leaveend_time: this.leaveEndTime,
        leave_time: totalMinutes,
        reason: this.reason,
        status: "待呈核", // 初始狀態
      };
      //console.log("Sending leave application:", leaveApplication);
      try {
        const response = await axios.post(
          "http://localhost:5163/api/Leave",
          leaveApplication
        );
        console.log(response.data);
        alert("請假申請已提交！");

        // 提交補休申請後更新剩余加班時間
        if (this.leaveType === "compensatory") {
          // 在此處調用 calculateTotalLeaveMinutes 來確保使用排除午休時間後的請假總時長
          const totalLeaveMinutesWithoutLunch =
            this.calculateTotalLeaveMinutes();
          // 更新剩余補休時間
          this.remainingOvertime -= totalLeaveMinutesWithoutLunch;
          this.updateRemainingTimeAndSave(this.remainingOvertime);
        }
        // 可選: 重新從後端獲取最新的剩余加班時間
        //await this.fetchOvertimeAndRepairData();
        this.clearForm();
      } catch (error) {
        console.error("提交請假申請時出錯:", error);
        alert("提交失敗，請稍後再試。");
      }
    },

    //請假多日期 時間計算邏輯
    calculateTotalLeaveMinutes() {
      const lunchBreakStart = "12:00";
      const lunchBreakEnd = "13:30";
      let startDate = new Date(this.leaveStartDate);
      const endDate = new Date(this.leaveEndDate);
      let totalMinutes = 0;

      while (startDate <= endDate) {
        if (startDate.getDay() !== 6 && startDate.getDay() !== 0) {
          // 排除周末
          let dayStart, dayEnd;

          if (
            startDate.toDateString() ===
            new Date(this.leaveStartDate).toDateString()
          ) {
            dayStart = new Date(
              this.leaveStartDate + "T" + this.leaveStartTime
            );
          } else {
            dayStart = new Date(startDate);
            dayStart.setHours(8, 30); // 默認工作日開始時間
          }

          if (
            startDate.toDateString() ===
            new Date(this.leaveEndDate).toDateString()
          ) {
            dayEnd = new Date(this.leaveEndDate + "T" + this.leaveEndTime);
          } else {
            dayEnd = new Date(startDate);
            dayEnd.setHours(17, 30); // 默認工作日結束時間
          }

          let dayMinutes = (dayEnd - dayStart) / 60000; // 當天請假分鐘數

          // 計算午休時間
          let lunchStart = new Date(startDate);
          let lunchEnd = new Date(startDate);
          lunchStart.setHours(...lunchBreakStart.split(":"));
          lunchEnd.setHours(...lunchBreakEnd.split(":"));

          if (dayStart < lunchEnd && dayEnd > lunchStart) {
            dayMinutes -= 90; // 減去午休時間90分鐘
          }

          totalMinutes += dayMinutes;
        }
        startDate.setDate(startDate.getDate() + 1); // 移至下一天
      }

      return totalMinutes;
    },

    clearForm() {
      // 清空表單數據
      this.leaveStartDate = "";
      this.leaveEndDate = "";
      this.leaveType = "";
      this.reason = "";

      // 保留默認的開始和結束時間
      this.leaveStartTime = "08:30";
      this.leaveEndTime = "17:30";
    },
    async fetchOvertimeAndRepairData() {
      // 從 Vuex 獲取用戶名
      const username = this.userInfo.username;
      try {
        // 發送 GET 請求到後端API，獲取用戶的剩余加班時間
        const response = await axios.get(
          `http://localhost:5163/api/SearchOvertimeRepair/approvedtime/${username}`
        );

        // 假設後端正確返回數據，並且數據結構如 { remainingMinutes: number }
        if (
          response &&
          response.data &&
          response.data.remainingMinutes !== undefined
        ) {
          // 設置剩余加班時間
          this.remainingOvertime = response.data.remainingMinutes;
          console.log("剩余的加班時間（分鐘）:", this.remainingOvertime);
        } else {
          // 如果響應數據不包含期望的 remainingMinutes 字段
          console.warn("響應數據中沒有找到剩余加班時間");
          this.updateRemainingTimeAndSave(0); // 例如，設置為0
        }

        // this.updateRemainingTimeAndSave(response.data.RemainingMinutes);
      } catch (error) {
        console.error("獲取數據出錯:", error);
        // 出錯時重置剩余加班時間為0而不是null，這樣可以避免前端因為null值而可能出現的錯誤
        this.updateRemainingTimeAndSave(0); // 設置為0或根據需要處理
      }
    },
    updateRemainingTimeAndSave(remainingTime) {
      // 確保remainingTime有一個有效的值
      if (remainingTime !== undefined && remainingTime !== null) {
        this.remainingOvertime = remainingTime;
        localStorage.setItem("remainingOvertime", remainingTime.toString());
      } else {
        console.error("嘗試更新的剩余時間是undefined或null");
        // 可能需要設置一個默認值或執行其他錯誤處理邏輯
      }
    },

    // 添加一個方法來判斷所選時間是否跨越午休時間段
    doesOverlapWithLunchBreak() {
      // 這里需要添加邏輯來判斷所選請假時間是否跨越了設定的午休時間段
      // 以下為示例邏輯，請根據實際情況進行調整
      const lunchStart = 12 * 60; // 午休開始時間，換算為從午夜開始的分鐘數
      const lunchEnd = 13.5 * 60; // 午休結束時間
      const leaveStartMinutes = this.timeToMinutes(this.leaveStartTime);
      const leaveEndMinutes = this.timeToMinutes(this.leaveEndTime);

      // 判斷請假時間是否與午休時間有重疊
      return !(leaveEndMinutes <= lunchStart || leaveStartMinutes >= lunchEnd);
    },
    // 將時間字符串轉換為從午夜開始的分鐘數
    timeToMinutes(time) {
      const [hours, minutes] = time.split(":").map(Number);
      return hours * 60 + minutes;
    },
  },
  mounted() {
    console.log("LeaveApplicationForm mounted");
    this.fetchOvertimeAndRepairData(); // 獲取剩余加班時間
  },
};
</script>

<style scoped>
form {
  display: flex; /* 使用flex布局 */
  flex-direction: column; /* 子元素垂直排列 */
  align-items: stretch; /* 子元素在交叉軸上居中 */
  padding: 20px;
  background: #f8f8f8;
  border-radius: 10px;
}

label {
  display: block; /* 使標簽獨占一行 */
  margin-top: 20px; /* 在標簽上方添加外邊距 */
}

input[type="date"],
textarea {
  width: 50%; /* 輸入和文本區域占滿剩余空間 */
  padding: 10px; /* 內部填充 */
  margin-top: 5px; /* 與標簽之間的空間 */
  border: 1px solid #ccc; /* 邊框 */
  border-radius: 5px; /* 輕微的圓角 */
}

textarea {
  height: 120px; /* 設置一個初始高度 */
  width: 490px; /* 設置一個初始高度 */
}

button {
  display: block; /* 按鈕獨占一行 */
  width: 100%; /* 按鈕占滿剩余空間 */
  padding: 10px; /* 按鈕內部填充 */
  margin-top: 30px; /* 與文本區域之間的空間 */
  background-color: #3498db; /* 按鈕背景色 */
  color: white; /* 文字顏色 */
  border: none; /* 無邊框 */
  border-radius: 5px; /* 圓角 */
  cursor: pointer; /*
鼠標懸停時顯示指針 /
transition: background-color 0.3s ease; / 過渡動畫 */
}

button:hover {
  background-color: #2980b9; /* 鼠標懸停時按鈕的背景色 */
}

.btn-clearForm {
  background-color: #e74c3c;
  color: white;
}

.btn-clearForm:hover {
  background-color: #de786c; /* Change the background color on hover */
  color: #ffffff; /* Change the text color on hover */
}

.date-time-container {
  display: flex; /* 使用flex布局 */
  align-items: center; /* 子元素在交叉軸上居中 */
  margin-top: 10px; /* 在標簽上方添加外邊距 */
  align-items: baseline;
}

input[type="time"] {
  margin-left: 10px; /* 時間輸入框與日期輸入框之間的空間 */
}

select {
  width: 50%; /* 與輸入框相同的寬度 */
  padding: 10px; /* 內部填充 */
  margin-top: 5px; /* 與標簽之間的空間 */
  border: 1px solid #ccc; /* 邊框 */
  border-radius: 5px; /* 輕微的圓角 */
  margin-bottom: 20px; /* 與下一個標簽之間的空間 */
}

.warning {
  color: red; /* 設置警告文字為紅色 */
}

.submit-button:disabled {
  background-color: #ccc; /* 灰色背景 */
  color: #666; /* 深灰色文字 */
  cursor: not-allowed; /* 禁止符號光標 */
}

/* 移除禁用按鈕的懸停效果 */
.submit-button:disabled:hover {
  background-color: #ccc; /* 保持背景不變 */
}
</style>
