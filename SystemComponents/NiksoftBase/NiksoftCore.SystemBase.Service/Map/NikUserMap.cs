using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.SystemBase.Service
{
    public class NikUserMap : IEntityTypeConfiguration<NikUser>
    {
        public void Configure(EntityTypeBuilder<NikUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Users");
        }
    }
}