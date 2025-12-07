# 房產管理系統 (Real Estate Management System)

這是一個基於 **ASP.NET Core 8.0** 開發的全方位房產管理平台。系統採用標準的 **N-Tier 分層架構** 與 **Repository Pattern** 設計，實現了高可維護性的前後台分離邏輯。

專案模擬了真實的 O2O (Online to Offline) 業務場景：房仲透過 **MVC 後台** 管理房源與預約，客戶透過 **Web API** (模擬 App 端) 進行瀏覽與預約看屋。

## 核心功能 (Key Features)

### 👨‍💼 房仲後台 (Admin Portal - MVC)
* **現代化儀表板**：基於 Bootstrap 5 的卡片式設計 (Dashboard UI)，介面直觀。
* **房源管理 (CRUD)**：
    * 支援房源的新增、修改、刪除與查詢。
    * **圖片上傳**：整合 `IFormFile` 支援房源封面照片上傳與即時預覽。
    * **權限控管 (RBAC)**：實作所有權驗證，房仲只能編輯或刪除**自己負責**的房源。
    * **狀態管理**：支援房源上架/下架切換，下架時系統會自動邏輯判斷並取消相關預約。
* **預約審核**：
    * 專屬的預約管理看板，僅顯示該房仲負責案件的預約。
    * 支援「確認」、「拒絕」或「完成帶看」等狀態變更操作。
* **安全性**：使用 **Cookie Authentication** 與 **BCrypt** 密碼雜湊確保後台安全。

### 📱 客戶服務 (Client API - Web API)
* **RESTful 風格 API**：提供標準化的 JSON 介面，並整合 **Swagger UI** 文件。
* **JWT 身分驗證**：使用 JSON Web Token 保護 API 端點，實現無狀態 (Stateless) 驗證。
* **線上預約**：
    * 客戶可查詢房源詳情並送出看屋預約。
    * 支援預約改期 (Reschedule) 與取消 (Cancel) 功能。
    * 提供查詢個人歷史紀錄。

---

## 技術堆疊 (Tech Stack)

* **框架**：.NET 8.0 (ASP.NET Core MVC & Web API)
* **資料庫**：SQL Server (LocalDB)
* **ORM**：Entity Framework Core (Code First)
* **架構模式**：
    * **N-Tier Architecture**: `Web`, `Infrastructure`, `Core`
    * **Repository Pattern**: 封裝資料存取邏輯
    * **Service Layer**: 封裝商業邏輯 (Business Logic)
* **安全性**：
    * **Hybrid Auth**: 同時支援 Cookie (MVC) 與 JWT (API) 雙重驗證
    * **BCrypt**: 密碼安全雜湊
    * **TransactionScope**: 確保跨表操作的資料一致性
* **前端**: Razor Views, Bootstrap 5, jQuery

---

## 快速啟動 (Getting Started)

### 1. 前置需求
* Visual Studio 2022 或 VS Code
* .NET SDK 8.0
* SQL Server LocalDB

### 2.專案執行
```bash
git clone [https://github.com/yt-sideproj/RealEstateManagementSystem.git](https://github.com/yt-sideproj/RealEstateManagementSystem.git)
cd RealEstateManagementSystem
```
### 3. 設定與執行
專案內建 `DbInitializer`，啟動時會自動建立資料庫並寫入測試資料 (Seed Data)。您無需手動執行 Migration 指令。

1.  確認 `appsettings.json` 連線字串 (預設使用 LocalDB)。
2.  在 Visual Studio 選擇 RealEstateManagement.Web 按下 **F5**，或在終端機執行：
    ```bash
    cd RealEstateManagement.Web
    dotnet run
    ```

### 4. 體驗系統
* **後台首頁**：瀏覽器會自動開啟 `https://localhost:7xxx/` 並導向房源列表。
* **API 文件**：前往 `https://localhost:7xxx/swagger` 查看與測試 API。

---

## 測試帳號 (Test Accounts)

系統啟動時會自動寫入以下種子資料，請使用這些帳號進行完整流程測試。

### 👨‍💼 房仲 (後台 MVC 登入)

| 角色 | 員工編號 (帳號) | 密碼 | 權限說明 |
| :--- | :--- | :--- | :--- |
| **房仲 A** | `A001` | `1234` | 負責台北市大安區、新北市板橋區案件 |
| **房仲 B** | `A002` | `1234` | 負責台北市內湖區、信義區案件 |

> **💡 測試情境**：嘗試用 `A001` 登入，您會發現無法編輯或刪除屬於 `A002` 的房源，驗證權限隔離機制。

### 📱 客戶 (API Swagger 登入)

| 角色 | Email (帳號) | 密碼 | 用途 |
| :--- | :--- | :--- | :--- |
| **客戶 1** | `user1@test.com` | `1111` | 一般客戶 API 測試 |
| **客戶 2** | `user2@test.com` | `1111` | 一般客戶 API 測試 |

---

## 專案結構 (Project Structure)

```text
RealEstateManagementSystem/
├── RealEstateManagement.Core/           # 核心層 (Models, Interfaces, DTOs)
│   ├── Models/                          # 資料庫實體 (Entities)
│   ├── Interfaces/                      # Repository 與 Service 介面
│   └── DTOs/                            # 資料傳輸物件
│
├── RealEstateManagement.Infrastructure/ # 基礎設施層 (EF Core, Services Impl)
│   ├── Data/                            # DbContext 與 Seed Data
│   ├── Repositories/                    # 資料存取邏輯實作
│   └── Services/                        # 商業邏輯實作
│
└── RealEstateManagement.Web/            # 展示層 (MVC & API)
    ├── Controllers/                     # MVC Controllers (Account, Houses, Appointments)
    ├── Controllers/Api/                 # Web API Controllers (Auth, AppointmentsApi)
    ├── Views/                           # Razor Views (UI)
    └── wwwroot/                         # 靜態檔案 (css, js, images)