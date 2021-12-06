using Microsoft.EntityFrameworkCore;
using NiksoftCore.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //public DbSet<BourseUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.ApplyConfiguration(new BourseUserMap());
        }
    }
}
