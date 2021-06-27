using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.FormBuilder.Service
{
    public class FormControlMap : IEntityTypeConfiguration<FormControl>
    {
        public void Configure(EntityTypeBuilder<FormControl> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("FB_FormControls");

            builder.HasOne(x => x.Form)
                .WithMany(x => x.FormControls)
                .HasForeignKey(x => x.FormId).IsRequired(true);
        }
    }
}