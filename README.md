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

graph TD
    A[使用者登入.vue] -->|登入成功| B[首頁.vue]
    A[使用者登入.vue] -.->|註冊| C[使用者註冊.vue]
    B --> D[請假申請表單.vue]
    B --> E[加班申請.vue]
    B --> F[更改帳戶.vue]
    B --> G[查看紀錄.vue]
    B --> H[角色權限.vue]
    D --> I[請假審批.vue]
    E --> J[加班審批.vue]
    G --> K[查詢請假狀態.vue]
    G --> L[查詢加班補休.vue]
    H --> M[角色權限.vue]

    style A fill:#f9f,stroke:#333,stroke-width:2px
    style B fill:#bbf,stroke:#f66,stroke-width:2px
    style C fill:#f9f,stroke:#333,stroke-width:2px
    style D fill:#bbf,stroke:#333,stroke-width:2px
    style E fill:#bbf,stroke:#333,stroke-width:2px
    style F fill:#bbf,stroke:#333,stroke-width:2px
    style G fill:#bbf,stroke:#333,stroke-width:2px
    style H fill:#bbf,stroke:#333,stroke-width:2px
    style I fill:#bbf,stroke:#333,stroke-width:2px
    style J fill:#bbf,stroke:#333,stroke-width:2px
    style K fill:#bbf,stroke:#333,stroke-width:2px
    style L fill:#bbf,stroke:#333,stroke-width:2px
    style M fill:#bbf,stroke:#333,stroke-width:2px
