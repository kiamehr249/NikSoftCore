using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.LMS.Service
{
    public class CourseMap : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Courses");

            builder.HasOne(x => x.Calendar)
                .WithMany(x => x.Courses)
                .HasForeignKey(x => x.CalendarId);

            builder.HasOne(x => x.Term)
               .WithMany(x => x.Courses)
               .HasForeignKey(x => x.TermId);
        }
    }
}