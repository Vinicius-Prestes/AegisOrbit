namespace AegisOrbit.API.Domain.Exceptions;


public class CollisionCalculationException : Exception
{
    public string OrbitIdA { get; }
    public string OrbitIdB { get; }

    public CollisionCalculationException(string message, Guid idA, Guid idB) : base(message)
    {
        OrbitIdA = idA.ToString();
        OrbitIdB = idB.ToString();
    }
}