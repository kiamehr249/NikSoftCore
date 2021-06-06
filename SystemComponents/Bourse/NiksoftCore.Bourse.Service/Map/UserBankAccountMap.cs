using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class UserBankAccountMap : IEntityTypeConfiguration<UserBankAccount>
    {
        public void Configure(EntityTypeBuilder<UserBankAccount> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_UserBankAccounts");

            builder.HasOne(x => x.User)
                .WithMany(x => x.BankAccounts)
                .HasForeignKey(x => x.UserId).IsRequired(true);
        }
    }
}