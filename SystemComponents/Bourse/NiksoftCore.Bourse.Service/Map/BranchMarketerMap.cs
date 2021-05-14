using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class BranchMarketerMap : IEntityTypeConfiguration<BranchMarketer>
    {
        public void Configure(EntityTypeBuilder<BranchMarketer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_BranchMarketers");

            builder.HasOne(x => x.User)
                .WithMany(x => x.BranchMarketers)
                .HasForeignKey(x => x.UserId).IsRequired(true);

            builder.HasOne(x => x.Branch)
                .WithMany(x => x.BranchMarketers)
                .HasForeignKey(x => x.BranchId).IsRequired(true);


        }
    }
}