using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class BranchAdvertiserMap : IEntityTypeConfiguration<BranchAdvertiser>
    {
        public void Configure(EntityTypeBuilder<BranchAdvertiser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_BranchAdvertisers");

            builder.HasOne(x => x.User)
                .WithMany(x => x.BranchAdvertisers)
                .HasForeignKey(x => x.UserId).IsRequired(true);

            builder.HasOne(x => x.AdLeader)
                .WithMany(x => x.BAdrLeaders)
                .HasForeignKey(x => x.LeaderId).IsRequired(true);
        }
    }
}