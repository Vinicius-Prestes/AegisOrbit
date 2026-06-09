using AegisOrbit.API.Domain.ValueObjects;

namespace AegisOrbit.API.Domain.Entities;

public abstract class SpacialObject
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
    public double Mass { get; protected set; } // In kg
    public OrbitalCoordinates CurrentPosition { get; protected set; }
    public double Velocity { get; protected set; } // In km/h

    protected SpacialObject(string name, double mass, OrbitalCoordinates initialPosition, double velocity)
    {
        Id = Guid.NewGuid();
        Name = name;
        Mass = mass;
        CurrentPosition = initialPosition;
        Velocity = velocity;
    }

    // Polymorphic Method: Each object type calculates its reentry risk differently
    public abstract double CalculateAtmosphericReentryRisk();

    public void UpdatePosition(OrbitalCoordinates newPosition, double newVelocity)
    {
        CurrentPosition = newPosition;
        Velocity = newVelocity;
    }
}