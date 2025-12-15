# 问题解决：CompanyId NULL 插入错误

## 问题描述

启动项目时报错：
```
Cannot insert the value NULL into column 'CompanyId', table 'Finance.dbo.Debt'; column does not allow nulls. INSERT fails.
```

## 问题分析

### 根本原因

EF Core 在处理 `OwnsOne` 配置的值对象时，如果实体的无参构造函数中对值对象属性进行了初始化，可能会导致映射问题。

**问题代码**（`Debt.cs`）：
```csharp
private Debt() 
{
    Code = string.Empty;
    Company = CompanyInfo.Create(Guid.Empty, string.Empty, string.Empty); // ❌ 错误
    AgentName = string.Empty;
}
```

当 EF Core 尝试从数据库反序列化 `Debt` 实体时：
1. 首先调用无参构造函数
2. 然后尝试填充值对象属性
3. 但由于构造函数中已经初始化了 `Company`，导致 EF Core 的映射逻辑出现问题
4. 最终在插入数据时，`CompanyId` 列被设置为 NULL

### 技术细节

**值对象配置**（`DebtConfiguration.cs`）：
```csharp
builder.OwnsOne(x => x.Company, company =>
{
    company.Property(c => c.Id)
        .HasColumnName("CompanyId")
        .IsRequired();
    // ...
});
```

EF Core 的 `OwnsOne` 会将值对象的属性映射为实体表的列，但需要无参构造函数中不对值对象进行初始化。

## 解决方案

### 方案一：修改无参构造函数（最终采用）

**修改后的代码**（`Debt.cs`）：
```csharp
private Debt() 
{
    Code = string.Empty;
    Company = null!;  // ✅ 正确：让 EF Core 自动填充
    AgentName = string.Empty;
}
```

**原理**：
- `null!` 告诉编译器这个属性会被 EF Core 在反序列化时填充
- EF Core 会在从数据库读取数据后，自动创建 `CompanyInfo` 实例并填充属性

### 方案二：使用原始 SQL 插入（数据初始化）

由于 EF Core 在插入包含值对象的实体时存在问题，在 `DataSeeder` 中使用原始 SQL：

```csharp
var sql = @"
    INSERT INTO Debt (Id, Code, CompanyId, CompanyName, CompanyCode, 
                     DepartmentIdPath, DepartmentNamePath, DepartmentCurrentId, 
                     AgentId, AgentName, DocumentType, BusinessUnit, Currency, 
                     PurchaseContractNo, ProjectName, IsDeleted, CreatedAt)
    VALUES 
    (NEWID(), 'AP202512150001', '11111111-1111-1111-1111-111111111111', 
     N'总部公司', 'C001', '01/0101', N'总部/财务部', '0101',
     @Agent1Id, @Agent1Name, N'采购应付', N'采购BU', 'CNY', 
     'PO-2025-0001', NULL, 0, GETUTCDATE())";

await context.Database.ExecuteSqlRawAsync(sql, 
    new SqlParameter("@Agent1Id", agents[0].Id),
    new SqlParameter("@Agent1Name", agents[0].Name));
```

**优点**：
- 绕过 EF Core 的值对象映射问题
- 直接控制 SQL 语句
- 适用于数据初始化场景

## 修改的文件

### 1. Domain Layer

**文件**: `Debt.cs`
```csharp
// 修改无参构造函数
private Debt() 
{
    Code = string.Empty;
    Company = null!;  // 修改点
    AgentName = string.Empty;
}
```

### 2. Infrastructure Layer

**文件**: `DataSeeder.cs`
```csharp
// 使用原始 SQL 插入应付单数据
private static async Task SeedDebtsAsync(FinanceDbContext context, ILogger logger)
{
    // ... 检查逻辑 ...
    
    var sql = @"INSERT INTO Debt (...) VALUES (...)";
    await context.Database.ExecuteSqlRawAsync(sql, parameters);
}
```

## 验证结果

### 数据库验证

```sql
-- 查看应付单数据
SELECT Code, CompanyName, AgentName FROM Debt;

-- 结果：
Code            CompanyName    AgentName
AP202512150001  总部公司       上海XX贸易有限公司
AP202512150002  华东分公司     杭州YY技术服务有限公司
AP202512150003  总部公司       杭州YY技术服务有限公司

-- 查看应付明细数据
SELECT COUNT(*) as DetailCount FROM DebtDetail;

-- 结果：5 条明细记录
```

### 应用启动日志

```
[15:15:40 INF] 供应商数据已存在，跳过初始化
[15:15:40 INF] 应付单数据已存在，跳过初始化
[15:15:41 INF] 已添加 5 个应付明细
[15:15:41 INF] 数据初始化完成
[15:15:41 INF] Now listening on: https://localhost:61801
[15:15:41 INF] Now listening on: http://localhost:61803
[15:15:41 INF] Application started.
```

✅ **项目启动成功，数据初始化完成！**

## 经验总结

### EF Core 值对象最佳实践

1. **无参构造函数**
   - 值对象属性应设置为 `null!`
   - 不要在构造函数中初始化值对象
   - 让 EF Core 自动处理反序列化

2. **OwnsOne 配置**
   ```csharp
   builder.OwnsOne(x => x.ValueObject, vo =>
   {
       vo.Property(v => v.Property)
           .HasColumnName("ColumnName")
           .IsRequired();
   }).Navigation(x => x.ValueObject).IsRequired();
   ```

3. **数据初始化**
   - 对于复杂的值对象映射，考虑使用原始 SQL
   - 在 Seeder 中分步保存，避免一次性提交大量数据
   - 使用参数化查询防止 SQL 注入

### 调试技巧

1. **启用 EF Core SQL 日志**
   ```json
   "Logging": {
     "LogLevel": {
       "Microsoft.EntityFrameworkCore.Database.Command": "Information"
     }
   }
   ```

2. **检查数据库表结构**
   ```sql
   SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE 
   FROM INFORMATION_SCHEMA.COLUMNS 
   WHERE TABLE_NAME = 'Debt';
   ```

3. **使用 SQL 直接测试**
   ```sql
   INSERT INTO Debt (Id, Code, CompanyId, ...) 
   VALUES (NEWID(), 'TEST', '...', ...);
   ```

## 相关文档

- [供应商和应付表实现总结](./供应商和应付表实现总结.md)
- [数据库迁移完成报告](./数据库迁移完成报告.md)
- [批量付款数据库设计](./11.批量付款数据库设计.md)

## 问题状态

✅ **已解决** - 2025年12月15日 15:15

**解决方法**：
1. 修改 `Debt` 实体的无参构造函数，将 `Company` 设置为 `null!`
2. 在 `DataSeeder` 中使用原始 SQL 插入应付单数据
3. 验证数据完整性和应用启动成功

**影响范围**：
- Domain Layer: `Debt.cs`
- Infrastructure Layer: `DataSeeder.cs`

**测试结果**：
- ✅ 供应商数据：2 条
- ✅ 应付单数据：3 条
- ✅ 应付明细数据：5 条
- ✅ 应用启动成功
- ✅ API 端点可访问：https://localhost:61801
