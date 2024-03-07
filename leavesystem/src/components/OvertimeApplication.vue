<template>
  <div class="overtime-application">
    <h2>加班申請</h2>
    <form @submit.prevent="submitForm" class="overtime-form">
      <div class="date-time-container">
        <div class="cross-midnight-selection">
          <label>跨夜加班：</label>
          <input
            type="radio"
            id="是"
            value="是"
            v-model="crossMidnight"
            @change="handleCrossMidnightChange"
          />
          <label for="是">是</label>
          <input
            type="radio"
            id="否"
            value="否"
            v-model="crossMidnight"
            @change="handleCrossMidnightChange"
          />
          <label for="否">否</label>
          <span v-if="crossMidnight"
            >選擇的是：{{
              crossMidnight === "是" ? "是（跨夜加班）" : "否（非跨夜加班）"
            }}</span
          >
        </div>
        <div class="date-time-container" v-if="crossMidnight">
          <label for="overtime-date">加班日期：</label>
          <input type="date" id="overtime-date" v-model="overtimeDate" />
          <input
            type="time"
            id="overtime-start-time"
            v-model="overtimeStartTime"
          />
          <span>至</span>
          <input type="time" id="overtime-end-time" v-model="overtimeEndTime" />
        </div>
      </div>

      <label for="reason">加班原因：</label>
      <textarea id="reason" v-model="reason"></textarea>

      <button
        type="submit"
        class="btn-submitForm"
        :disabled="isOvertimeExceeded"
        :class="{ 'disabled-button': isOvertimeExceeded }"
      >
        提交申請
      </button>
      <button class="btn-clearForm" type="button" @click="clearForm">
        清除表單
      </button>
      <div class="error-message" v-if="isOvertimeExceeded"></div>
    </form>
    <div class="overtime-duration">
      <p v-if="isNextDayWorkTimeExceeded" class="error-text">
        已是新的一天上班日。
      </p>
      <p
        v-else-if="isOvertimeExceeded && overtimeDuration >= 300"
        class="error-text"
      >
        已超過今日加班時間上限，請另外選擇日期。
      </p>
      <p v-else-if="overtimeDuration > 0">
        加班時長: {{ Math.floor(overtimeDuration / 60) }} 小時
        {{ overtimeDuration % 60 }} 分鐘
      </p>
      <p v-else>加班時長: 0 小時</p>
    </div>
  </div>
</template>

<script>
import OvertimeApplicationForm from "@/JS/OvertimeApplication.js";

export default {
  mixins: [OvertimeApplicationForm],
  name: "OvertimeApplicationForm",
};
</script>

<style scoped>
/* 主要容器樣式 */
.overtime-application {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh; /* 使表單居中並占據整個視口高度 */
  font-family: Arial, sans-serif; /* 設置字體 */
}

/* 表單樣式 */
.overtime-form {
  display: flex;
  flex-direction: column;
  align-items: center;
  background: #f4f4f4;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1); /* 添加陰影效果 */
}

.overtime-form label {
  margin: 10px 0;
  font-weight: bold;
}

.overtime-form input[type="date"],
.overtime-form input[type="time"],
.overtime-form textarea {
  width: 100%;
  padding: 10px;
  margin: 5px 0;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-size: 14px;
}

.overtime-form input[type="time"] {
  display: inline-block;
  width: auto;
}

/* 提交申請按鈕樣式 */
.btn-submitForm {
  background-color: #3498db; /* 藍色背景 */
  color: white;
  width: 100%; /* 按鈕占滿整個寬度 */
  margin: 20px 0; /* 添加上下邊距，以及左右邊距 */
  padding: 10px;
  border: 否ne;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.btn-submitForm:hover {
  background-color: #2980b9;
}

/* 清除表單按鈕樣式 */
.btn-clearForm {
  background-color: #e74c3c; /* 紅色背景 */
  color: white;
  width: 100%; /* 按鈕占滿整個寬度 */
  margin: 20px 0; /* 添加上下邊距，以及左右邊距 */
  padding: 10px;
  border: 否ne;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.btn-clearForm:hover {
  background-color: #de786c;
}

/* 加班時長顯示樣式 */
.overtime-duration {
  font-size: 18px;
  font-weight: bold;
}

/* "至" 兩側添加空格 */
.date-time-container span {
  margin: 0 10px;
}

.error-text {
  color: red;
  font-weight: bold;
}

/* 禁用狀態的按鈕樣式 */
.disabled-button {
  background-color: #ccc; /* 灰色背景 */
  color: #999; /* 灰色文字顏色 */
  cursor: 否t-allowed; /* 鼠標指針樣式設置為不可用 */
  transition: 否ne; /* 移除過渡效果 */
  pointer-events: 否ne; /* 禁用按鈕的交互 */
}

.cross-midnight-button {
  margin: 10px;
  padding: 10px 20px;
  background-color: #3498db;
  color: white;
  border: 否ne;
  border-radius: 5px;
  cursor: pointer;
}
.cross-midnight-button:hover {
  background-color: #2980b9;
}

.cross-midnight-selection {
  margin-bottom: 20px;
}
</style>
