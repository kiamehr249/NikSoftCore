using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.FormBuilder.Service
{
    public class ControlItemMap : IEntityTypeConfiguration<ControlItem>
    {
        public void Configure(EntityTypeBuilder<ControlItem> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("FB_ControlItems");

            builder.HasOne(x => x.FormControl)
                .WithMany(x => x.ControlItems)
                .HasForeignKey(x => x.ControlId).IsRequired(true);

            builder.HasOne(x => x.Form)
                .WithMany(x => x.ControlItems)
                .HasForeignKey(x => x.FormId).IsRequired(true);
        }
    }
}