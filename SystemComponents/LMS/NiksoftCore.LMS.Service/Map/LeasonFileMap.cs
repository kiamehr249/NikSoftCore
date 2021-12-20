using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.LMS.Service
{
    public class LeasonFileMap : IEntityTypeConfiguration<LeasonFile>
    {
        public void Configure(EntityTypeBuilder<LeasonFile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("LeasonFiles");

            builder.HasOne(x => x.Leason)
                .WithMany(x => x.LeasonFiles)
                .HasForeignKey(x => x.LeasonId);
        }
    }
}