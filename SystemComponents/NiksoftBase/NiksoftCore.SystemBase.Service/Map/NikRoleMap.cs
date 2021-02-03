using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.SystemBase.Service
{
    public class NikRoleMap : IEntityTypeConfiguration<NikRole>
    {
        public void Configure(EntityTypeBuilder<NikRole> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Roles");

        }
    }
}