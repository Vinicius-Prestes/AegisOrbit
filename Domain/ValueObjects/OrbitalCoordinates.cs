namespace AegisOrbit.API.Domain.ValueObjects;

public struct OrbitalCoordinates
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double Altitude { get; init; }
    public DateTime UpdatedAt { get; init; }

    public OrbitalCoordinates(double latitude, double longitude, double altitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
        UpdatedAt = DateTime.UtcNow;
    }
}