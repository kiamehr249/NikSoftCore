using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.FormBuilder.Service
{
    public class BourseUserMap : IEntityTypeConfiguration<FormUser>
    {
        public void Configure(EntityTypeBuilder<FormUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Users");
        }
    }
}