using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.FormBuilder.Service
{
    public class UserProfileMap : IEntityTypeConfiguration<FormUserProfile>
    {
        public void Configure(EntityTypeBuilder<FormUserProfile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("UserProfiles");

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserProfiles)
                .HasForeignKey(x => x.UserId).IsRequired(true);
        }
    }
}