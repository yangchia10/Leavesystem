<template>
  <div class="role-table">
    <table>
      <thead>
        <tr>
          <th>員工姓名</th>
          <th>員工編號</th>
          <th>員工密碼</th>
          <th>電子郵件</th>
          <th>手機電話</th>
          <th>權限</th>
          <th>操作</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="employee in employees" :key="employee.employee_id">
          <td>{{ employee.employeename }}</td>
          <td>{{ employee.username }}</td>
          <td>{{ employee.password }}</td>
          <td>{{ employee.email }}</td>
          <td>{{ employee.phone }}</td>
          <td>{{ employee.role }}</td>
          <td>
            <button class="btn btn-edit" @click="editRole(employee)">
              編輯文檔
            </button>
            <button class="btn btn-delete" @click="confirmDelete(employee)">
              刪除
            </button>
          </td>
        </tr>
      </tbody>
    </table>
    <!-- 模態窗口 -->
    <div v-if="showModal" class="modal">
      <div class="modal-content">
        <span class="close" @click="closeModal">&times;</span>
        <form @submit.prevent="submitEdit">
          <label>員工姓名:</label>
          <input v-model="editEmployee.employeename" type="text" />
          <label>員工編號:</label>
          <input v-model="editEmployee.username" type="text" />
          <label>員工密碼:</label>
          <input v-model="editEmployee.password" type="text" />
          <label>電子郵件:</label>
          <input v-model="editEmployee.email" type="text" />
          <label>手機電話:</label>
          <input v-model="editEmployee.phone" type="text" />
          <label>權限:</label>
          <input v-model="editEmployee.role" type="text" />
          <!-- 其他字段 -->
          <button type="submit">確認更新</button>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";
export default {
  name: "RoleTable",
  data() {
    return {
      employees: [], // 員工數據
      showModal: false,
      editEmployee: {},
      roles: [], // 角色數據
    };
  },
  created() {
    this.fetchEmployees();
    this.fetchRoles(); // 獲取角色數據
  },
  methods: {
    fetchEmployees() {
      axios
        .get("http://localhost:5163/api/RolePermissions")
        .then((response) => {
          this.employees = response.data;
        })
        .catch((error) => {
          console.error("Error fetching employees:", error);
        });
    },
    fetchRoles() {
      axios
        .get("http://localhost:5163/api/RolePermissions/Roles")
        .then((response) => {
          this.roles = response.data;
        })
        .catch((error) => {
          console.error("Error fetching roles:", error);
        });
    },
    editRole(employee) {
      this.editEmployee = { ...employee };
      this.showModal = true;
    },
    closeModal() {
      this.showModal = false;
    },
    submitEdit() {
      if (window.confirm("是否確認更新？")) {
        axios
          .put(
            `http://localhost:5163/api/RolePermissions/${this.editEmployee.employee_id}`,
            this.editEmployee
          )
          .then(() => {
            const index = this.employees.findIndex(
              (emp) => emp.employee_id === this.editEmployee.employee_id
            );
            if (index !== -1) {
              this.employees[index] = { ...this.editEmployee };
            }
            this.closeModal();
          })
          .catch((error) => {
            console.error("Error updating employee:", error);
          });
      }
    },

    confirmDelete(employee) {
      // 彈出確認對話框
      if (window.confirm("確認刪除該角色嗎？")) {
        // 用戶確認刪除
        this.deleteRole(employee);
      }
    },
    deleteRole(employee) {
      // 彈出確認對話框

      // 用戶確認刪除
      axios
        .delete(
          `http://localhost:5163/api/RolePermissions/${employee.employee_id}`
        )
        .then(() => {
          // 從前端的列表中移除該員工
          this.employees = this.employees.filter(
            (emp) => emp.employee_id !== employee.employee_id
          );
          // 可能還需要重新獲取角色數據
          this.fetchRoles();
        })
        .catch((error) => {
          console.error("Error deleting employee:", error);
        });
    },
  },
};
</script>

<style scoped>
/* 根容器樣式 */
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

/* 表頭樣式 */
.role-table thead tr {
  background-color: #007bff;
  color: #ffffff;
  text-align: left;
  font-weight: bold;
}

/* 單元格樣式 */
.role-table th,
.role-table td {
  padding: 12px 15px;
  border-bottom: 1px solid #e0e0e0;
}

/* 表體行樣式 */
.role-table tbody tr {
  background-color: #f9f9f9;
  transition: background-color 0.3s ease;
}

.role-table tbody tr:hover {
  background-color: #f1f1f1;
}

/* 按鈕樣式 */
.btn {
  padding: 10px 15px;
  margin-right: 10px;
  border: none;
  border-radius: 5px;
  font-size: 0.9em;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.2s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.btn-edit {
  background-color: #ffbb2b;
  color: #ffffff;
}

.btn-edit:hover {
  background-color: #ffcc40;
}

.btn-delete {
  background-color: #ff4b2b;
  color: white;
}

.btn-delete:hover {
  background-color: #ff6240;
}

/* 模態窗口樣式 */
.modal {
  display: block;
  position: fixed;
  z-index: 1000;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  overflow: auto;
  background-color: rgba(0, 0, 0, 0.6);
}

.modal-content {
  position: relative;
  background-color: #fefefe;
  margin: 10% auto;
  padding: 20px;
  border-radius: 10px;
  width: 40%;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
}

.close {
  color: #aaaaaa;
  position: absolute;
  top: 20px;
  right: 20px;
  font-size: 24px;
  font-weight: bold;
  cursor: pointer;
}

.close:hover {
  color: #000;
}

input[type="text"],
select {
  width: 100%;
  padding: 10px;
  margin: 8px 0;
  display: inline-block;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-sizing: border-box;
}

button[type="submit"] {
  width: 100%;
  background-color: #4caf50;
  color: white;
  padding: 14px 20px;
  margin: 8px 0;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button[type="submit"]:hover {
  background-color: #45a049;
}

@media (max-width: 768px) {
  .modal-content {
    width: 80%;
  }
}
</style>
