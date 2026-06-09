using AegisOrbit.API.Domain.Interface;
using AegisOrbit.API.Domain.ValueObjects;

namespace AegisOrbit.API.Domain.Entities;

public class ActiveSatellite : SpacialObject, ITelemetryTransmitter
{
    public string Operator { get; private set; }
    public string OperatingFrequency { get; private set; }
    public bool IsSignalActive { get; private set; }

    public ActiveSatellite(string name, double mass, OrbitalCoordinates position, double velocity, string satelliteOperator, string frequency)
        : base(name, mass, position, velocity)
    {
        Operator = satelliteOperator;
        OperatingFrequency = frequency;
        IsSignalActive = true;
    }

    public override double CalculateAtmosphericReentryRisk()
    {
        // Active satellites have thrusters to perform controlled reentry (lower environmental risk)
        return (Mass * 0.05) / CurrentPosition.Altitude;
    }

    public void TransmitData()
    {
        if (!IsSignalActive) 
            throw new InvalidOperationException("Cannot transmit data when the signal is inactive.");
    }
}