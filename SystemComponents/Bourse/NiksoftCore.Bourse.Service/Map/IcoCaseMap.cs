using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class IcoCaseMap : IEntityTypeConfiguration<IcoCase>
    {
        public void Configure(EntityTypeBuilder<IcoCase> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("T_IcoCases");
        }
    }
}