using Microsoft.EntityFrameworkCore;
using MarcasAutos.Api.Models;
using System.Reflection.Metadata;

namespace MarcasAutos.Api.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  public DbSet<MarcaAuto> MarcasAutos => Set<MarcaAuto>();

  protected override void OnModelCreating(ModelBuilder moderlBuilder)
  {
    moderlBuilder.Entity<MarcaAuto>(entity =>
    {
      entity.ToTable("MarcasAutos");
      entity.HasKey(m => m.Id);
      entity.Property(m => m.Nombre)
        .IsRequired()
        .HasMaxLength(100);
      entity.HasData(
        new MarcaAuto { Id = 1, Nombre = "Toyota" },
        new MarcaAuto { Id = 2, Nombre = "Honda" },
        new MarcaAuto { Id = 3, Nombre = "Nissan" }
      );
    });
  }
}