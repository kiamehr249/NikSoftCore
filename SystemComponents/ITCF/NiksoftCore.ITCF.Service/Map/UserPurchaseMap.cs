using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.ITCF.Service
{
    public class UserPurchaseMap : IEntityTypeConfiguration<UserPurchase>
    {
        public void Configure(EntityTypeBuilder<UserPurchase> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("ITCF_UserPurchases");

            builder.HasOne(x => x.Product)
                .WithMany(x => x.Purchases)
                .HasForeignKey(x => x.ProductId).IsRequired(false);

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserPurchases)
                .HasForeignKey(x => x.UserId).IsRequired(false);
        }
    }
}