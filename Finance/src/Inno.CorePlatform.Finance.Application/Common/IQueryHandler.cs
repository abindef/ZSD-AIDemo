namespace Inno.CorePlatform.Finance.Application.Common;

/// <summary>
/// 查询处理器接口
/// </summary>
public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}
