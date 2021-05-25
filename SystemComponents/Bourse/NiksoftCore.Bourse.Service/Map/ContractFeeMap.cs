using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class ContractFeeMap : IEntityTypeConfiguration<ContractFee>
    {
        public void Configure(EntityTypeBuilder<ContractFee> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_ContractFees");

            builder.HasOne(x => x.Fee)
                .WithMany(x => x.ContractFees)
                .HasForeignKey(x => x.FeeId).IsRequired(true);

            builder.HasOne(x => x.Contract)
                .WithMany(x => x.ContractFees)
                .HasForeignKey(x => x.ContractId).IsRequired(true);
        }
    }
}