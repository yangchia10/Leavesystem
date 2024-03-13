# leavesystem 請假系統

## 前端資料夾

```
leavesystem
```

### 前端檔案

```
leavesystem\src\components 主要程式畫面
leavesystem\src\assets\css css設定
leavesystem\src\JS js檔案
其他的主要檔案說明在文件
```

### 執行前端

```
到前端資料夾輸入指令

npm run serve
```

## 後端資料夾

```
backend
```

### 後端檔案

```
backend\leaveSystem_vue\leaveSystem_vue\Controllers  API程式檔
backend\leaveSystem_vue\leaveSystem_vue\Data    理數據庫的實體模型和它們之間的關係
backend\leaveSystem_vue\leaveSystem_vue\Models  資料庫model
```

```
backend\leaveSystem_vue\leaveSystem_vue\appsettings.json

這份的內容資料庫連線，現在是連線到docker內的，如果要改成本地端的請手動更改
Server=sqlserver
Database=leaveSystem
User Id=SAPassword=!QAZ2wsx#EDC
```

其他詳細資料請看請假系統文件

## docker-compose.yml

```
docker-compose.yml

在docker-compose目錄下執行
docker-compose up -d
```

## 資料庫

```
leaveSystem.bak
```


