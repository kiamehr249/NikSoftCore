using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class SettingMap : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_Settings");

            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Childs)
                .HasForeignKey(x => x.ParentId).IsRequired(false);
        }
    }
}