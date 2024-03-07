<template>
  <div class="search-leave-status">
    <h2>查看請假狀況</h2>
    <div class="search-box">
      <input
        type="text"
        v-model="searchQuery"
        placeholder="輸入員工姓名或編號"
        @keyup.enter="fetchLeaveStatus"
      />
      <button @click="fetchLeaveStatus">搜索</button>
    </div>

    <div v-if="loading" class="loading">加載中...</div>

    <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>

    <div v-if="leaveData.length > 0" class="leave-status">
      <table>
        <thead>
          <tr>
            <th>員工編號</th>
            <th>員工姓名</th>
            <th>假別</th>
            <th>起始時間</th>
            <th>結束時間</th>
            <th>請假起始日</th>
            <th>請假終止日</th>
            <th>狀態</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(data, index) in leaveData" :key="index">
            <td>{{ data.employeename }}</td>
            <td>{{ data.username }}</td>
            <td>{{ data.type }}</td>
            <td>{{ data.startDate }}</td>
            <td>{{ data.startDate }}</td>
            <td>{{ data.leavestart_time }}</td>
            <td>{{ data.leaveend_time }}</td>
            <td>{{ data.status }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-if="!loading && leaveData.length === 0" class="no-data">
      查無數據
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "SearchLeaveStatus",
  data() {
    return {
      searchQuery: "",
      leaveData: [],
      loading: false,
      errorMessage: "",
    };
  },
  methods: {
    fetchLeaveStatus() {
      if (!this.searchQuery) {
        this.errorMessage = "請輸入搜索內容";
        return;
      }
      this.loading = true;
      this.errorMessage = "";
      this.leaveData = [];
      axios
        .get(
          `http://localhost:5163/api/SearchLeaveStatus?query=${this.searchQuery}`
        )
        .then((response) => {
          this.leaveData = response.data;
          if (this.leaveData.length === 0) {
            this.errorMessage = "未找到相關數據";
          }
        })
        .catch((error) => {
          this.errorMessage = "搜索時發生錯誤";
          console.error("Error:", error);
        })
        .finally(() => {
          this.loading = false;
        });
    },
  },
};
</script>

<style scoped>
.search-leave-status {
  max-width: 1000px;
  min-width: 600px; /* 設置最小寬度 */
  min-height: 200px; /* 設置最小高度 */
  margin: auto;
  padding: 20px;
  background-color: #f8f8f8;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.search-box {
  display: flex;
  margin-bottom: 20px;
  align-items: center;
}

.search-box input {
  flex-grow: 1;
  padding: 10px;
  margin-right: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
  transition: border-color 0.3s ease;
}

.search-box input:focus {
  border-color: #007bff;
  outline: none;
}

.search-box button {
  padding: 10px 20px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.search-box button:hover {
  background-color: #0056b3;
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
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

table th,
table td {
  border: 1px solid #ddd;
  padding: 12px;
  text-align: left;
}

table th {
  background-color: #007bff;
  color: white;
  text-transform: uppercase;
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
</style>
