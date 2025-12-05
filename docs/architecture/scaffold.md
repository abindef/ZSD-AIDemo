---
trigger: manual
---

# 项目脚手架定义

> 本文档定义项目的基础架构脚手架，AI生成代码时必须遵循此结构

## 一、技术选型

### 1.1 核心框架

| 类别 | 技术 | 版本 | 说明 |
|------|------|------|------|
| 运行时 | .NET | 8.0+ | LTS版本 |
| Web框架 | ASP.NET Core | 8.0+ | Minimal API + Controllers |
| ORM | Entity Framework Core | 8.0+ | Code First |
| 云原生 | Dapr | 1.12+ | 服务调用、Pub/Sub、状态管理 |
| 依赖注入 | Microsoft.Extensions.DI | 内置 | 构造函数注入 |
| 日志 | Serilog | 3.x | 结构化日志 |
| 验证 | FluentValidation | 11.x | 命令/查询验证 |
| 测试 | xUnit + Moq + FluentAssertions | - | 单元测试 |

### 1.2 辅助库

| 类别 | 技术 | 用途 |
|------|------|------|
| 对象映射 | Mapster | DTO映射 |
| API文档 | Swagger/OpenAPI | 接口文档 |
| 健康检查 | AspNetCore.HealthChecks | 服务健康监控 |
| 分布式追踪 | OpenTelemetry | 可观测性 |

---

## 二、项目结构

### 2.1 解决方案结构

```
Finance/
├── src/
│   ├── Inno.CorePlatform.Finance.Domain/                 # 领域层（核心）
│   │   ├── Aggregates/                 # 聚合根
│   │   │   ├── Receivables/            # 应收聚合
│   │   │   │   ├── Receivable.cs       # 聚合根
│   │   │   │   ├── ReceivableId.cs     # 强类型ID
│   │   │   │   └── ReceivableStatus.cs # 状态枚举
│   │   │   ├── Payables/               # 应付聚合
│   │   │   └── Claims/                 # 认款聚合
│   │   ├── ValueObjects/               # 值对象
│   │   │   ├── Money.cs                # 金额
│   │   │   └── PaymentTerm.cs          # 账期
│   │   ├── Events/                     # 领域事件
│   │   │   ├── ReceivableCreatedEvent.cs
│   │   │   └── ReceivableSettledEvent.cs
│   │   ├── Services/                   # 领域服务
│   │   │   └── WriteOffService.cs      # 冲销服务
│   │   ├── Repositories/               # 仓储接口
│   │   │   └── IReceivableRepository.cs
│   │   └── Exceptions/                 # 领域异常
│   │       └── BusinessException.cs
│   │
│   ├── Inno.CorePlatform.Finance.Application/            # 应用层
│   │   ├── Commands/                   # 命令
│   │   │   └── ClaimReceivable/
│   │   │       ├── ClaimReceivableCommand.cs
│   │   │       ├── ClaimReceivableHandler.cs
│   │   │       └── ClaimReceivableValidator.cs
│   │   ├── Queries/                    # 查询
│   │   │   └── GetReceivable/
│   │   ├── EventHandlers/              # 事件处理器
│   │   │   └── ReceivableCreatedHandler.cs
│   │   ├── Services/                   # 应用服务
│   │   │   └── IClaimService.cs
│   │   └── DTOs/                       # 数据传输对象
│   │       └── ReceivableDto.cs
│   │
│   ├── Inno.CorePlatform.Finance.Infrastructure/         # 基础设施层
│   │   ├── Persistence/                # 持久化
│   │   │   ├── FinanceDbContext.cs
│   │   │   ├── Configurations/         # EF配置
│   │   │   │   └── ReceivableConfiguration.cs
│   │   │   └── Repositories/           # 仓储实现
│   │   │       └── ReceivableRepository.cs
│   │   ├── Adapters/                   # 外部适配器
│   │   │   ├── Kingdee/                # 金蝶集成
│   │   │   └── Dapr/                   # Dapr集成
│   │   └── Extensions/                 # 扩展方法
│   │       └── ServiceCollectionExtensions.cs
│   │
│   ├── Inno.CorePlatform.Finance.Backend/           # 后端宿主（内部调用）
│   │   ├── Controllers/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   └── Inno.CorePlatform.Finance.WebApi/            # WebApi宿主（对外能力）
│       ├── Controllers/
│       ├── Program.cs
│       └── appsettings.json
│
├── tests/
│   ├── Inno.CorePlatform.Finance.Domain.Tests/           # 领域层测试
│   │   ├── Aggregates/
│   │   │   └── ReceivableTests.cs
│   │   └── ValueObjects/
│   │       └── MoneyTests.cs
│   ├── Inno.CorePlatform.Finance.Application.Tests/      # 应用层测试
│   └── Inno.CorePlatform.Finance.Integration.Tests/      # 集成测试
│
└── docs/                               # 文档
    ├── glossary.md                     # 术语表
    ├── roles/                          # 角色定义
    ├── templates/                      # 模板
    └── architecture/                   # 架构文档
```

### 2.2 命名空间约定

```csharp
// 领域层
namespace Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
namespace Inno.CorePlatform.Finance.Domain.ValueObjects;
namespace Inno.CorePlatform.Finance.Domain.Events;
namespace Inno.CorePlatform.Finance.Domain.Services;

// 应用层
namespace Inno.CorePlatform.Finance.Application.Commands.ClaimReceivable;
namespace Inno.CorePlatform.Finance.Application.Queries.GetReceivable;
namespace Inno.CorePlatform.Finance.Application.EventHandlers;

// 基础设施层
namespace Inno.CorePlatform.Finance.Infrastructure.Persistence;
namespace Inno.CorePlatform.Finance.Infrastructure.Adapters.Kingdee;

// 宿主层
namespace Inno.CorePlatform.Finance.Backend.Controllers;
namespace Inno.CorePlatform.Finance.WebApi.Controllers;
```

---

## 三、分层规范

### 3.1 依赖规则

```
┌─────────────────────────────────────────────────────────┐
│                    Host (Backend/WebApi)                │
│                         ↓                               │
│  ┌─────────────────────────────────────────────────┐   │
│  │              Application                         │   │
│  │                   ↓                              │   │
│  │  ┌─────────────────────────────────────────┐    │   │
│  │  │            Domain (核心)                 │    │   │
│  │  │         不依赖任何外层                   │    │   │
│  │  └─────────────────────────────────────────┘    │   │
│  └─────────────────────────────────────────────────┘   │
│                         ↑                               │
│  ┌─────────────────────────────────────────────────┐   │
│  │           Infrastructure                         │   │
│  │    实现Domain定义的接口，依赖外部库              │   │
│  └─────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

### 3.2 各层职责

| 层 | 职责 | 可以依赖 | 不可依赖 |
|-----|------|----------|----------|
| **Domain** | 业务逻辑、领域规则 | 无 | Application, Infrastructure, Host |
| **Application** | 用例编排、事务管理 | Domain | Infrastructure, Host |
| **Infrastructure** | 技术实现、外部集成 | Domain, Application | Host |
| **Host** | 启动配置、API暴露 | 全部 | - |

---

## 四、编码规范

### 4.1 聚合根模板

```csharp
namespace Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;

/// <summary>
/// 应收单聚合根
/// </summary>
public class Receivable : Entity<ReceivableId>, IAggregateRoot
{
    // 私有构造函数，防止直接实例化
    private Receivable() { }
    
    // 属性（只读）
    public Money Amount { get; private set; }
    public ReceivableStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    // 导航属性（只读集合）
    private readonly List<Claim> _claims = new();
    public IReadOnlyCollection<Claim> Claims => _claims.AsReadOnly();
    
    // 工厂方法
    public static Receivable Create(ReceivableId id, Money amount, /* 其他参数 */)
    {
        // 参数校验
        Guard.Against.Null(id, nameof(id));
        Guard.Against.Null(amount, nameof(amount));
        
        var receivable = new Receivable
        {
            Id = id,
            Amount = amount,
            Status = ReceivableStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        
        // 发布领域事件
        receivable.AddDomainEvent(new ReceivableCreatedEvent(id, amount));
        
        return receivable;
    }
    
    // 业务方法
    public void Claim(Money claimAmount, ClaimType type)
    {
        // 不变量校验
        if (Status == ReceivableStatus.Settled)
            throw new BusinessException("已核销的应收单不能再认款");
        
        // 业务逻辑
        var claim = Claim.Create(this.Id, claimAmount, type);
        _claims.Add(claim);
        
        // 状态流转
        if (GetRemainingAmount() == Money.Zero)
        {
            Status = ReceivableStatus.Settled;
            AddDomainEvent(new ReceivableSettledEvent(Id, Amount));
        }
    }
    
    // 私有方法
    private Money GetRemainingAmount() => Amount - _claims.Sum(c => c.Amount);
}
```

### 4.2 值对象模板

```csharp
namespace Inno.CorePlatform.Finance.Domain.ValueObjects;

/// <summary>
/// 金额值对象
/// </summary>
public record Money
{
    public decimal TaxIncluded { get; }      // 含税金额（2位小数）
    public decimal TaxExcluded { get; }      // 不含税金额（10位小数）
    public int TaxRate { get; }              // 税率（整数百分比）
    
    private Money(decimal taxIncluded, decimal taxExcluded, int taxRate)
    {
        TaxIncluded = taxIncluded;
        TaxExcluded = taxExcluded;
        TaxRate = taxRate;
    }
    
    public static Money Create(decimal taxIncluded, int taxRate)
    {
        Guard.Against.Negative(taxIncluded, nameof(taxIncluded));
        Guard.Against.OutOfRange(taxRate, nameof(taxRate), 0, 100);
        
        var taxExcluded = Math.Round(taxIncluded / (1 + taxRate / 100m), 10);
        return new Money(
            Math.Round(taxIncluded, 2),
            taxExcluded,
            taxRate
        );
    }
    
    public static Money Zero => new(0, 0, 0);
    
    public static Money operator +(Money a, Money b) => /* 实现 */;
    public static Money operator -(Money a, Money b) => /* 实现 */;
}
```

### 4.3 应用服务模板

```csharp
namespace Inno.CorePlatform.Finance.Application.Commands.ClaimReceivable;

public record ClaimReceivableCommand(
    ReceivableId ReceivableId,
    Money Amount,
    ClaimType Type
);

public class ClaimReceivableHandler : ICommandHandler<ClaimReceivableCommand, Result>
{
    private readonly IReceivableRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public ClaimReceivableHandler(
        IReceivableRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result> Handle(ClaimReceivableCommand command, CancellationToken ct)
    {
        // 1. 加载聚合
        var receivable = await _repository.GetByIdAsync(command.ReceivableId, ct);
        if (receivable is null)
            return Result.Fail("应收单不存在");
        
        // 2. 执行业务逻辑
        receivable.Claim(command.Amount, command.Type);
        
        // 3. 持久化
        await _unitOfWork.SaveChangesAsync(ct);
        
        return Result.Ok();
    }
}
```

---

## 五、配置规范

### 5.1 appsettings.json 结构

```json
{
  "App": {
    "Name": "finance-backend",
    "Version": "1.0.0"
  },
  "ConnectionStrings": {
    "Default": "Server=...;Database=Finance;..."
  },
  "Dapr": {
    "PubSubName": "pubsub",
    "StateStoreName": "statestore"
  },
  "Kingdee": {
    "BaseUrl": "https://kingdee.example.com",
    "AppId": "xxx",
    "AppSecret": "xxx"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    }
  }
}
```

### 5.2 依赖注入注册

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// 领域服务
builder.Services.AddScoped<IWriteOffService, WriteOffService>();

// 应用服务
builder.Services.AddScoped<IClaimService, ClaimService>();

// 基础设施
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IReceivableRepository, ReceivableRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Dapr
builder.Services.AddDaprClient();

var app = builder.Build();
```

---

## 六、API规范

### 6.1 路由约定

| 宿主 | 基础路径 | 用途 |
|------|----------|------|
| Backend | `/api/v1/{resource}` | 内部服务调用 |
| WebApi | `/api/v1/{resource}` | 对外能力接口 |

### 6.2 响应格式

```csharp
// 成功响应
public record ApiResponse<T>
{
    public bool Success { get; init; } = true;
    public T? Data { get; init; }
    public string? Message { get; init; }
}

// 错误响应
public record ApiErrorResponse
{
    public bool Success { get; init; } = false;
    public string Code { get; init; }
    public string Message { get; init; }
    public object? Details { get; init; }
}
```

### 6.3 HTTP状态码

| 状态码 | 场景 |
|--------|------|
| 200 | 成功 |
| 201 | 创建成功 |
| 400 | 请求参数错误 |
| 404 | 资源不存在 |
| 409 | 业务冲突（如状态不允许） |
| 500 | 服务器错误 |

---

## 七、测试规范

### 7.1 测试命名

```csharp
// 格式：{方法名}_{场景}_{期望结果}
[Fact]
public void Claim_WithSettledReceivable_ShouldThrowBusinessException()
```

### 7.2 测试覆盖率要求

| 层 | 最低覆盖率 |
|-----|-----------|
| Domain | 90% |
| Application | 80% |
| Infrastructure | 60% |

---

## 八、检查清单

AI生成代码前必须确认：

- [ ] 代码放置在正确的层和目录
- [ ] 命名空间符合约定
- [ ] 聚合根使用工厂方法创建
- [ ] 值对象是不可变的
- [ ] 领域事件在状态变更时发布
- [ ] 依赖注入正确注册
- [ ] 单元测试覆盖业务规则
