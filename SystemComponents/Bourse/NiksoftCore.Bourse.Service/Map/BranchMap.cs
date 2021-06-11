using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class BranchMap : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_Branchs");

            builder.HasOne(x => x.BranchArea)
                .WithMany(x => x.Branches)
                .HasForeignKey(x => x.AreaId).IsRequired(true);
        }
    }
}