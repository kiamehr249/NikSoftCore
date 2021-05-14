using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class BranchMasterMap : IEntityTypeConfiguration<BranchMaster>
    {
        public void Configure(EntityTypeBuilder<BranchMaster> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_BranchMasters");

            builder.HasOne(x => x.User)
                .WithMany(x => x.BranchMasters)
                .HasForeignKey(x => x.UserId).IsRequired(true);

            builder.HasOne(x => x.Branch)
                .WithMany(x => x.BranchMasters)
                .HasForeignKey(x => x.BranchId).IsRequired(true);


        }
    }
}