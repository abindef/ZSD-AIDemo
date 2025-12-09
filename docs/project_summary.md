# dotnet-common 项目详细文档

## 项目概述
`dotnet-common` 是一个基础类库项目,为基于 .NET Core 的平台提供支持。它提供了通用工具、基类和基础设施抽象,以促进微服务和分布式应用程序的开发。该项目深度集成了领域驱动设计(DDD)和云原生技术(如 Dapr 和 Kubernetes)。

---

## 项目结构

```
dotnet-common/
├── src/                          # 源代码目录
│   ├── Inno.CorePlatform.Common/           # 核心通用库
│   ├── Inno.CorePlatform.ServiceClient/    # 服务客户端库
│   ├── Inno.CorePlatform.TestCommon/       # 测试通用库
│   └── Inno.CorePlatform.TestCommon.NUnit/ # NUnit测试支持
├── test/                         # 测试目录
│   ├── Inno.CorePlatform.Common.UnitTest/
│   ├── Inno.CorePlatform.ServiceClientUnitTest/
│   └── TestWebApp/
└── demo/                         # 示例项目
```

---

## 核心组件详解

### 1. DDD (领域驱动设计) 模块

#### 1.1 Entity (实体)
**设计理念**: 实体是具有唯一标识符的领域对象,支持领域事件机制。

**核心实现**:
```csharp
public abstract class Entity<TId> : IEntityWithEvent
    where TId : notnull, IEquatable<TId>
{
    public TId Id { get; set; }
    
    // 领域事件集合 - 使用并发队列确保线程安全
    private readonly ConcurrentQueue<IDomainEvent> _domainEvents = new();
    
    public IProducerConsumerCollection<IDomainEvent> DomainEvents => _domainEvents;
    
    // 添加领域事件
    protected void AddEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Enqueue(domainEvent);
    }
    
    // 清除领域事件
    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
    
    // 使用 MassTransit 生成分布式唯一 ID
    public static Guid NewIdGuid()
    {
        return MassTransit.NewId.NextGuid();
    }
}
```

**关键特性**:
- 泛型 ID 支持,可使用 `Guid`、`int`、`long` 等类型
- 内置领域事件机制,支持事件驱动架构
- 使用 `[BsonIgnore]`、`[NotMapped]` 等特性确保事件不被持久化
- 集成 MassTransit 的分布式 ID 生成器

#### 1.2 ValueObject (值对象)
**设计理念**: 值对象通过属性值而非标识符来定义相等性。

**核心实现**:
```csharp
public abstract class ValueObject : IEqualityComparer<ValueObject>, IValueObject
{
    // 通过反射比较所有属性和字段
    public bool Equals(ValueObject? x, ValueObject? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        
        return x.GetProperties().All(p => x.PropertiesAreEqual(y, p))
            && x.GetFields().All(f => x.FieldsAreEqual(y, f));
    }
    
    // 基于所有属性和字段计算哈希码
    public int GetHashCode(ValueObject obj)
    {
        unchecked
        {
            int hash = 17;
            foreach (var prop in obj.GetProperties())
            {
                var value = prop.GetValue(this, null);
                hash = hash * 23 + (value?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }
    
    // 业务规则验证
    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }
}
```

**关键特性**:
- 自动通过反射比较所有属性和字段
- 支持 `[IgnoreMember]` 特性排除特定成员
- 内置业务规则验证机制

#### 1.3 Repository (仓储)
**设计理念**: 提供聚合根的持久化抽象,支持 CRUD 操作和工作单元模式。

**接口定义**:
```csharp
public interface IRepositorySupportCrud<TAggregateRoot, TId> : IRepository
    where TId : notnull, IEquatable<TId>
    where TAggregateRoot : Entity<TId>, IAggregateRoot
{
    Task<int> AddAsync(TAggregateRoot root);
    Task<int> DeleteAsync(TId id);
    Task<TAggregateRoot?> GetAsync(TId id);
    Task<int> UpdateAsync(TAggregateRoot root);
}

public interface IRepositorySupportUow : IRepository
{
    // 如果为 True,仓储不应自己调用提交操作
    bool UowJoined { get; set; }
}
```

**使用场景**:
- `IRepositorySupportCrud`: 基础 CRUD 操作
- `IRepositorySupportUow`: 支持工作单元模式,用于事务管理
- `IRepositorySupportCrudAndUow`: 同时支持两者

#### 1.4 ApplicationService (应用服务)
**设计理念**: 应用服务层协调领域对象,处理业务用例。

**核心实现**:
```csharp
// 命令应用服务 - 处理写操作
public abstract class CommandAppService : ICommandAppService
{
    protected readonly IDomainEventDispatcher? DomainEventDispatcher;
    private readonly IAppServiceContextAccessor? contextAccessor;
    
    protected CommandAppService(
        IDomainEventDispatcher? deDispatcher, 
        IAppServiceContextAccessor? contextAccessor)
    {
        this.DomainEventDispatcher = deDispatcher;
        this.contextAccessor = contextAccessor;
    }
    
    protected AppServiceContext Context => 
        contextAccessor != null ? contextAccessor.Get() : AppServiceContext.Empty;
}

// 查询应用服务 - 处理读操作
public abstract class QueryAppService : IQueryAppService
{
    private readonly IAppServiceContextAccessor? contextAccessor;
    
    protected AppServiceContext Context => 
        contextAccessor != null ? contextAccessor.Get() : AppServiceContext.Empty;
}
```

**CQRS 分离**:
- `CommandAppService`: 处理命令(写操作),集成领域事件分发器
- `QueryAppService`: 处理查询(读操作),返回 `IQueryable<TDto>`

#### 1.5 DomainEvent (领域事件)
**设计理念**: 领域事件用于解耦业务逻辑,实现事件驱动架构。

**核心实现**:
```csharp
public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        this.OccurredOn = DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset OccurredOn { get; }
}

// 领域事件分发器
public interface IDomainEventDispatcher
{
    Task Dispatch(IDomainEvent devent);
    Task DispathForEntity(IEntityWithEvent entity);
}
```

**使用流程**:
1. 实体内部调用 `AddEvent()` 添加领域事件
2. 应用服务在保存实体后调用 `DomainEventDispatcher.DispathForEntity()`
3. 事件处理器异步处理领域事件

---

### 2. EntityFramework 模块

#### EfBaseRepository (EF Core 仓储基类)
**设计理念**: 使用 EF Core 持久化聚合,支持聚合根与持久化对象(PO)的映射。

**核心实现**:
```csharp
public abstract class EfBaseRepository<TId, TAggregateRoot, TPO> 
    : IRepositorySupportCrudAndUow<TAggregateRoot, TId>
    where TId : struct, IEquatable<TId>
    where TAggregateRoot : Entity<TId>, IAggregateRoot
{
    protected readonly DbContext dbContext;
    
    public bool UowJoined { get; set; }
    
    // 添加聚合 - 使用 Mapster 映射
    public virtual async Task<int> AddAsync(TAggregateRoot root)
    {
        var po = root.Adapt<TPO>();
        dbContext.Add(po);
        
        if (UowJoined) return 0; // 工作单元模式下不立即提交
        
        return await dbContext.SaveChangesAsync();
    }
    
    // 获取聚合 - 需子类实现 Include 逻辑
    public virtual async Task<TAggregateRoot?> GetAsync(TId id)
    {
        var po = await GetPoWithIncludeAsync(id);
        if (po == null) return default;
        return po.Adapt<TAggregateRoot>();
    }
    
    // 子类实现具体的 Include 逻辑
    protected abstract Task<TPO> GetPoWithIncludeAsync(TId id);
    
    // 删除聚合
    public virtual async Task<int> DeleteAsync(TId id)
    {
        dbContext.Remove(CreateDeletingPo(id));
        if (UowJoined) return 0;
        return await dbContext.SaveChangesAsync();
    }
    
    protected abstract TPO CreateDeletingPo(TId id);
    public abstract Task<int> UpdateAsync(TAggregateRoot root);
}
```

**关键特性**:
- 使用 Mapster 进行聚合根与持久化对象的映射
- 支持工作单元模式 (`UowJoined`)
- 子类需实现 `GetPoWithIncludeAsync` 以加载关联实体

**使用示例**:
```csharp
public class OrderRepository : EfBaseRepository<Guid, Order, OrderPO>
{
    public OrderRepository(OrderDbContext dbContext) : base(dbContext) { }
    
    protected override async Task<OrderPO> GetPoWithIncludeAsync(Guid id)
    {
        return await dbContext.Set<OrderPO>()
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);
    }
    
    protected override OrderPO CreateDeletingPo(Guid id)
    {
        return new OrderPO { Id = id };
    }
    
    public override async Task<int> UpdateAsync(Order root)
    {
        var po = root.Adapt<OrderPO>();
        dbContext.Update(po);
        if (UowJoined) return 0;
        return await dbContext.SaveChangesAsync();
    }
}
```

---

### 3. MongoDB 模块

#### MongoBaseRepository (MongoDB 仓储基类)
**设计理念**: 使用 MongoDB 持久化聚合,直接存储聚合根对象。

**核心实现**:
```csharp
public abstract class MongoBaseRepository<TId, TAggregateRoot> 
    : IRepositorySupportCrudAndUow<TAggregateRoot, TId>
    where TId : notnull, IEquatable<TId>
    where TAggregateRoot : Entity<TId>, IAggregateRoot
{
    protected readonly IMongoDatabase db;
    protected readonly IMongoCollection<TAggregateRoot> collection;
    
    protected MongoBaseRepository(IMongoDatabase db)
    {
        this.db = db;
        this.collection = db.GetCollection<TAggregateRoot>(CollectionName);
    }
    
    protected abstract string CollectionName { get; }
    
    public bool UowJoined { get; set; }
    
    public async Task<int> AddAsync(TAggregateRoot root)
    {
        await collection.InsertOneAsync(root);
        return 1;
    }
    
    public async Task<int> DeleteAsync(TId id)
    {
        var result = await collection.DeleteOneAsync(p => p.Id.Equals(id));
        return (int)result.DeletedCount;
    }
    
    public async Task<TAggregateRoot?> GetAsync(TId id)
    {
        return await collection.Find(p => p.Id.Equals(id)).FirstOrDefaultAsync();
    }
    
    public async Task<int> UpdateAsync(TAggregateRoot root)
    {
        var result = await collection.ReplaceOneAsync(
            p => p.Id.Equals(root.Id), root);
        return (int)result.ModifiedCount;
    }
}
```

**关键特性**:
- 直接存储聚合根,无需 PO 映射
- 使用 MongoDB 的原生 API
- 支持工作单元模式

**使用示例**:
```csharp
public class ProductRepository : MongoBaseRepository<Guid, Product>
{
    public ProductRepository(IMongoDatabase db) : base(db) { }
    
    protected override string CollectionName => "Products";
}
```

---

### 4. Clients 模块 (服务间通信)

#### BaseWebApiClient (基于 Dapr 的 Web API 客户端)
**设计理念**: 使用 Dapr 进行服务间通信,支持服务发现和负载均衡。

**核心实现**:
```csharp
public abstract class BaseWebApiClient<T> : BaseClient<T>
{
    protected readonly DaprClient _daprClient;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    
    protected abstract string GetAppId();
    
    // 调用远程方法 - 无参数
    protected async Task<TResponse> InvokeMethodAsync<TResponse>(
        string methodName, 
        RequestMethodEnum defaultMethod = RequestMethodEnum.POST)
    {
        try
        {
            var request = _daprClient.CreateInvokeMethodRequest(GetAppId(), methodName);
            return await SendRequestAsync<TResponse>(request, defaultMethod);
        }
        catch (ApplicationException ex)
        {
            _logger.LogError(ex, "Dapr接口调用异常。");
            throw new ApplicationException(ex.Message);
        }
    }
    
    // 调用远程方法 - 带参数对象
    protected async Task<TResponse> InvokeMethodWithQueryObjectAsync<TRequest, TResponse>(
        TRequest inputParam, 
        string methodName, 
        RequestMethodEnum defaultMethod = RequestMethodEnum.POST)
    {
        try
        {
            var paramJson = inputParam != null ? inputParam.ToJson() : "";
            _logger.LogInformation($"请求:【AppID】{GetAppId()}【MethodName】{methodName} 【参数对象】{paramJson}");
            
            var request = _daprClient.CreateInvokeMethodRequest(GetAppId(), methodName, inputParam);
            return await SendRequestAsync<TResponse>(request, defaultMethod);
        }
        catch (ApplicationException ex)
        {
            _logger.LogError(ex, "Dapr接口调用异常。");
            throw new ApplicationException(ex.Message);
        }
    }
    
    // 统一响应处理
    protected virtual async Task<TResponse> DefaultResponseAsync<TResponse>(
        HttpRequestMessage request)
    {
        var res = await _daprClient.InvokeMethodAsync<BaseResponseData<TResponse>>(request);
        if (res.Code != CodeStatusEnum.Success)
        {
            _logger.LogError($"Dapr调用失败");
            _logger.LogError($"请求内容 {request.ToJson()} ");
            _logger.LogError($"返回结果 {res.ToJson()} ");
            throw new ApplicationException($"{res.Message}");
        }
        return res.Data;
    }
    
    // 获取当前登录用户信息
    protected (string UserId, string UserName) GetCurrentLoginUser()
    {
        if (_httpContextAccessor.HttpContext == null)
            return ("", "");
            
        var userId = _httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(t => t.Type == "sub")?.Value;
        var userName = _httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(t => t.Type == "upn")?.Value;
            
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userId))
        {
            // 兼容 WebAPI 请求
            userId = _httpContextAccessor.HttpContext.Request.Headers["X-Inno-UserId"];
            userName = _httpContextAccessor.HttpContext.Request.Headers["X-Inno-UserName"];
            return (userId ?? "", userName ?? "");
        }
        return (userId, userName);
    }
}
```

**关键特性**:
- 基于 Dapr 的服务调用,支持服务发现
- 自动传递用户上下文 (`X-Inno-UserId`, `X-Inno-UserName`)
- 统一的错误处理和日志记录
- 支持 POST、GET、PUT、DELETE 等 HTTP 方法

**使用示例**:
```csharp
public class OrderServiceClient : BaseWebApiClient<OrderServiceClient>
{
    public OrderServiceClient(
        DaprClient daprClient, 
        ILogger<OrderServiceClient> logger, 
        IHttpContextAccessor httpContextAccessor) 
        : base(daprClient, logger, httpContextAccessor)
    {
    }
    
    protected override string GetAppId() => "order-service";
    
    public async Task<OrderDto> GetOrderAsync(Guid orderId)
    {
        return await InvokeMethodWithQueryObjectAsync<GetOrderRequest, OrderDto>(
            new GetOrderRequest { OrderId = orderId },
            "api/orders/get",
            RequestMethodEnum.POST
        );
    }
}
```

---

### 5. Query 模块 (查询服务)

#### BaseQueryService (查询服务基类)
**设计理念**: 提供通用的查询、分页、排序、过滤功能。

**核心功能**:
- 分页查询
- 多字段排序
- 动态过滤
- 字段选择
- 聚合查询

**使用场景**:
```csharp
public class OrderQueryService : BaseQueryService<OrderDto, Guid>
{
    public IQueryable<OrderDto> GetListQueryable()
    {
        // 返回可查询的订单列表
    }
    
    public IQueryable<OrderDto> GetItemQueryable(Guid key)
    {
        // 返回单个订单
    }
}
```

---

### 6. Logging 模块

#### 日志增强功能
**核心组件**:
- `ClaimEnricher`: 从 HttpContext 提取用户信息添加到日志
- `ClsFormatter`: 自定义日志格式化器
- `ClsJsonConsoleFormatter`: JSON 格式的控制台日志输出

**关键特性**:
- 结构化日志
- 用户上下文自动注入
- 支持多种日志输出格式

---

## 技术栈

| 类别 | 技术 |
|------|------|
| **框架** | .NET Core/5/6/7/8 |
| **架构模式** | DDD、CQRS、微服务 |
| **云原生** | Dapr、Kubernetes |
| **数据库** | SQL Server (EF Core)、MongoDB |
| **对象映射** | Mapster |
| **消息/事件** | MassTransit |
| **日志** | Serilog |

---

## 使用指南

### 1. 创建聚合根
```csharp
public class Order : Entity<Guid>, IAggregateRoot
{
    public string OrderNumber { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();
    
    public void AddItem(Product product, int quantity)
    {
        var item = new OrderItem(product, quantity);
        Items.Add(item);
        
        // 添加领域事件
        AddEvent(new OrderItemAddedEvent(Id, item.ProductId, quantity));
    }
}
```

### 2. 实现仓储
```csharp
// EF Core 仓储
public class OrderRepository : EfBaseRepository<Guid, Order, OrderPO>
{
    // 实现必要的抽象方法
}

// MongoDB 仓储
public class ProductRepository : MongoBaseRepository<Guid, Product>
{
    protected override string CollectionName => "Products";
}
```

### 3. 创建应用服务
```csharp
public class OrderAppService : CommandAppService
{
    private readonly IOrderRepository _orderRepository;
    
    public OrderAppService(
        IOrderRepository orderRepository,
        IDomainEventDispatcher eventDispatcher,
        IAppServiceContextAccessor contextAccessor)
        : base(eventDispatcher, contextAccessor)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<Guid> CreateOrderAsync(CreateOrderCommand command)
    {
        var order = new Order(command.OrderNumber);
        await _orderRepository.AddAsync(order);
        
        // 分发领域事件
        await DomainEventDispatcher.DispathForEntity(order);
        
        return order.Id;
    }
}
```

### 4. 服务间调用
```csharp
public class InventoryServiceClient : BaseWebApiClient<InventoryServiceClient>
{
    protected override string GetAppId() => "inventory-service";
    
    public async Task<bool> ReserveStockAsync(Guid productId, int quantity)
    {
        return await InvokeMethodWithQueryObjectAsync<ReserveStockRequest, bool>(
            new ReserveStockRequest { ProductId = productId, Quantity = quantity },
            "api/inventory/reserve",
            RequestMethodEnum.POST
        );
    }
}
```

---

## 设计模式总结

| 模式 | 应用场景 |
|------|----------|
| **Repository** | 聚合根持久化抽象 |
| **Unit of Work** | 事务管理 |
| **Domain Event** | 解耦业务逻辑 |
| **CQRS** | 读写分离 |
| **Adapter** | 聚合根与持久化对象映射 |
| **Template Method** | 仓储基类定义通用流程 |
| **Strategy** | 查询策略 |

---

## 最佳实践

1. **聚合设计**: 保持聚合边界清晰,避免过大的聚合
2. **领域事件**: 使用领域事件实现跨聚合的业务逻辑
3. **仓储模式**: 每个聚合根对应一个仓储
4. **工作单元**: 在应用服务层管理事务边界
5. **服务调用**: 使用 Dapr 客户端进行服务间通信
6. **日志记录**: 使用结构化日志,包含用户上下文

---

## 注意事项

1. **领域事件不持久化**: 使用 `[BsonIgnore]`、`[NotMapped]` 等特性
2. **并发安全**: 领域事件使用 `ConcurrentQueue`
3. **ID 生成**: 使用 `Entity.NewIdGuid()` 生成分布式唯一 ID
4. **对象映射**: 使用 Mapster 进行高性能映射
5. **用户上下文**: 通过 `AppServiceContext` 访问当前用户信息
