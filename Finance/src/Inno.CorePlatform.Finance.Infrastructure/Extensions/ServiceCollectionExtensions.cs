using Inno.CorePlatform.Finance.Domain.Repositories;
using Inno.CorePlatform.Finance.Infrastructure.Persistence;
using Inno.CorePlatform.Finance.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inno.CorePlatform.Finance.Infrastructure.Extensions;

/// <summary>
/// 基础设施层服务注册扩展
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加基础设施层服务
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 数据库上下文
        services.AddDbContext<FinanceDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));

        // 仓储       
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FinanceDbContext>());

        return services;
    }
}
