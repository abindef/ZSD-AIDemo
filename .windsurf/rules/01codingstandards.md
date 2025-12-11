---
trigger: manual
---


# 编码规范 (Coding Standards)

<directory_structure>
  Root: `src/`
  - `Inno.CorePlatform.Finance.Domain`: 聚合根, 值对象, 领域服务
  - `Inno.CorePlatform.Finance.Application`: Commands, Queries, EventHandlers
  - `Inno.CorePlatform.Finance.Infrastructure`: Persistence, Adapters
  - `Inno.CorePlatform.Finance.Backend`: 前端对接 API
  - `Inno.CorePlatform.Finance.WebApi`: 微服务间 API
</directory_structure>

<coding_rules>
  <general>
    - **命名空间**: 必须严格匹配物理文件夹路径。
    - **异步**: 所有 I/O 操作必须使用 `async/await`。
    - **API 响应**: 所有 Controller 必须返回 `ApiResponse<T>` 包装器。
  </general>

  <ddd_patterns>
    <template name="AggregateRoot">
      public class Receivable : Entity<ReceivableId>, IAggregateRoot {
          // 1. 私有构造函数 (ORM需要)
          private Receivable() { } 
          
          // 2. 属性只读，禁止 public setter
          public Money Amount { get; private set; }
          
          // 3. 静态工厂方法创建
          public static Receivable Create(ReceivableId id, Money amount) {
              Guard.Against.Null(id);
              return new Receivable { Id = id, Amount = amount };
          }
          
          // 4. 业务行为方法
          public void Claim(Money amount) {
              // 业务校验
              if (IsSettled) throw new BusinessException("已核销");
              // 状态变更
              // 发布事件
              AddDomainEvent(new ReceivableClaimedEvent(Id));
          }
      }
    </template>

    <template name="ValueObject">
      public record Money {
          public decimal Amount { get; }
          // 强制不可变性
          private Money(decimal amount) => Amount = amount;
          public static Money Create(decimal amount) => new(amount);
      }
    </template>
  </ddd_patterns>
</coding_rules>
