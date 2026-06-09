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

        // 2. Mapeamento de Struct (Complex Type do .NET 8)
        // Substituído 'OwnsOne' por 'ComplexProperty' para aceitar a Struct perfeitamente
        modelBuilder.Entity<SpacialObject>().ComplexProperty(s => s.CurrentPosition, cm =>
        {
            cm.Property(p => p.Latitude).HasColumnName("Latitude");
            cm.Property(p => p.Longitude).HasColumnName("Longitude");
            cm.Property(p => p.Altitude).HasColumnName("Altitude");
            cm.Property(p => p.UpdatedAt).HasColumnName("PositionUpdatedAt");
        });
        
        // 3. Fix para o banco Oracle (bool -> NUMBER(1))
        modelBuilder.Entity<ActiveSatellite>()
            .Property(s => s.IsSignalActive)
            .HasColumnType("NUMBER(1)");
    }
}