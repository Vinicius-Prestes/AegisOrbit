using AegisOrbit.API.Domain.ValueObjects;

namespace AegisOrbit.API.Domain.Entities;

public abstract class SpacialObject
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; } = string.Empty;
    public double Mass { get; protected set; }
    public OrbitalCoordinates CurrentPosition { get; protected set; }
    = new OrbitalCoordinates(0, 0, 0);
    public double Velocity { get; protected set; }

    // Construtor usado pelo Entity Framework
    protected SpacialObject()
    {
    }

    // Construtor usado pela aplicação
    protected SpacialObject(
        string name,
        double mass,
        OrbitalCoordinates initialPosition,
        double velocity)
    {
        Id = Guid.NewGuid();
        Name = name;
        Mass = mass;
        CurrentPosition = initialPosition;
        Velocity = velocity;
    }

    public abstract double CalculateAtmosphericReentryRisk();

    public void UpdatePosition(
        OrbitalCoordinates newPosition,
        double newVelocity)
    {
        CurrentPosition = newPosition;
        Velocity = newVelocity;
    }
}