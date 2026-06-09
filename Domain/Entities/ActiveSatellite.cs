using AegisOrbit.API.Domain.Interface;
using AegisOrbit.API.Domain.ValueObjects;

namespace AegisOrbit.API.Domain.Entities;

public class ActiveSatellite : SpacialObject, ITelemetryTransmitter
{
    public string Operator { get; private set; } = string.Empty;
    public string OperatingFrequency { get; private set; } = string.Empty;
    public bool IsSignalActive { get; private set; }

    // Construtor protegido exigido pelo Entity Framework Core
    protected ActiveSatellite() : base()
    {
    }

    // Construtor usado pela sua aplicação / Controller
    public ActiveSatellite(string name, double mass, OrbitalCoordinates position, double velocity, string satelliteOperator, string frequency)
        : base(name, mass, position, velocity)
    {
        Operator = satelliteOperator;
        OperatingFrequency = frequency;
        IsSignalActive = true;
    }

    public override double CalculateAtmosphericReentryRisk()
    {
        return (Mass * 0.05) / CurrentPosition.Altitude;
    }

    public void TransmitData()
    {
        if (!IsSignalActive) 
            throw new InvalidOperationException("Cannot transmit data when the signal is inactive.");
    }
}