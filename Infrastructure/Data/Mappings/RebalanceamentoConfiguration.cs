using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings
{
    public class RebalanceamentoConfiguration : IEntityTypeConfiguration<Rebalanceamento>
    {
        public void Configure(EntityTypeBuilder<Rebalanceamento> builder)
        {
            builder.ToTable("Rebalanceamentos");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Tipo)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(r => r.TickerVendido).HasMaxLength(10);
            builder.Property(r => r.TickerComprado).HasMaxLength(10);

            builder.Property(r => r.ValorVenda)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(r => r.Cliente)
                .WithMany()
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
