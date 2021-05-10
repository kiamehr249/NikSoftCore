using Microsoft.EntityFrameworkCore;
using NiksoftCore.DataAccess;

namespace NiksoftCore.Bourse.Service
{
    public class BourceDbContext : NikDbContext, IBourceUnitOfWork
    {

        public BourceDbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        //public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.ApplyConfiguration(new SystemSettingMap());
        }
    }
}
