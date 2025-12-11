using Inno.CorePlatform.Finance.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inno.CorePlatform.Finance.Infrastructure.Persistence;

/// <summary>
/// 财务模块数据库上下文
/// </summary>
public class FinanceDbContext : DbContext, IUnitOfWork
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
