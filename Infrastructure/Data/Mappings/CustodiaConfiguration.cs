using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings
{
    internal class CustodiaConfiguration : IEntityTypeConfiguration<Custodia>
    {
        public void Configure(EntityTypeBuilder<Custodia> builder)
        {
            builder.ToTable("Custodias");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Ticker)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(c => c.PrecoMedio)
                .HasColumnType("decimal(18,4)");

            builder.HasOne(c => c.ContaGrafica)
                .WithMany(cg => cg.Custodias)
                .HasForeignKey(c => c.ContaGraficaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
