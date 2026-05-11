using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings
{
    internal class CestaTopFiveConfiguration : IEntityTypeConfiguration<CestaTopFive>
    {
        public void Configure(EntityTypeBuilder<CestaTopFive> builder)
        {
            builder.ToTable("CestasRecomendacao");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(c => c.Itens)
                .WithOne(i => i.Cesta)
                .HasForeignKey(i => i.CestaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
