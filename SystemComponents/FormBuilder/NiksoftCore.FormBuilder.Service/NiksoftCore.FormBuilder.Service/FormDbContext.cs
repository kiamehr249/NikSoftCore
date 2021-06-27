using Microsoft.EntityFrameworkCore;
using NiksoftCore.DataAccess;

namespace NiksoftCore.FormBuilder.Service
{
    public class FormDbContext : NikDbContext, IFormUnitOfWork
    {

        public FormDbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<FormUser> Users { get; set; }
        public DbSet<FormUserProfile> UserProfiles { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormControl> FormControls { get; set; }
        public DbSet<ControlItem> ControlItems { get; set; }
        public DbSet<FormData> FormDatas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new BourseUserMap());
            builder.ApplyConfiguration(new UserProfileMap());
            builder.ApplyConfiguration(new FormMap());
            builder.ApplyConfiguration(new FormControlMap());
            builder.ApplyConfiguration(new ControlItemMap());
            builder.ApplyConfiguration(new FormDataMap());
        }
    }
}
