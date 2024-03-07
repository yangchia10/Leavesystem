<template>
  <div id="app-homePage">
    <!-- Top bar -->
    <!-- 頂部欄 -->
    <div class="top-bar">
      <!-- 切換側邊欄按鈕 -->
      <el-icon @click="toggleSidebar" class="toggle-button">
        <Expand v-if="!isSidebarOpen" />
        <Fold v-else />
      </el-icon>
      <!-- 用戶名稱 -->
      <div class="user-name">HI! {{ userInfo.username }}</div>
    </div>
    <!-- 側邊欄 -->
    <aside class="sidebar">
      <nav class="nav">
        <div class="nav-list">
          <pre>{{ userInfo }}</pre>
          <!-- Sys_Role 條目，根據用戶角色顯示 -->
          <div class="nav-item" v-if="userInfo && userInfo.role === 1">
            <!-- 權限管理 -->
            <span class="nav-link" @click="toggleDropdown('Sys_Role')"
              >權限管理</span
            >
            <span class="dropdown-icon" @click="toggleDropdown('Sys_Role')">
              <el-icon>
                <ArrowUp v-if="dropdowns.Sys_Role" />
                <ArrowDown v-else />
              </el-icon>
            </span>
            <div v-show="dropdowns.Sys_Role" class="dropdown-menu">
              <router-link to="/home/RolePermissions" class="submenu-item"
                >角色權限</router-link
              >
              <router-link to="/home/Leaveapprove" class="submenu-item"
                >請假 呈核</router-link
              >
              <router-link to="/home/Overtimeapprove" class="submenu-item"
                >加班 呈核</router-link
              >
              <router-link to="/home/SearchLeaveStatus" class="submenu-item"
                >查看請假狀況</router-link
              >
              <router-link to="/home/SearchOvertimeRepair" class="submenu-item"
                >查看加班/補修狀況</router-link
              >
            </div>
          </div>

          <!-- aaa item -->
          <div class="nav-item">
            <span class="nav-link" @click="toggleDropdown('Sys_Personal')"
              >個人資訊</span
            >
            <span class="dropdown-icon" @click="toggleDropdown('Sys_Personal')">
              <el-icon
                ><ArrowUp v-if="dropdowns.Sys_Personal" />
                <ArrowDown v-if="!dropdowns.Sys_Personal"
              /></el-icon>
            </span>
            <div v-show="dropdowns.Sys_Personal" class="dropdown-menu">
              <!-- <a class="submenu-item">個人中心</a> -->
              <router-link to="/home/changeAccount" class="submenu-item"
                >帳密修改</router-link
              >
            </div>
          </div>
          <!-- ... other items ... -->
          <div class="nav-item">
            <span
              class="nav-link"
              @click="toggleDropdown('Sys_LeaveApplicationForm')"
              >請假申請</span
            >
            <span
              class="dropdown-icon"
              @click="toggleDropdown('Sys_LeaveApplicationForm')"
            >
              <el-icon
                ><ArrowUp v-if="dropdowns.Sys_LeaveApplicationForm" />
                <ArrowDown v-if="!dropdowns.Sys_LeaveApplicationForm"
              /></el-icon>
            </span>
            <div
              v-show="dropdowns.Sys_LeaveApplicationForm"
              class="dropdown-menu"
            >
              <router-link to="/home/LeaveApplicationForm" class="submenu-item"
                >請假/補修 申請</router-link
              >
              <router-link to="/home/Viewrecords" class="submenu-item"
                >查看紀錄</router-link
              >
            </div>
          </div>
          <!-- ... other items ... -->
          <div class="nav-item">
            <span class="nav-link" @click="toggleDropdown('Sys_OTapplication')"
              >加班</span
            >
            <span
              class="dropdown-icon"
              @click="toggleDropdown('Sys_OTapplication')"
            >
              <el-icon
                ><ArrowUp v-if="dropdowns.Sys_OTapplication" />
                <ArrowDown v-if="!dropdowns.Sys_OTapplication"
              /></el-icon>
            </span>
            <div v-show="dropdowns.Sys_OTapplication" class="dropdown-menu">
              <router-link to="/home/OvertimeApplication" class="submenu-item"
                >加班申請</router-link
              >
              <router-link to="/home/OTrecords" class="submenu-item"
                >時數記錄查詢</router-link
              >
            </div>
          </div>
        </div>
      </nav>
      <div class="logout-container">
        <button @click.stop="logoutAndRedirect" class="logout-button">
          登出
        </button>
      </div>
    </aside>
    <div class="main-content">
      <router-view></router-view>
      <!-- ... rest of the main content ... -->
    </div>
  </div>
</template>

<script>
import homePage from "@/JS/homePage.js";

export default {
  mixins: [homePage],
  name: "homePage",
};
</script>
