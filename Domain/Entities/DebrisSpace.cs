using AegisOrbit.API.Domain.ValueObjects;

namespace AegisOrbit.API.Domain.Entities;

public class DebrisSpace : SpacialObject
{
    public string Origin { get; private set; } = string.Empty;
    public double EstimatedSizeMeters { get; private set; }

    // Construtor protegido exigido pelo Entity Framework Core
    protected DebrisSpace() : base()
    {
    }

    // Construtor usado pela sua aplicação
    public DebrisSpace(string name, double mass, OrbitalCoordinates position, double velocity, string origin, double estimatedSize)
        : base(name, mass, position, velocity)
    {
        Origin = origin;
        EstimatedSizeMeters = estimatedSize;
    }

    public override double CalculateAtmosphericReentryRisk()
    {
        return (Mass * 0.4) / CurrentPosition.Altitude;
    }
}