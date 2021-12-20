using Microsoft.EntityFrameworkCore;
using NiksoftCore.DataAccess;

namespace NiksoftCore.LMS.Service
{
    public class LmsDbContext : NikDbContext, ILmsUnitOfWork
    {

        public LmsDbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<LmsUser> LmsUsers { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<CalendarDay> CalendarDays { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Leason> Leasons { get; set; }
        public DbSet<LeasonFile> LeasonFiles { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new LmsUserMap());
            builder.ApplyConfiguration(new TermMap());
            builder.ApplyConfiguration(new CalendarMap());
            builder.ApplyConfiguration(new CalendarDayMap());
            builder.ApplyConfiguration(new CourseMap());
            builder.ApplyConfiguration(new LeasonMap());
            builder.ApplyConfiguration(new LeasonFileMap());
            builder.ApplyConfiguration(new UserCourseMap());
        }
    }
}
