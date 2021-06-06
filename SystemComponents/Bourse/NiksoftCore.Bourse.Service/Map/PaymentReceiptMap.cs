using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class PaymentReceiptMap : IEntityTypeConfiguration<PaymentReceipt>
    {
        public void Configure(EntityTypeBuilder<PaymentReceipt> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_PaymentReceipts");

            builder.HasOne(x => x.User)
                .WithMany(x => x.PaymentReceipts)
                .HasForeignKey(x => x.UserId).IsRequired(true);

            builder.HasOne(x => x.Transaction)
                .WithMany(x => x.PaymentReceipts)
                .HasForeignKey(x => x.TransactionId).IsRequired(true);


        }
    }
}