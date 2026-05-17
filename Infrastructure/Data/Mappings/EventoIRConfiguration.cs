using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings
{
    public class EventoIRConfiguration : IEntityTypeConfiguration<EventoIR>
    {
        public void Configure(EntityTypeBuilder<EventoIR> builder)
        {
            builder.ToTable("EventosIR");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.ValorBase)
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.ValorIR)
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.Tipo)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(e => e.Cliente)
                .WithMany()
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
