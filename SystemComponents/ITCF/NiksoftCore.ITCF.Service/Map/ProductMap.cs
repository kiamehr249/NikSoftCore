using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.ITCF.Service
{
    public class ProductMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("ITCF_Products");

            builder.HasOne(x => x.Business)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.BusinessId).IsRequired(false);

            builder.HasOne(x => x.BusinessCategory)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId).IsRequired(false);


        }
    }
}