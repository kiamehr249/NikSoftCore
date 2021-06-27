using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.FormBuilder.Service
{
    public class FormDataMap : IEntityTypeConfiguration<FormData>
    {
        public void Configure(EntityTypeBuilder<FormData> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("FB_FormDatas");

            builder.HasOne(x => x.Form)
                .WithMany(x => x.FormDatas)
                .HasForeignKey(x => x.FormId).IsRequired(true);

            builder.HasOne(x => x.User)
                .WithMany(x => x.FormDatas)
                .HasForeignKey(x => x.UserId).IsRequired(false);
        }
    }
}