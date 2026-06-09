using AegisOrbit.API.Domain.ValueObjects;

namespace AegisOrbit.API.Domain.Entities;

public abstract partial class SpacialObject
{
    public abstract double CalculateAtmosphericReentryRisk();

    public void UpdatePosition(
        OrbitalCoordinates newPosition,
        double newVelocity)
    {
        CurrentPosition = newPosition;
        Velocity = newVelocity;
    }
}
