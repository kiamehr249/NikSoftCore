using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class BranchConsultantMap : IEntityTypeConfiguration<BranchConsultant>
    {
        public void Configure(EntityTypeBuilder<BranchConsultant> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_BranchConsultants");

            builder.HasOne(x => x.User)
                .WithMany(x => x.BranchConsultants)
                .HasForeignKey(x => x.UserId).IsRequired(true);

            builder.HasOne(x => x.Branch)
                .WithMany(x => x.BranchConsultants)
                .HasForeignKey(x => x.BranchId).IsRequired(true);


        }
    }
}