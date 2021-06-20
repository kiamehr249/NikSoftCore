using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class TicketMap : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_Tickets");

            builder.HasOne(x => x.User)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.UserId).IsRequired(true);

            builder.HasOne(x => x.Target)
                .WithMany(x => x.TicketTargets)
                .HasForeignKey(x => x.TargetId).IsRequired(false);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.CategoryId).IsRequired(true);

            builder.HasOne(x => x.Priority)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.PriorityId).IsRequired(true);
        }
    }
}