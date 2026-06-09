namespace AegisOrbit.API.Domain.DTOs;

public class CollisionAlertDTO
{
    public Guid AlertId { get; set; }
    public string ObjectAName { get; set; }
    public string ObjectBName { get; set; }
    public double CollisionProbability { get; set; }
    public string RiskLevel { get; set; }
    public DateTime EstimatedCollisionTime { get; set; } // Manipulação precisa de data
    public DateTime GeneratedAt { get; set; }

    public CollisionAlertDTO()
    {
        AlertId = Guid.NewGuid();
        GeneratedAt = DateTime.UtcNow;
    }
}