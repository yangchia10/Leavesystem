<template>
  <div class="search-overtimeRepair-status">
    <h2>查看加班補修狀況</h2>
    <div class="search-box">
      <input
        type="text"
        v-model="searchQuery"
        placeholder="輸入員工姓名或編號"
        @keyup.enter="fetchOvertimeStatus"
      />
      <button @click="fetchOvertimeStatus">搜索</button>
    </div>

    <div v-if="loading" class="loading">加載中...</div>

    <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>

    <!-- 折疊面板 -->
    <div class="accordion">
      <div class="accordion-item">
        <button
          class="accordion-header"
          @click="
            toggleAccordion('overtime')
            //fetchOvertimeStatus('overtime');
          "
        >
          加班時數詳情
        </button>
        <div class="accordion-content" v-show="accordionStatus.overtime">
          <!-- 加班數據表格 -->
          <div v-if="overData.length > 0" class="leave-status">
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
                <tr v-for="(data, index) in overData" :key="index">
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
          </div>
        </div>
      </div>

      <div class="accordion-item">
        <button
          class="accordion-header"
          @click="
            toggleAccordion('repair')
            //fetchOvertimeStatus('repair');
          "
        >
          補休詳情
        </button>
        <div class="accordion-content" v-show="accordionStatus.repair">
          <!-- 補休數據表格 -->
          <div v-if="leaveData.length > 0" class="leave-status">
            <table>
              <thead>
                <tr>
                  <th>員工姓名</th>
                  <th>員工編號</th>
                  <th>假別</th>
                  <th>請假原因</th>
                  <th>請假起始日</th>
                  <th>請假終止日</th>
                  <th>起始時間</th>
                  <th>結束時間</th>
                  <th>補休時長</th>
                  <th>狀態</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(data, index) in leaveData" :key="index">
                  <td>{{ data.username }}</td>
                  <td>{{ data.employeename }}</td>
                  <td>{{ data.type }}</td>
                  <td>{{ data.reason }}</td>
                  <td>{{ data.startDate }}</td>
                  <td>{{ data.endDate }}</td>
                  <td>{{ data.leavestart_time }}</td>
                  <td>{{ data.leaveend_time }}</td>
                  <td>{{ formatTime(data.leave_time) }}</td>
                  <td>{{ data.status }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <div
      v-if="!loading && overData.length === 0 && leaveData.length === 0"
      class="no-data"
    >
      查無數據
    </div>
  </div>
  <!-- 加班總時數和補休總時數 -->
  <div class="summary">
    <p>加班總時數: {{ formattedTotalOvertimeHours }}</p>
    <p>已補休總時數: {{ formattedTotalCompensationHours }}</p>
    <p :class="{ 'text-negative': remainingHours.isNegative }">
      剩餘時數: {{ remainingHours.formattedTime }}
    </p>
  </div>
</template>

<script>
import axios from "axios";

// 將 timeStringToMinutes 函數定義在組件外部
function timeStringToMinutes(timeString) {
  const parts = timeString.split("小時");
  const hours = parseInt(parts[0], 10);
  const minutes =
    parts.length > 1 ? parseInt(parts[1].replace("分鐘", ""), 10) : 0;
  return hours * 60 + minutes;
}
export default {
  name: "SearchLeaveStatus",
  data() {
    return {
      searchQuery: "",
      overData: [],
      leaveData: [],
      loading: false,
      errorMessage: "",

      accordionStatus: {
        overtime: false,
        repair: false,
      },
    };
  },
  computed: {
    totalOvertimeMinutes() {
      return this.overData
        .filter((data) => data.over_status === "批准")
        .reduce(
          (total, curr) =>
            total + timeStringToMinutes(String(curr.overtimehours)),
          0
        );
    },
    totalCompensationMinutes() {
      return this.leaveData
        .filter((data) => data.status === "批准")
        .reduce((total, curr) => total + parseInt(curr.leave_time, 10), 0);
    },

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
    formattedTotalCompensationHours() {
      const totalMinutes = this.totalCompensationMinutes;
      const hours = Math.floor(totalMinutes / 60);
      let minutes = totalMinutes % 60;
      if (minutes < 30) {
        minutes = 0;
      } else {
        minutes = 30;
      }
      return `${hours}小時${minutes}分鐘`;
    },
    remainingHours() {
      // 使用已經定義好的方法將格式化的時間轉換為分鐘
      const overtimeMinutes = this.formattedTimeToMinutes(
        this.formattedTotalOvertimeHours
      );
      const compensationMinutes = this.formattedTimeToMinutes(
        this.formattedTotalCompensationHours
      );

      // 計算剩余分鐘數
      let remainingMinutes = overtimeMinutes - compensationMinutes;

      // 對於負數剩余時間的特殊處理
      if (remainingMinutes < 0) {
        // 計算小時和分鐘
        let hours = Math.floor(Math.abs(remainingMinutes) / 60);
        let minutes = Math.abs(remainingMinutes % 60);

        // 對分鐘數進行四舍五入處理
        minutes = minutes >= 30 ? 30 : 0;

        // 格式化剩余時間
        const sign = "-";
        return {
          formattedTime: `${sign}${hours}小時${minutes}分鐘`,
          isNegative: true,
        };
      } else {
        // 正數剩余時間的處理
        let hours = Math.floor(remainingMinutes / 60);
        let minutes = remainingMinutes % 60;
        minutes = minutes >= 30 ? 30 : 0;

        return {
          formattedTime: `${hours}小時${minutes}分鐘`,
          isNegative: false,
        };
      }
    },
  },
  methods: {
    formatTime(minutes) {
      const hours = Math.floor(minutes / 60);
      const mins = minutes % 60;
      return `${hours}小時${mins}分鐘`;
    },
    formattedTimeToMinutes(timeString) {
      const parts = timeString.split("小時");
      const hours = parseInt(parts[0], 10);
      const minutes =
        parts.length > 1 ? parseInt(parts[1].replace("分鐘", ""), 10) : 0;
      return hours * 60 + minutes;
    },
    fetchOvertimeStatus() {
      if (!this.searchQuery) {
        this.errorMessage = "請輸入搜索內容";
        return;
      }
      this.loading = true;
      this.errorMessage = "";
      this.overData = []; // 清空原有加班數據，確保每次搜索都是新的結果
      this.leaveData = []; // 清空原有補休數據

      // 準備加班和補休的 API 請求
      const overDataRequest = axios.get(
        `http://localhost:5163/api/SearchOvertimeRepair/Overtime?query=${this.searchQuery}`
      );
      const leaveDataRequest = axios.get(
        `http://localhost:5163/api/SearchOvertimeRepair/Repair?query=${this.searchQuery}`
      );

      Promise.all([
        overDataRequest.catch((error) => {
          return { data: null, error };
        }),
        leaveDataRequest.catch((error) => {
          return { data: null, error };
        }),
      ])
        .then((responses) => {
          const [overResponse, leaveResponse] = responses;

          // 處理加班數據響應
          if (overResponse && overResponse.data && !overResponse.error) {
            this.overData = overResponse.data.overtimeRepair;
            this.accordionStatus.overtime = this.overData.length > 0;
          } else {
            this.overData = []; // 確保數據為空
            // 可以設置一個特定的消息表示加班數據為空
          }

          // 處理補休數據響應
          if (leaveResponse && leaveResponse.data && !leaveResponse.error) {
            this.leaveData = leaveResponse.data.leaveData;
            this.accordionStatus.repair = this.leaveData.length > 0;
          } else {
            this.leaveData = []; // 確保數據為空
            // 可以設置一個特定的消息表示補休數據為空
          }

          if (this.overData.length === 0 && this.leaveData.length === 0) {
            this.errorMessage = "未找到相關數據";
          } else {
            // 當搜索成功後，根據數據存在情況展開對應的折疊面板
            this.accordionStatus.overtime = this.overData.length > 0;
            this.accordionStatus.repair = this.leaveData.length > 0;
          }
        })
        .catch((error) => {
          // 這里的catch理論上不會被觸發，因為每個請求的錯誤都在Promise.all中被捕獲處理了
          this.errorMessage = "搜索時發生未知錯誤";
          console.error("Error:", error);
        })
        .finally(() => {
          this.loading = false;
        });
    },

    toggleAccordion(panel) {
      this.accordionStatus[panel] = !this.accordionStatus[panel];
    },
  },
  mounted() {
    // 觸發一次默認的 API 請求
    this.fetchOvertimeStatus();
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

.text-negative {
  color: red;
  font-weight: bold;
}
</style>
