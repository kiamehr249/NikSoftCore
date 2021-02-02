using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.SystemBase.Service
{
    public class MenuMap : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Menus");

            builder.HasOne(x => x.MenuCategory)
                .WithMany(x => x.Menus)
                .HasForeignKey(x => x.CategoryId).IsRequired(true);

            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Childs)
                .HasForeignKey(x => x.ParentId).IsRequired(false);
        }
    }
}