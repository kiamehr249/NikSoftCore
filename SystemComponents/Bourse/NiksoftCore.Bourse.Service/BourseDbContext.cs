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
        public DbSet<UserBankAccount> UserBankAccounts { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<BranchMaster> BranchMasters { get; set; }
        public DbSet<BranchMarketer> BranchMarketers { get; set; }
        public DbSet<BranchConsultant> BranchConsultants { get; set; }
        public DbSet<BranchUser> BranchUsers { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<MediaCategory> MediaCategories { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<ContractFee> ContractFees { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<ContractLetter> ContractLetters { get; set; }
        public DbSet<BaseTransaction> BaseTransactions { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        public DbSet<BranchArea> BranchAreas { get; set; }
        public DbSet<BranchAdLeader> BranchAdLeaders { get; set; }
        public DbSet<BranchAdvertiser> BranchAdvertisers { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<TicketPriority> TicketPriorities { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketAnswer> TicketAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new BourseUserMap());
            builder.ApplyConfiguration(new UserProfileMap());
            builder.ApplyConfiguration(new UserBankAccountMap());
            builder.ApplyConfiguration(new FeeMap());
            builder.ApplyConfiguration(new BranchMap());
            builder.ApplyConfiguration(new BranchMasterMap());
            builder.ApplyConfiguration(new BranchMarketerMap());
            builder.ApplyConfiguration(new BranchConsultantMap());
            builder.ApplyConfiguration(new BranchUserMap());
            builder.ApplyConfiguration(new ContractMap());
            builder.ApplyConfiguration(new MediaCategoryMap());
            builder.ApplyConfiguration(new MediaMap());
            builder.ApplyConfiguration(new ContractFeeMap());
            builder.ApplyConfiguration(new SettingMap());
            builder.ApplyConfiguration(new ContractLetterMap());
            builder.ApplyConfiguration(new BaseTransactionMap());
            builder.ApplyConfiguration(new PaymentReceiptMap());
            builder.ApplyConfiguration(new BranchAreaMap());
            builder.ApplyConfiguration(new BranchAdLeaderMap());
            builder.ApplyConfiguration(new BranchAdvertiserMap());
            builder.ApplyConfiguration(new TicketCategoryMap());
            builder.ApplyConfiguration(new TicketPriorityMap());
            builder.ApplyConfiguration(new TicketMap());
            builder.ApplyConfiguration(new TicketAnswerMap());
        }
    }
}
