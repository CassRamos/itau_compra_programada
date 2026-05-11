using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings
{
    internal class ItemCestaConfiguration : IEntityTypeConfiguration<ItemCesta>
    {
        public void Configure(EntityTypeBuilder<ItemCesta> builder)
        {
            builder.ToTable("ItensCesta");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Ticker)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(i => i.Percentual)
                .HasColumnType("decimal(5,2)");
        }
    }
}
