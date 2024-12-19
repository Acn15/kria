using Microsoft.EntityFrameworkCore;
using KriaAPI.Models;

namespace KriaAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Repositorio> Repositorios { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                    .HasIndex(u => u.Email)
                    .IsUnique();

            modelBuilder.Entity<Repositorio>()
                .HasOne(r => r.Usuario) // Um Repositorio pertence a um Usuario
                .WithMany(u => u.Repositorios) // Um Usuario tem muitos Repositorios
                .HasForeignKey(r => r.UsuarioId) // Chave estrangeira
                .OnDelete(DeleteBehavior.Cascade); // Se o Usuario for deletado, deletar os Repositorios
        }
    }
}
