using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.SystemBase.Service
{
    public class NikUserRoleMap : IEntityTypeConfiguration<NikUserRole>
    {
        public void Configure(EntityTypeBuilder<NikUserRole> builder)
        {
            builder.HasKey(x => x.UserId);
            builder.ToTable("UserRoles");

            //builder.HasOne(x => x.NikUser)
            //    .WithMany(x => x.NikUserRoles)
            //    .HasForeignKey(x => x.UserId).IsRequired(true);

            //builder.HasOne(x => x.NikRole)
            //    .WithMany(x => x.NikUserRoles)
            //    .HasForeignKey(x => x.RoleId).IsRequired(true);
        }
    }
}