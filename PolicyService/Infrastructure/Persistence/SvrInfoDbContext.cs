using Microsoft.EntityFrameworkCore;
using PolicyService.Domain.Entities;
using PolicyService;

namespace PolicyService.Infrastructure.Persistence
{
    public class PolicyServiceDbContext : DbContext
    {
        public PolicyServiceDbContext(DbContextOptions<PolicyServiceDbContext> options) : base(options) { }

        public DbSet<Statement> Statement { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Statement
            modelBuilder.Entity<Statement>(entity =>
            {
                entity.HasKey(s => s.StatementId); // Definição da chave primária
                entity.Property(s => s.Title).HasMaxLength(255).IsRequired();
                entity.Property(s => s.Content).HasColumnType("TEXT").IsRequired();
                entity.Property(s => s.Status).IsRequired();
                entity.Property(s => s.StatementType).IsRequired();
                entity.Property(s => s.ActiveTime).IsRequired();
                entity.Property(s => s.SectorId).IsRequired();
                entity.Property(s => s.ReceiverType).IsRequired();
            });
        }
    }
}
