using Microsoft.EntityFrameworkCore;
using NiksoftCore.DataAccess;

namespace NiksoftCore.Bourse.Service
{
    public class BourseDbContext : NikDbContext, IBourseUnitOfWork
    {

        public BourseDbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<BourseUser> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<PricingPackage> PricingPackages { get; set; }
        public DbSet<IcoCase> IcoCases { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new BourseUserMap());
            builder.ApplyConfiguration(new UserProfileMap());
            builder.ApplyConfiguration(new PricingPackageMap());
            builder.ApplyConfiguration(new IcoCaseMap());
        }
    }
}
