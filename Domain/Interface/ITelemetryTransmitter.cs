namespace AegisOrbit.API.Domain.Interface;

public interface ITelemetryTransmitter
{
    string OperatingFrequency { get; }
    bool IsSignalActive { get; }
    void TransmitData();
}