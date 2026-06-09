using AegisOrbit.API.Domain.Entities;
using AegisOrbit.API.Domain.DTOs;

namespace AegisOrbit.API.Domain.Interface;

public interface ICollisionService
{
    CollisionAlertDTO CheckCollisionRisk(SpacialObject obj1, SpacialObject obj2);
}
