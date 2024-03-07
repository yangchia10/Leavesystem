import { createRouter, createWebHistory } from "vue-router";
// Adjust these imports to use relative paths
import UserLogin from "./components/UserLogin.vue";
import UserRegister from "./components/UserRegister.vue";
import homePage from "./components/homePage.vue";
import LeaveApplicationForm from "./components/LeaveApplicationForm.vue";
import changeAccount from "./components/changeAccount.vue";
import OvertimeApplication from "./components/OvertimeApplication.vue";
import OTrecords from "./components/OTrecords.vue";
import RolePermissions from "./components/RolePermissions.vue";
import Leaveapprove from "./components/Leaveapprove.vue";
import Overtimeapprove from "./components/Overtimeapprove.vue";
import Viewrecords from "./components/Viewrecords.vue";
import SearchLeaveStatus from "./components/SearchLeaveStatus.vue";
import SearchOvertimeRepair from "./components/SearchOvertimeRepair.vue";

const routes = [
  { path: "/", redirect: "/login" }, // 添加重定向規則
  { path: "/login", name: "UserLogin", component: UserLogin },
  { path: "/register", name: "UserRegister", component: UserRegister },

  {
    path: "/home",
    name: "homePage",
    component: homePage,
    children: [
      {
        path: "LeaveApplicationForm",
        name: "LeaveApplicationForm",
        component: LeaveApplicationForm,
      },
      {
        path: "changeAccount",
        name: "changeAccount",
        component: changeAccount,
      },
      {
        path: "OvertimeApplication",
        name: "OvertimeApplication",
        component: OvertimeApplication,
      },
      {
        path: "OTrecords",
        name: "OTrecords",
        component: OTrecords,
      },
      {
        path: "RolePermissions",
        name: "RolePermissions",
        component: RolePermissions,
      },
      {
        path: "Leaveapprove",
        name: "Leaveapprove",
        component: Leaveapprove,
      },
      {
        path: "Overtimeapprove",
        name: "Overtimeapprove",
        component: Overtimeapprove,
      },
      {
        path: "Viewrecords",
        name: "Viewrecords",
        component: Viewrecords,
      },
      {
        path: "SearchLeaveStatus",
        name: "SearchLeaveStatus",
        component: SearchLeaveStatus,
      },
      {
        path: "SearchOvertimeRepair",
        name: "SearchOvertimeRepair",
        component: SearchOvertimeRepair,
      },
      // ...可以繼續添加更多子路由
    ],
  },
];
const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
