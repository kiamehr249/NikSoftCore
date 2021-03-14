using Microsoft.EntityFrameworkCore;
using NiksoftCore.DataAccess;

namespace NiksoftCore.ITCF.Service
{
    public class ITCFDbContext : NikDbContext, IITCFUnitOfWork
    {

        public ITCFDbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<UserLegalForm> UserLegalForms { get; set; }
        public DbSet<BusinessCategory> BusinessCategories { get; set; }
        public DbSet<IndustrialPark> IndustrialParks { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Introduction> Introductions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserPurchase> UserPurchases { get; set; }
        public DbSet<UserModel> UserModels { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ProductFile> ProductFiles { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserLegalFormMap());
            builder.ApplyConfiguration(new BusinessCategoryMap());
            builder.ApplyConfiguration(new IndustrialParkMap());
            builder.ApplyConfiguration(new BusinessMap());
            builder.ApplyConfiguration(new CountryMap());
            builder.ApplyConfiguration(new ProvinceMap());
            builder.ApplyConfiguration(new CityMap());
            builder.ApplyConfiguration(new IntroductionMap());
            builder.ApplyConfiguration(new ProductMap());
            builder.ApplyConfiguration(new UserPurchaseMap());
            builder.ApplyConfiguration(new UserModelMap());
            builder.ApplyConfiguration(new UserProfileMap());
            builder.ApplyConfiguration(new ProductFileMap());
        }
    }
}
