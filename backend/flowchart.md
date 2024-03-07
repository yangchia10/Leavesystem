# ASP.NET Core Web API 流程圖

## 啟動程序
- `Program.cs`
  - 設置日誌記錄、資料庫上下文、CORS策略、身份認證
  - 初始化Swagger
  - 配置HTTP請求管道

## 數據模型 (`Models`)
- `Employee`
- `LoginModel`
- `LeaveApplication`
- `OvertimeModel`
- `PasswordChangeModel`
- `TokenRequestModel`

## 數據上下文
- `ApplicationDbContext`：管理數據庫的實體模型

## API 控制器
### 用戶管理
- `LoginController`
  - 登錄、登出、刷新令牌
- `RegisterController`
  - 新員工註冊、測試資料庫連接

### 密碼管理
- `ChangePassword.cs`
  - 變更密碼

### 員工請假管理
- `EmployeeLeaveController`
  - 查看待呈核請假、批准/駁回請假

### 加班管理
- `EmployeeOvertimeController`
  - 查看待呈核加班、批准/駁回加班

### 請假申請管理
- `LeavesController`
  - 提交請假申請、查看請假申請

### 角色與權限管理
- `RolePermissionsController`
  - 員工列表、角色管理

### 搜索功能
- `SearchLeaveStatus`
- `SearchOvertimeRepair`

## 回應模型
- `ApiResponse`


# Vue 應用流程圖

## 入口點
- `main.js`：應用啟動點，加載Vue插件、組件及路由。

## 根組件
- `App.vue`：整個應用的根組件，設定全局布局和樣式。

## 路由配置
- `router.js`：定義路由規則，控制不同URL對應的頁面組件。

## Vuex狀態管理
- `index.js`：管理應用狀態，提供統一操作方法。

## 頁面組件
### 登入註冊
- `UserLogin.vue`：用戶登入頁面。
- `UserRegister.vue`：用戶註冊頁面。

### 首頁
- `homePage.vue`：登入後的首頁，根據用戶角色展示不同內容。

### 員工和管理員共用功能
- `changeAccount.vue`：帳號設置。
- `LeaveApplicationForm.vue`：請假申請表單。
- `OTrecords.vue`：加班記錄。
- `OvertimeApplication.vue`：加班申請。
- `Viewrecords.vue`：查看請假和加班記錄。

### 管理員專用功能
- `Leaveapprove.vue`：請假審批。
- `Overtimeapprove.vue`：加班審批。
- `RolePermissions.vue`：角色權限設置。
- `SearchLeaveStatus.vue`：查詢請假狀態。
- `SearchOvertimeRepair.vue`：查詢加班補修。

## 流程
1. 用戶通過`UserLogin.vue`或`UserRegister.vue`進行登入或註冊。
2. 登入後跳轉至`homePage.vue`，根據角色顯示相應功能。
3. 員工和管理員可訪問共用功能組件。
4. 管理員獨有訪問管理專用功能組件的權限。


