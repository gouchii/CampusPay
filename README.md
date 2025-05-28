# ASP.NET Core Web API

This is a RESTful Web API built using ASP.NET Core and Entity Framework Core.

---

## 🚀 Features

- Clean and modular architecture
- Entity Framework Core for data access
- SignalR for real-time communication
- SQL Server Express support

---

## 🛠️ Setup Instructions

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/your-username/your-repo-name.git
   ```
2. **Install SQL Server Express**
   
    Download and install SQL Server Express from the official Microsoft website:

    https://www.microsoft.com/en-us/sql-server/sql-server-downloads

3. **Modify the connection string**

    Open the appsettings.json file and update the DefaultConnection string with your SQL Server details:

```json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=YOUR_PC_NAME\\SQLEXPRESS;Initial Catalog=yOUR_DATABASE_NAME;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
  }
```

4. **Apply EF Core migrations**

If EF Core CLI tools are not installed, run:

```bash
dotnet tool install --global dotnet-ef
```
Then apply the migrations using:

```bash
dotnet ef database update
```
---
## ▶️ Running the App

Start the API server with:
```bash
dotnet run
```
