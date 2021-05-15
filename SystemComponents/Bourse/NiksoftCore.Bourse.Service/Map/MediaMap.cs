using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class MediaMap : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_Medias");

            builder.HasOne(x => x.User)
                .WithMany(x => x.Medias)
                .HasForeignKey(x => x.UserId).IsRequired(true);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Medias)
                .HasForeignKey(x => x.CategoryId).IsRequired(true);
        }
    }
}