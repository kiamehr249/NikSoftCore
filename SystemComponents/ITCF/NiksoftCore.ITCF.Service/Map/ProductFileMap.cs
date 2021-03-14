using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.ITCF.Service
{
    public class ProductFileMap : IEntityTypeConfiguration<ProductFile>
    {
        public void Configure(EntityTypeBuilder<ProductFile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("ITCF_ProductFiles");

            builder.HasOne(x => x.Product)
                .WithMany(x => x.Files)
                .HasForeignKey(x => x.ProductId).IsRequired(true);
        }
    }
}