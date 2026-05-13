using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings
{
    public class OrdemCompraConfiguration : IEntityTypeConfiguration<OrdemCompra>
    {
        public void Configure(EntityTypeBuilder<OrdemCompra> builder)
        {
            builder.ToTable("OrdensCompra");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Ticker)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(o => o.PrecoUnitario)
                .HasColumnType("decimal(18,4)");

            builder.Property(o => o.TipoMercado)
                .IsRequired()
                .HasMaxLength(20);

        }
    }
}
