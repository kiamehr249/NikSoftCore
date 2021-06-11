using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class BranchAreaMap : IEntityTypeConfiguration<BranchArea>
    {
        public void Configure(EntityTypeBuilder<BranchArea> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_BranchAreas");
        }
    }
}