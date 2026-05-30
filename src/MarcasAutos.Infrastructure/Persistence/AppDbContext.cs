using MarcasAutos.Domain.MarcasAutos;
using Microsoft.EntityFrameworkCore;

namespace MarcasAutos.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options)
  {
  }

  public DbSet<MarcaAuto> MarcasAutos => Set<MarcaAuto>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<MarcaAuto>(entity =>
    {
      entity.ToTable("MarcasAutos");

      entity.HasKey(marca => marca.Id);

      entity.Property(marca => marca.Nombre)
              .IsRequired()
              .HasMaxLength(100);

      entity.HasData(
              new { Id = 1, Nombre = "Toyota" },
              new { Id = 2, Nombre = "Honda" },
              new { Id = 3, Nombre = "Nissan" }
          );
    });
  }
}
