using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class BourseUserMap : IEntityTypeConfiguration<BourseUser>
    {
        public void Configure(EntityTypeBuilder<BourseUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Users");
        }
    }
}