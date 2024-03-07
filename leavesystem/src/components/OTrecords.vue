<template>
  <div class="search-overtimeRepair-status">
    <h2>加班時數查詢</h2>
    <div class="search-box">
      <label for="startDate">起始日期:</label>
      <input type="date" id="startDate" v-model="startDate" />
      <label for="endDate">終止日期:</label>
      <input type="date" id="endDate" v-model="endDate" />
      <button @click="fetchOvertimeStatus">搜索</button>
    </div>
    <table>
      <thead>
        <tr>
          <th>員工編號</th>
          <th>員工姓名</th>
          <th>加班日期</th>
          <th>是否跨夜加班</th>
          <th>起始加班時間</th>
          <th>結束加班時間</th>
          <th>加班時長</th>
          <th>加班原因</th>
          <th>狀態</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="data in overData" :key="data.ovretime_id">
          <td>{{ data.employeename }}</td>
          <td>{{ data.username }}</td>
          <td>{{ data.overDate }}</td>
          <td>{{ data.overnight }}</td>
          <td>{{ data.overstart_time }}</td>
          <td>{{ data.overend_time }}</td>
          <td>{{ data.overtimehours }}</td>
          <td class="table-cell-ellipsis">{{ data.description }}</td>
          <td>{{ data.over_status }}</td>
        </tr>
      </tbody>
    </table>
    <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>
  </div>
  <!-- 加班總時數和補休總時數 -->
  <div class="summary">
    <p>加班總時數: {{ formattedTotalOvertimeHours }}</p>
  </div>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";
export default {
  name: "OTrecords",
  data() {
    return {
      overData: [],
      totalOvertimeMinutes: 0, // 添加這個屬性來保存總加班分鐘數
      errorMessage: "", // Error message initialization
      startDate: null,
      endDate: null,
    };
  },
  mounted() {
    // 觸發一次默認的 API 請求
    this.fetchOvertimeStatus();
  },
  created() {
    this.fetchOvertimeStatus(); // 獲取批准數據
  },
  computed: {
    formattedTotalOvertimeHours() {
      const totalMinutes = this.totalOvertimeMinutes;
      const hours = Math.floor(totalMinutes / 60);
      let minutes = totalMinutes % 60;

      if (minutes < 30) {
        minutes = 0;
      } else {
        minutes = 30;
      }

      return `${hours}小時${minutes}分鐘`;
    },
    ...mapState(["userInfo"]),
  },
  methods: {
    fetchOvertimeStatus() {
      if (this.startDate && this.endDate && this.startDate > this.endDate) {
        this.errorMessage = "日期範圍錯誤：終止日不能早於起始日";
        console.log("Invalid date range: End date is earlier than Start date");
        return; // 不執行API請求
      }
      if (!this.startDate || !this.endDate) {
        console.log("Start Date or End Date not provided. Not fetching data.");
        return; // 如果未提供開始或結束日期，則不執行API請求
      }
      console.log(
        "Sending request with startDate:",
        this.startDate,
        "endDate:",
        this.endDate
      );
      // 格式化日期為 YYYY-MM-DD
      const formattedStartDate = this.startDate;
      const formattedEndDate = this.endDate;
      console.log(
        "Formatted dates:",
        "Start Date:",
        formattedStartDate,
        "End Date:",
        formattedEndDate
      );
      axios
        .get(
          `http://localhost:5163/api/SearchOvertimeRepair/overtimerecords/${this.userInfo.employee_id}?startDate=${this.startDate}&endDate=${this.endDate}`,
          {
            params: {
              start_date: formattedStartDate,
              end_date: formattedEndDate,
            },
          }
        )

        .then((response) => {
          console.log("API Response:", response);
          this.overData = response.data.details || [];

          // 篩選出狀態為“批准”的記錄
          const approvedOvertimeRecords = this.overData.filter(
            (data) => data.over_status === "批准"
          );
          // 僅對批准的記錄計算總加班時間
          this.totalOvertimeMinutes = approvedOvertimeRecords.reduce(
            (acc, curr) => acc + this.convertHoursToMinutes(curr.overtimehours),
            0
          );

          // 檢查返回的數據是否為空
          if (this.overData.length === 0) {
            this.errorMessage = "查無數據";
          } else {
            this.errorMessage = ""; // 清除之前的錯誤消息
          }
        })

        .catch((error) => {
          //this.errorMessage = "搜索時發生錯誤";
          console.error("Error:", error);
        });
    },
    convertHoursToMinutes(hoursString) {
      // 假設 hoursString 格式為 "X小時Y分鐘"
      const parts = hoursString.split("小時");
      const hours = parseInt(parts[0], 10);
      const minutes = parseInt(parts[1].replace("分鐘", ""), 10);
      return hours * 60 + minutes;
    },
    formatDate(date) {
      return date.split("/").join("-");
    },
  },
};
</script>

<style scoped>
/* 基本布局和顏色 */
.search-overtimeRepair-status {
  max-width: 1500px;
  margin: 20px auto;
  padding: 20px;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

.search-box {
  display: flex;
  margin-bottom: 20px;
  align-items: center;
}

.search-box input {
  flex-grow: 1;
  padding: 10px 15px;
  margin-right: 10px;
  border: 2px solid #007bff;
  border-radius: 4px;
  transition: border-color 0.3s ease;
}

.search-box input:focus {
  border-color: #0056b3;
  outline: none;
}

.search-box button {
  padding: 10px 20px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.2s ease, transform 0.2s ease;
}

.search-box button:hover {
  background-color: #0056b3;
  transform: translateY(-2px);
}

.loading,
.error-message,
.no-data {
  text-align: center;
  margin-top: 20px;
  font-size: 1.1em;
  color: #666;
}

table {
  width: 100%;
  border-collapse: collapse;
}

table th,
table td {
  border: 1px solid #ddd;
  padding: 12px;
  text-align: left;
}

/* 限制表格單元格內文本的顯示長度 */
.table-cell-ellipsis {
  max-width: 150px; /* 您可以根據需要調整這個寬度 */
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  cursor: pointer;
}

table th {
  background-color: #007bff;
  color: white;
}

table tbody tr {
  transition: background-color 0.3s ease;
}

table tbody tr:nth-child(even) {
  background-color: #f9f9f9;
}

table tbody tr:hover {
  background-color: #eef4ff;
}

.accordion-item {
  margin-bottom: 10px;
}

.accordion-header {
  padding: 10px;
  background-color: #007bff;
  color: white;
  border: none;
  width: 100%;
  text-align: left;
  cursor: pointer;
  transition: background-color 0.3s, transform 0.2s ease;
}

.accordion-header:hover {
  background-color: #0056b3;
  transform: translateY(-2px);
}

.accordion-content {
  padding: 10px;
  border: 1px solid #ddd;
  border-top: none;
  background-color: #f9f9f9;
}
</style>
