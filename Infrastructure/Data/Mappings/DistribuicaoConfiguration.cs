using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings
{
    internal class DistribuicaoConfiguration : IEntityTypeConfiguration<Distribuicao>
    {
        public void Configure(EntityTypeBuilder<Distribuicao> builder)
        {
            builder.ToTable("Distribuicoes");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Ticker)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(d => d.PrecoUnitario)
                .HasColumnType("decimal(18,4)");

            builder.HasOne(d => d.OrdemCompra)
                .WithMany()
                .HasForeignKey(d => d.OrdemCompraId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.CustodiaFilhote)
                .WithMany()
                .HasForeignKey(d => d.CustodiaFilhoteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
