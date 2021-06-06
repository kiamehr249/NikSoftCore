using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class BaseTransactionMap : IEntityTypeConfiguration<BaseTransaction>
    {
        public void Configure(EntityTypeBuilder<BaseTransaction> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_BaseTransactions");
        }
    }
}