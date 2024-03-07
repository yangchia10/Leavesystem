import { createStore } from "vuex";
import axios from "axios";

const store = createStore({
  state() {
    return {
      // 直接從 Vuex 中管理用戶信息，不再依賴 localStorage
      userInfo: { role: null },

      overtimeHours: 0, // 加班時數（以分鐘為單位）
      compensationHours: 0, // 補休時數（以分鐘為單位）
    };
  },
  mutations: {
    // 設置用戶信息
    setUserInfo(state, userInfo) {
      console.log("Mutation: Setting user info", userInfo);
      state.userInfo = userInfo;
    },
    // 清除用戶信息
    clearUserInfo(state) {
      console.log("Clearing user info");
      state.userInfo = { role: null };
    },
    // 新增：設置加班和補休時數的突變
    setOvertimeHours(state, hours) {
      state.overtimeHours = hours;
    },
    setCompensationHours(state, hours) {
      state.compensationHours = hours;
    },
  },
  actions: {
    // 用戶登出
    async logout({ commit }) {
      try {
        const response = await fetch("http://localhost:5163/api/Login/logout", {
          method: "POST",
          credentials: "include", // 確保發送 cookie
        });

        if (response.ok) {
          // 清除 Vuex 中的用戶信息
          commit("clearUserInfo");
          console.log("Logout successful");
          // 這里不負責重定向，重定向邏輯應該在調用 logout action 的組件中處理
        } else {
          throw new Error(
            "Logout failed with response status: " + response.status
          );
        }
      } catch (error) {
        console.error("Logout failed:", error);
      }
    },

    // 獲取用戶信息
    async fetchUserInfo({ commit }) {
      try {
        const response = await fetch("http://localhost:5163/api/HomePage", {
          method: "GET",
          credentials: "include", // 確保發送HttpOnly Cookie
        });

        if (!response.ok) {
          console.error(
            "Failed to fetch user info, HTTP status:",
            response.status
          );
          return;
        }

        const data = await response.json();
        commit("setUserInfo", data); // 假設後端返回的數據直接是用戶信息
        console.log("Fetched and set user info:", data);
      } catch (error) {
        console.error("Error fetching user info:", error);
      }
    },
    fetchUserOvertimeAndCompensation({ commit, state }) {
      // 假設有一個API端點，根據當前登錄的用戶ID獲取其加班和補休時間
      const userId = state.userInfo.employee_id; // 使用從state中獲取的用戶ID
      axios
        .get(`/api/SearchOvertimeRepair/overtimerecords/${userId}`)
        .then((response) => {
          const { TotalOvertimeMinutes, TotalCompensationHours } =
            response.data;
          commit("setOvertimeHours", TotalOvertimeMinutes);
          commit("setCompensationHours", TotalCompensationHours);
        })
        .catch((error) => console.error("獲取用戶加班和補休時間失敗:", error));
    },
  },
  getters: {
    remainingCompensationHours: (state) => {
      return ((state.overtimeHours - state.compensationHours) / 60).toFixed(2);
    },
  },
});

export default store;
