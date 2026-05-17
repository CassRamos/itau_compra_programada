using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<CestaRecomendacao> CestaTopFive { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ContaGrafica> ContasGraficas { get; set; }
        public DbSet<Custodia> Custodias { get; set; }
        public DbSet<Distribuicao> Distribuicoes { get; set; }
        public DbSet<EventoIR> EventosIR { get; set; }
        public DbSet<ItemCesta> ItensCesta { get; set; }
        public DbSet<OrdemCompra> OrdensCompra { get; set; }
        public DbSet<Rebalanceamento> Rebalanceamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
