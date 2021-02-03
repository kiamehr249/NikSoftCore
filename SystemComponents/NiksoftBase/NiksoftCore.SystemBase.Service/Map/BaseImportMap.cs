using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.SystemBase.Service
{
    public class BaseImportMap : IEntityTypeConfiguration<BaseImport>
    {
        public void Configure(EntityTypeBuilder<BaseImport> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("BaseImports");
        }
    }
}