using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class BranchAdLeaderMap : IEntityTypeConfiguration<BranchAdLeader>
    {
        public void Configure(EntityTypeBuilder<BranchAdLeader> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_BranchAdLeaders");

            builder.HasOne(x => x.User)
                .WithMany(x => x.BranchAdLeaders)
                .HasForeignKey(x => x.UserId).IsRequired(true);

            builder.HasOne(x => x.Branch)
                .WithMany(x => x.BranchAdLeaders)
                .HasForeignKey(x => x.BranchId).IsRequired(true);
        }
    }
}