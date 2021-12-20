using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.LMS.Service
{
    public class LeasonMap : IEntityTypeConfiguration<Leason>
    {
        public void Configure(EntityTypeBuilder<Leason> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Leasons");

            builder.HasOne(x => x.CalendarDay)
                .WithMany(x => x.Leasons)
                .HasForeignKey(x => x.CalendarDayId);

            builder.HasOne(x => x.Course)
                .WithMany(x => x.Leasons)
                .HasForeignKey(x => x.CourseId);
        }
    }
}