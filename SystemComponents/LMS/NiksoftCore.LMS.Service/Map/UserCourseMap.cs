using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.LMS.Service
{
    public class UserCourseMap : IEntityTypeConfiguration<UserCourse>
    {
        public void Configure(EntityTypeBuilder<UserCourse> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("UserCourses");

            builder.HasOne(x => x.Course)
                .WithMany(x => x.UserCourses)
                .HasForeignKey(x => x.CourseId);

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserCourses)
                .HasForeignKey(x => x.UserId);
        }
    }
}