import { createApp } from "vue";
import App from "./App.vue";
import * as ElementPlusIconsVue from "@element-plus/icons-vue";
import ElementPlus from "element-plus";
import "element-plus/dist/index.css";
import router from "./router.js"; // 導入路由配置
import store from "./store"; // 確保這個路徑是正確的

// 導入 CSS

import "./assets/css/UserRegister.css";
import "./assets/css/UserLogin.css";
import "./assets/css/homePage.css";
import "./assets/css/Leaveapprove.css";
import "./assets/css/Overtimeapprove.css";

// 創建 Vue 應用實例
const app = createApp(App);

// 注冊 ElementPlus 圖標
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component);
}

// 使用 ElementPlus
app.use(ElementPlus);

// 使用 Vuex store
app.use(store);

// 使用 Vue Router
app.use(router);

// 掛載 Vue 應用實例
app.mount("#app");
