using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class BranchUserMap : IEntityTypeConfiguration<BranchUser>
    {
        public void Configure(EntityTypeBuilder<BranchUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_BranchUsers");

            builder.HasOne(x => x.User)
                .WithMany(x => x.BranchUsers)
                .HasForeignKey(x => x.UserId).IsRequired(true);

            builder.HasOne(x => x.Branch)
                .WithMany(x => x.BranchUsers)
                .HasForeignKey(x => x.BranchId).IsRequired(true);
        }
    }
}