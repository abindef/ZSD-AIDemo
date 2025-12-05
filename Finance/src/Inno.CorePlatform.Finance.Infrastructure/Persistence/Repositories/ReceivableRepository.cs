using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
using Inno.CorePlatform.Finance.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inno.CorePlatform.Finance.Infrastructure.Persistence.Repositories;

/// <summary>
/// 应收单仓储实现
/// </summary>
public class ReceivableRepository : IReceivableRepository
{
    private readonly FinanceDbContext _context;

    public ReceivableRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<Receivable?> GetByIdAsync(ReceivableId id, CancellationToken cancellationToken = default)
    {
        return await _context.Receivables
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Receivable>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        return await _context.Receivables
            .Where(x => x.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Receivable receivable, CancellationToken cancellationToken = default)
    {
        await _context.Receivables.AddAsync(receivable, cancellationToken);
    }

    public Task UpdateAsync(Receivable receivable, CancellationToken cancellationToken = default)
    {
        _context.Receivables.Update(receivable);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Receivable receivable, CancellationToken cancellationToken = default)
    {
        _context.Receivables.Remove(receivable);
        return Task.CompletedTask;
    }
}
