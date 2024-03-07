<template>
  <div class="role-table">
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
          <th>狀態</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="data in approveData" :key="data.leave_id">
          <td>{{ data.username }}</td>
          <td>{{ data.employeename }}</td>
          <td>{{ data.type }}</td>
          <td>{{ data.reason }}</td>
          <td>{{ data.startDate }}</td>
          <td>{{ data.endDate }}</td>
          <td>{{ data.leavestart_time }}</td>
          <td>{{ data.leaveend_time }}</td>
          <td>{{ data.status }}</td>
        </tr>
      </tbody>
    </table>
    <p v-if="approveData.length === 0" class="no-data">查無資料</p>
  </div>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";
export default {
  name: "RoleTable",
  data() {
    return {
      approveData: [], // 存儲批準數據
    };
  },
  created() {
    this.fetchApproveData(); // 獲取批准數據
  },
  computed: {
    ...mapState(["userInfo"]),
  },
  methods: {
    fetchApproveData() {
      if (this.userInfo && this.userInfo.employee_id)
        console.log(
          "Fetching data for employee_id:",
          this.userInfo.employee_id
        );
      // 獲取當前登錄用戶的ID
      axios
        .get(
          `http://localhost:5163/api/EmployeeLeave/pendingApprovals/${this.userInfo.employee_id}`
        )
        .then((response) => {
          this.approveData = response.data;
        })
        .catch((error) => {
          console.error("Error fetching approve data:", error);
        });
    },
  },
};
</script>
<style scoped>
.role-table {
  width: 100%;
  max-width: 1000px;
  margin: 40px auto;
  border-collapse: separate;
  border-spacing: 0 10px;
  font-size: 0.9em;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
  background-color: #ffffff;
  border-radius: 8px;
  overflow: hidden;
}

.role-table thead tr {
  background-color: #007bff;
  color: #ffffff;
  text-align: left;
  font-weight: bold;
}

.role-table th,
.role-table td {
  padding: 12px 15px;
  border-bottom: 1px solid #e0e0e0;
}

.role-table tbody tr {
  background-color: #f9f9f9;
  transition: background-color 0.3s ease;
}

.role-table tbody tr:hover {
  background-color: #f1f1f1;
}

.btn {
  padding: 8px 12px;
  margin-right: 8px;
  border: none;
  border-radius: 4px;
  font-size: 0.9em;
  cursor: pointer;
  transition: background-color 0.2s ease;
}

.btn-confirm {
  background-color: #28a745;
  color: white;
}

.btn-confirm:hover {
  background-color: #218838;
}

.btn-reject {
  background-color: #dc3545;
  color: white;
}

.btn-reject:hover {
  background-color: #c82333;
}

.no-data {
  text-align: center;
  color: #666;
  margin-top: 20px;
}

@media (max-width: 768px) {
  .role-table {
    width: 90%;
    margin: 20px auto;
  }
}
</style>
