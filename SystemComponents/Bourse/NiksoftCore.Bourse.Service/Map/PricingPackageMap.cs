using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class PricingPackageMap : IEntityTypeConfiguration<PricingPackage>
    {
        public void Configure(EntityTypeBuilder<PricingPackage> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("T_PricingPackages");
        }
    }
}