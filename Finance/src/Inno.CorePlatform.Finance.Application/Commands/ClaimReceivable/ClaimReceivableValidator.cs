using FluentValidation;

namespace Inno.CorePlatform.Finance.Application.Commands.ClaimReceivable;

/// <summary>
/// 认款命令验证器
/// </summary>
public class ClaimReceivableValidator : AbstractValidator<ClaimReceivableCommand>
{
    public ClaimReceivableValidator()
    {
        RuleFor(x => x.ReceivableId)
            .NotEmpty()
            .WithMessage("应收单ID不能为空");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("认款金额必须大于0");

        RuleFor(x => x.TaxRate)
            .InclusiveBetween(0, 100)
            .WithMessage("税率必须在0-100之间");

        RuleFor(x => x.ClaimType)
            .NotEmpty()
            .WithMessage("认款类型不能为空");
    }
}
