# 书签管理系统 (NetFavorite)

基于 .NET Core 10.0 的书签管理系统后端 API，支持用户认证、书签管理、分类管理等功能。

## 技术栈

- **框架**: .NET Core 10.0
- **数据库**: SQL Server + Entity Framework Core
- **认证**: JWT Bearer Token
- **密码加密**: PBKDF2 + HMACSHA512（带随机盐值）
- **API 文档**: Swagger/Swashbuckle
- **ORM**: Entity Framework Core

## 功能特性

### 用户管理
- 用户注册（自动加密密码）
- 用户登录（JWT Token 认证）
- 修改密码（验证旧密码，加密新密码）
- 用户信息查询与修改
- 用户删除

### 书签管理
- 书签 CRUD 操作
- 按分类筛选书签
- 书签搜索

### 分类管理
- 分类 CRUD 操作
- 分类下书签统计

### 安全特性
- 密码加盐哈希加密（PBKDF2 + HMACSHA512，20次迭代）
- JWT Token 认证
- 接口权限控制（[Authorize] / [AllowAnonymous]）

## 项目结构

```
NetFavorite/
├── Controllers/          # 控制器
│   ├── LoginController.cs        # 登录、修改密码、测试接口
│   ├── LoginUserController.cs    # 用户管理
│   ├── BookmarkController.cs     # 书签管理
│   └── CategoryController.cs     # 分类管理
├── Models/               # 数据模型
│   ├── LoginUser.cs                # 用户模型
│   ├── Bookmark.cs                 # 书签模型
│   └── Category.cs                 # 分类模型
├── Utilities/           # 工具类
│   ├── HashPasswordService.cs     # 密码加密服务
│   └── TokenService.cs            # JWT Token 服务
├── wwwroot/swagger-ui/   # Swagger 自定义资源
│   ├── custom-swagger.css         # 自定义样式
│   └── custom-swagger.js          # 自定义脚本
├── NetFavoriteDbContext.cs       # 数据库上下文
├── Program.cs                     # 程序入口
└── appsettings.json              # 配置文件
```

## 快速开始

### 环境要求

- .NET SDK 10.0 或更高版本
- SQL Server 2019 或更高版本
- VS Code（推荐）或 Visual Studio 2022

### 安装步骤

1. **克隆项目**
```bash
git clone <repository-url>
cd webb
```

2. **配置数据库连接**

编辑 `appsettings.json` 文件：

```json
{
  "ConnectionStrings": {
    "NetFavoriteDbContext": "Server=localhost;Database=NetFavorite;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "JWT": {
    "SecretKey": "your-secret-key-here",
    "Expires": 10,
    "Issuer": "your-issuer",
    "Audience": "your-audience"
  }
}
```

3. **创建数据库**

在 SQL Server Management Studio (SSMS) 中执行：

```sql
CREATE DATABASE NetFavorite;
GO
```

4. **运行数据库迁移**

```bash
dotnet ef database update
```

或手动执行 `数据库更新脚本.sql`。

5. **启动项目**

```bash
dotnet run
```

项目启动后，浏览器会自动打开 Swagger 页面：http://localhost:5248/swagger

## API 接口文档

### 登录认证

#### POST /api/Login - 用户登录
**请求参数**:
| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| account | string | 是 | 用户账号 |
| password | string | 是 | 用户密码 |

**响应示例**:
```json
{
  "id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "account": "user01",
  "role": "普通用户",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

#### PATCH /api/Login - 修改密码（需认证）
**请求参数**:
| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| id | Guid | 是 | 用户ID |
| oldPassword | string | 是 | 旧密码 |
| newPassword | string | 是 | 新密码 |

**请求头**: `Authorization: Bearer <token>`

### 用户管理

#### GET /api/LoginUser - 获取所有用户列表
**响应示例**:
```json
[
  {
    "LoginUser_Id": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "LoginUser_Account": "user01",
    "LoginUser_Role": "普通用户"
  }
]
```

#### POST /api/LoginUser - 新增用户（需认证）
**请求体**:
```json
{
  "LoginUser_Account": "newuser",
  "LoginUser_Password": "password123",
  "LoginUser_Role": "普通用户"
}
```

**说明**: 密码会自动加密存储，无需手动处理。

#### PUT /api/LoginUser/{id} - 修改用户信息（需认证）
**请求体**:
```json
{
  "LoginUser_Account": "user01",
  "LoginUser_Password": "newpassword",
  "LoginUser_Role": "管理员"
}
```

**说明**: 密码会重新生成 Salt 并加密。

### 测试接口（开发环境）

#### GET /api/Login/test/salt - 生成随机盐值
**用途**: 用于生成新的 Salt 值

#### GET /api/Login/test/hash - 加密密码
**请求参数**:
| 参数 | 类型 | 必填 | 说明 |
|------|------|------|------|
| password | string | 是 | 明文密码 |
| salt | string | 是 | 盐值 |

**响应**: 返回加密后的密码字符串

## 密码安全机制

本项目采用 **PBKDF2 + HMACSHA512** 算法进行密码加密：

1. **生成随机盐值** (128位)
2. **使用盐值对密码进行哈希** (HMACSHA512)
3. **迭代次数**: 20次
4. **输出长度**: 256位 (32字节)

### 加密流程

```
明文密码 → CreateSalt() → HashPassword(password, salt) → 加密后的密码
```

### 验证流程

```
输入密码 → HashPassword(input, storedSalt) == storedPassword → 验证成功
```

### 核心代码

```csharp
// 生成随机盐值
string salt = HashPasswordService.CreateSalt();

// 加密密码
string hashedPassword = HashPasswordService.HashPassword("password", salt);

// 验证密码
bool isValid = HashPasswordService.Validate("password", salt, hashedPassword);
```

## 开发指南

### VS Code 调试配置

项目已配置好 VS Code 调试环境：
- 按 F5 启动调试
- 自动打开浏览器至 Swagger 页面
- 支持断点调试

### 常用命令

```bash
# 构建项目
dotnet build

# 运行项目
dotnet run

# 清理项目
dotnet clean

# 运行测试
dotnet test

# 添加迁移
dotnet ef migrations add MigrationName

# 更新数据库
dotnet ef database update
```

### 数据库 Schema

#### LoginUser 表
| 字段名 | 类型 | 说明 |
|--------|------|------|
| LoginUser_Id | uniqueidentifier | 主键 |
| LoginUser_Account | varchar(50) | 账号 |
| LoginUser_Password | varchar(500) | 加密后的密码 |
| LoginUser_Role | varchar(50) | 角色 |
| LoginUser_Salt | nvarchar(500) | 盐值 |

#### Bookmark 表
| 字段名 | 类型 | 说明 |
|--------|------|------|
| Bookmark_Id | uniqueidentifier | 主键 |
| Bookmark_Title | nvarchar(200) | 标题 |
| Bookmark_Url | nvarchar(1000) | URL |
| Category_Id | uniqueidentifier | 外键（分类ID） |
| User_Id | uniqueidentifier | 外键（用户ID） |

#### Category 表
| 字段名 | 类型 | 说明 |
|--------|------|------|
| Category_Id | uniqueidentifier | 主键 |
| Category_Name | varchar(50) | 分类名称 |
| User_Id | uniqueidentifier | 外键（用户ID） |

## 配置说明

### JWT 配置

```json
"JWT": {
  "SecretKey": "www.czie.edu.cn123456789www.czie.edu.cn123456789",  // JWT 密钥（至少16位）
  "Expires": 10,      // Token 过期时间（分钟）
  "Issuer": "czie",   // 签发者
  "Audience": "test"  // 接收者
}
```

### Swagger 配置

项目使用自定义的 Swagger UI 样式：
- 紫色渐变主题
- 彩色 API 端点分类
- 右下角浮动按钮跳转测试页面

自定义资源位于 `wwwroot/swagger-ui/` 目录。

## 常见问题

### Q: 启动时提示端口被占用？
A: 终端执行：`taskkill /F /IM NetFavorite.exe` 终止占用进程

### Q: 提示 "列名 'LoginUser_Salt' 无效"？
A: 确认已执行数据库迁移脚本，且连接的是正确的数据库（NetFavorite 而非 master）

### Q: Swagger 测试返回 401 Unauthorized？
A: 需要先调用登录接口获取 Token，然后点击 Authorize 按钮添加 Authorization 头

### Q: 如何重置用户密码？
A: 使用测试接口 `/api/Login/test/salt` 和 `/api/Login/test/hash` 生成新的 Salt 和加密密码，然后在 SSMS 中 UPDATE 对应记录

## 项目依赖

| 包名 | 版本 | 用途 |
|------|------|------|
| Microsoft.EntityFrameworkCore.SqlServer | 10.0.5 | SQL Server 数据库访问 |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.5 | JWT 认证 |
| Microsoft.AspNetCore.Cryptography.KeyDerivation | 10.0.7 | 密码加密算法 |
| Swashbuckle.AspNetCore | 6.5.0 | Swagger API 文档 |
| BCrypt.Net-Next | 2.2.1 | BCrypt 加密备用方案 |

## 许可证

MIT License

## 作者

1aush

---

**最后更新**: 2026-04-28
