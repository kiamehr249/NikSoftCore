using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.LMS.Service
{
    public class CalendarDayMap : IEntityTypeConfiguration<CalendarDay>
    {
        public void Configure(EntityTypeBuilder<CalendarDay> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("CalendarDays");

            builder.HasOne(x => x.Calendar)
                .WithMany(x => x.CalendarDays)
                .HasForeignKey(x => x.CalendarId);
        }
    }
}