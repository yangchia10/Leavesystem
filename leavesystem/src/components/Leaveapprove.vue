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
          <th>操作</th>
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
          <td>
            <button
              v-if="data.status === '待呈核'"
              class="btn btn-confirm"
              @click="confirmLeave(data, 'approve')"
            >
              批准
            </button>
            <button
              v-if="data.status === '待呈核'"
              class="btn btn-reject"
              @click="confirmLeave(data, 'reject')"
            >
              駁回
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import axios from "axios";
export default {
  name: "RoleTable",
  data() {
    return {
      approveData: [], // 存儲批准數據
    };
  },
  created() {
    this.fetchApproveData(); // 獲取批准數據
  },
  methods: {
    fetchApproveData() {
      axios
        .get("http://localhost:5163/api/EmployeeLeave/pendingApprovals")
        .then((response) => {
          this.approveData = response.data;
        })
        .catch((error) => {
          console.error("Error fetching approve data:", error);
        });
    },
    confirmLeave(data, action) {
      const message = action === "approve" ? "確定批准嗎？" : "確定駁回嗎？";
      if (window.confirm(message)) {
        const url = `http://localhost:5163/api/EmployeeLeave/${action}Leave/${data.leaveId}`;
        axios
          .post(url)
          .then(() => {
            alert(`請假紀錄已${action === "approve" ? "批准" : "駁回"}。`);
            this.fetchApproveData(); //在這裡重新調用fetchApproveData來更新資料
          })
          .catch((error) => {
            console.error(
              `${action === "approve" ? "批准" : "駁回"}操作失敗:`,
              error
            );
            alert(
              `${action === "approve" ? "批准" : "駁回"}操作失敗，請稍後再試。`
            );
          });
      }
    },
  },
};
</script>
