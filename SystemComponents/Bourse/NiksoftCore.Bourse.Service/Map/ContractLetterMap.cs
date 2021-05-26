using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.Bourse.Service
{
    public class ContractLetterMap : IEntityTypeConfiguration<ContractLetter>
    {
        public void Configure(EntityTypeBuilder<ContractLetter> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("B_ContractLetters");
        }
    }
}