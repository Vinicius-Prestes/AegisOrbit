namespace AegisOrbit.API.Domain.DTOs;

public class CollisionAlertDTO
{
    public Guid AlertId { get; set; }
    public string ObjectAName { get; set; } = string.Empty;
    public string ObjectBName { get; set; } = string.Empty;
    public double CollisionProbability { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public DateTime EstimatedCollisionTime { get; set; }
    public DateTime GeneratedAt { get; set; }

    public CollisionAlertDTO()
    {
        AlertId = Guid.NewGuid();
        GeneratedAt = DateTime.UtcNow;
    }
}