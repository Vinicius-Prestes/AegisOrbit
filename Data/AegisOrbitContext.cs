using Microsoft.EntityFrameworkCore;
using AegisOrbit.API.Domain.Entities;

namespace AegisOrbit.API.Data;

public class AegisOrbitContext : DbContext
{
    public AegisOrbitContext(DbContextOptions<AegisOrbitContext> options) : base(options)
    {
    }

    public DbSet<SpacialObject> SpacialObjects { get; set; }
    public DbSet<ActiveSatellite> ActiveSatellites { get; set; }
    public DbSet<DebrisSpace> DebrisSpaces { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // 1. Configuração de Herança (TPH)
    modelBuilder.Entity<SpacialObject>()
        .HasDiscriminator<string>("ObjectType")
        .HasValue<ActiveSatellite>("Satellite")
        .HasValue<DebrisSpace>("Debris");

    // 2. Mapeamento do Value Object (Struct)
    modelBuilder.Entity<SpacialObject>().OwnsOne(s => s.CurrentPosition, pos =>
    {
        pos.Property(p => p.Latitude).HasColumnName("Latitude");
        pos.Property(p => p.Longitude).HasColumnName("Longitude");
        pos.Property(p => p.Altitude).HasColumnName("Altitude");
        pos.Property(p => p.UpdatedAt).HasColumnName("PositionUpdatedAt");
    });

    // Força o C# bool a virar NUMBER(1) no banco de dados Oracle
    modelBuilder.Entity<ActiveSatellite>()
        .Property(s => s.IsSignalActive)
        .HasColumnType("NUMBER(1)");
    } 
}