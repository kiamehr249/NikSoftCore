using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class TicketAnswerMap : IEntityTypeConfiguration<TicketAnswer>
    {
        public void Configure(EntityTypeBuilder<TicketAnswer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_TicketAnswers");
        }
    }
}