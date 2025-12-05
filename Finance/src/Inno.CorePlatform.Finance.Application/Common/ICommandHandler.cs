namespace Inno.CorePlatform.Finance.Application.Common;

/// <summary>
/// 命令处理器接口
/// </summary>
public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}
