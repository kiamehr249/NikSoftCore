using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class MediaCategoryMap : IEntityTypeConfiguration<MediaCategory>
    {
        public void Configure(EntityTypeBuilder<MediaCategory> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_MediaCategories");
        }
    }
}