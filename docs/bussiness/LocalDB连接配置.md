# LocalDB 连接配置指南

## 当前 LocalDB 实例信息

```
实例名称: MSSQLLocalDB
版本: 15.0.4382.1
状态: Running
命名管道: np:\\.\pipe\LOCALDB#70702087\tsql\query
数据库: Finance
```

## Navicat 连接配置

### 方法一：标准连接（推荐）

**连接参数**:
```
连接名: Finance LocalDB
主机: (localdb)\MSSQLLocalDB
端口: (留空)
身份验证: Windows 身份验证
数据库: Finance
```

**步骤**:
1. 打开 Navicat for SQL Server
2. 新建连接 → SQL Server
3. 填写上述参数
4. 测试连接 → 确定

### 方法二：命名管道连接

**连接参数**:
```
主机: np:\\.\pipe\LOCALDB#70702087\tsql\query
身份验证: Windows 身份验证
```

⚠️ **注意**: 管道名称中的数字在 LocalDB 重启后会变化，需要重新获取。

### 获取最新管道名称

```powershell
sqllocaldb info MSSQLLocalDB
```

查看输出中的 "Instance pipe name" 字段。

## 其他数据库工具连接

### SQL Server Management Studio (SSMS)

**服务器名称**: `(localdb)\MSSQLLocalDB`

### Azure Data Studio

**服务器**: `(localdb)\MSSQLLocalDB`  
**身份验证类型**: Windows 身份验证

### Visual Studio

**服务器资源管理器**:
1. 添加连接
2. 数据源: Microsoft SQL Server
3. 服务器名称: `(localdb)\MSSQLLocalDB`
4. 数据库名称: Finance

### .NET 连接字符串

```json
"ConnectionStrings": {
  "Default": "Server=(localdb)\\MSSQLLocalDB;Database=Finance;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

## 常见问题

### Q1: 连接失败 "无法连接到服务器"

**解决方案**:
1. 确认 LocalDB 正在运行:
   ```powershell
   sqllocaldb info MSSQLLocalDB
   ```

2. 如果未运行，启动实例:
   ```powershell
   sqllocaldb start MSSQLLocalDB
   ```

### Q2: 找不到数据库 "Finance"

**解决方案**:
1. 先连接到 LocalDB（不指定数据库）
2. 在 Navicat 中查看数据库列表
3. 如果没有 Finance 数据库，运行迁移:
   ```powershell
   cd d:\Project\ZSD-AIDemo
   dotnet ef database update --project .\Finance\src\Inno.CorePlatform.Finance.Infrastructure --startup-project .\Finance\src\Inno.CorePlatform.Finance.Backend
   ```

### Q3: 权限不足

**解决方案**:
- LocalDB 使用 Windows 身份验证
- 确保当前 Windows 用户有访问权限
- LocalDB 默认只允许创建它的用户访问

### Q4: 命名管道名称变化

**原因**: LocalDB 每次重启后，管道名称中的数字会变化

**解决方案**: 使用标准连接方式 `(localdb)\MSSQLLocalDB` 而不是完整管道路径

## 验证连接

### 使用 sqlcmd 验证

```powershell
# 连接到 LocalDB
sqlcmd -S "(localdb)\MSSQLLocalDB" -d Finance

# 查看所有表
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
GO

# 查看 PaymentAutoItem 表结构
sp_help PaymentAutoItem;
GO

# 退出
EXIT
```

### 使用 PowerShell 验证

```powershell
# 查询表数据
sqlcmd -S "(localdb)\MSSQLLocalDB" -d Finance -Q "SELECT COUNT(*) as TableCount FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"
```

## 数据库表结构

当前 Finance 数据库包含以下表：

1. **PaymentAutoItem** - 批量付款单主表
2. **PaymentAutoDetail** - 批量付款明细表
3. **PaymentAutoAgent** - 批量付款供应商表
4. **__EFMigrationsHistory** - EF Core 迁移历史表

## 性能优化建议

### 1. 启用 TCP/IP（可选）

如果需要更好的性能或远程访问：

```powershell
# 以管理员身份运行
sqllocaldb stop MSSQLLocalDB
sqllocaldb delete MSSQLLocalDB
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

### 2. 使用 SQL Server Express

对于生产环境或需要更多功能，建议升级到 SQL Server Express：
- 下载: https://www.microsoft.com/sql-server/sql-server-downloads
- 免费版本，功能更完整
- 支持远程连接

## 备份与恢复

### 备份数据库

```sql
BACKUP DATABASE Finance
TO DISK = 'D:\Backup\Finance.bak'
WITH FORMAT, INIT, NAME = 'Finance-Full Database Backup';
```

### 恢复数据库

```sql
RESTORE DATABASE Finance
FROM DISK = 'D:\Backup\Finance.bak'
WITH REPLACE;
```

## 监控与维护

### 查看数据库大小

```sql
USE Finance;
GO

SELECT 
    DB_NAME() AS DatabaseName,
    SUM(size * 8 / 1024) AS SizeMB
FROM sys.database_files;
GO
```

### 查看表记录数

```sql
SELECT 
    t.NAME AS TableName,
    p.rows AS RowCounts
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.OBJECT_ID
WHERE p.index_id < 2
ORDER BY p.rows DESC;
GO
```

## 相关文档

- [批量付款实现总结](./批量付款实现总结.md)
- [数据库迁移完成报告](./数据库迁移完成报告.md)
- [批量付款数据库设计](./11.批量付款数据库设计.md)
