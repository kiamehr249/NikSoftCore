using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.LMS.Service
{
    public class LmsUserMap : IEntityTypeConfiguration<LmsUser>
    {
        public void Configure(EntityTypeBuilder<LmsUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Users");
        }
    }
}