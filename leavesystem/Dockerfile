# 使用較新的 Node.js 版本
FROM node:lts-alpine

# 設置環境變量
ENV NODE_ENV=development

# 設置工作目錄到 /usr/src/app/leavesystem
WORKDIR /usr/src/app/leavesystem

# 覆制 package.json 和相關文件到 /usr/src/app/leavesystem
COPY leavesystem/package*.json ./

# 安裝項目依賴
RUN npm install

# 覆制項目文件到 /usr/src/app/leavesystem
COPY leavesystem/ .

# 暴露端口
EXPOSE 8080

# 更改文件擁有者
RUN chown -R node /usr/src/app

# 切換用戶
USER node

# 運行開發服務器
CMD ["npm", "run", "serve"]
