using AegisOrbit.API.Domain.Entities;
using AegisOrbit.API.Domain.Interface;
using AegisOrbit.API.Domain.DTOs;
using AegisOrbit.API.Domain.Exceptions;

namespace AegisOrbit.API.Domain.Services;

public class CollisionService : ICollisionService
{
    public CollisionAlertDTO CheckCollisionRisk(SpacialObject obj1, SpacialObject obj2)
    {
        // Tratamento de Exceção Crítica: se as órbitas estiverem corrompidas ou inválidas
        if (obj1.CurrentPosition.Altitude <= 0 || obj2.CurrentPosition.Altitude <= 0)
        {
            throw new CollisionCalculationException(
                "Telemetry data error: Altitude cannot be zero or negative for orbital objects.", 
                obj1.Id, 
                obj2.Id
            );
        }

        // Simulação de cálculo matemático de aproximação 
        double distance = Math.Abs(obj1.CurrentPosition.Altitude - obj2.CurrentPosition.Altitude);
        double probability = 0.0;
        string riskLevel = "LOW";

        if (distance < 10) // Menos de 10km de distância orbital
        {
            probability = 94.7;
            riskLevel = "CRITICAL";
        }
        else if (distance < 50)
        {
            probability = 42.1;
            riskLevel = "MEDIUM";
        }

 
        double rawTimeWindowHours = distance / ((obj1.Velocity + obj2.Velocity) / 2);
        DateTime estimatedTime = DateTime.UtcNow.AddHours(rawTimeWindowHours);

        return new CollisionAlertDTO
        {
            ObjectAName = obj1.Name,
            ObjectBName = obj2.Name,
            CollisionProbability = probability,
            RiskLevel = riskLevel,
            EstimatedCollisionTime = estimatedTime
        };
    }
}