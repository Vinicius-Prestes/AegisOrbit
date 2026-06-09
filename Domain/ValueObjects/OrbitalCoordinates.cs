namespace AegisOrbit.API.Domain.ValueObjects;

public readonly struct OrbitalCoordinates
{
    public double Latitude { get; }
    public double Longitude { get; }
    public double Altitude { get; } // In kilometers
    public DateTime UpdatedAt { get; }

    public OrbitalCoordinates(double latitude, double longitude, double altitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
        UpdatedAt = DateTime.UtcNow; // Precise DateTime manipulation
    }
}