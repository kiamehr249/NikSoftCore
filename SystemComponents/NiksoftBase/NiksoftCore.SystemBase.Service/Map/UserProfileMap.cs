using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.SystemBase.Service
{
    public class UserProfileMap : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("UserProfiles");

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserProfiles)
                .HasForeignKey(x => x.UserId).IsRequired(true);
        }
    }
}