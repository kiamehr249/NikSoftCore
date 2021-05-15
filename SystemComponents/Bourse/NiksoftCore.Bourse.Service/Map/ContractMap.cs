using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class ContractMap : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_Contracts");

            builder.HasOne(x => x.User)
                .WithMany(x => x.Contracts)
                .HasForeignKey(x => x.UserId).IsRequired(true);

            builder.HasOne(x => x.Fee)
                .WithMany(x => x.Contracts)
                .HasForeignKey(x => x.FeeId).IsRequired(true);

            builder.HasOne(x => x.Branch)
                .WithMany(x => x.Contracts)
                .HasForeignKey(x => x.BranchId).IsRequired(true);
        }
    }
}