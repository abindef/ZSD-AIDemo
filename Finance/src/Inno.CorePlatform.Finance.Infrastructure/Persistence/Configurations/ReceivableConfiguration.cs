using Inno.CorePlatform.Finance.Domain.Aggregates.Receivables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inno.CorePlatform.Finance.Infrastructure.Persistence.Configurations;

/// <summary>
/// 应收单EF配置
/// </summary>
public class ReceivableConfiguration : IEntityTypeConfiguration<Receivable>
{
    public void Configure(EntityTypeBuilder<Receivable> builder)
    {
        builder.ToTable("Receivables");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => ReceivableId.From(value));

        builder.Property(x => x.CustomerId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.DueDate)
            .IsRequired();

        // 值对象配置 - Money
        builder.OwnsOne(x => x.Amount, money =>
        {
            money.Property(m => m.TaxIncluded)
                .HasColumnName("TaxIncludedAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.TaxExcluded)
                .HasColumnName("TaxExcludedAmount")
                .HasPrecision(18, 10);

            money.Property(m => m.TaxRate)
                .HasColumnName("TaxRate");
        });

        // 值对象配置 - PaymentTerm
        builder.OwnsOne(x => x.PaymentTerm, term =>
        {
            term.Property(t => t.Days)
                .HasColumnName("PaymentTermDays");

            term.Property(t => t.Description)
                .HasColumnName("PaymentTermDescription")
                .HasMaxLength(100);
        });

        // 认款记录配置
        builder.OwnsMany(x => x.Claims, claim =>
        {
            claim.ToTable("Claims");

            claim.WithOwner().HasForeignKey("ReceivableId");

            claim.Property<Guid>("Id");
            claim.HasKey("Id");

            claim.Property(c => c.Type)
                .IsRequired();

            claim.Property(c => c.ClaimedAt)
                .IsRequired();

            claim.Property(c => c.Remark)
                .HasMaxLength(500);

            claim.OwnsOne(c => c.Amount, money =>
            {
                money.Property(m => m.TaxIncluded)
                    .HasColumnName("TaxIncludedAmount")
                    .HasPrecision(18, 2);

                money.Property(m => m.TaxExcluded)
                    .HasColumnName("TaxExcludedAmount")
                    .HasPrecision(18, 10);

                money.Property(m => m.TaxRate)
                    .HasColumnName("TaxRate");
            });
        });
    }
}
