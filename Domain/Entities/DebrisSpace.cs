using AegisOrbit.API.Domain.ValueObjects;

namespace AegisOrbit.API.Domain.Entities;

public class DebrisSpace : SpacialObject
{
    public string Origin { get; private set; }  //corrigindo classe
    public double EstimatedSizeMeters { get; private set; }


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