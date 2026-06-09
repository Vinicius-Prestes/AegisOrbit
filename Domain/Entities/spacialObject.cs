using AegisOrbit.API.Domain.ValueObjects;

namespace AegisOrbit.API.Domain.Entities;

public abstract class ObjetoEspacial
{
    public Guid Id { get; protected set; }
    public string Nome { get; protected set; }
    public double Massa { get; protected set; } // Em kg
    public CoordenadasOrbitais PosicaoAtual { get; protected set; }
    public double Velocidade { get; protected set; } // Em km/h

    protected ObjetoEspacial(string nome, double massa, CoordenadasOrbitais posicaoInicial, double velocidade)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Massa = massa;
        PosicaoAtual = posicaoInicial;
        Velocidade = velocidade;
    }

    // Método Polimórfico: Cada tipo de objeto calcula seu risco de reentrada de forma diferente
    public abstract double CalcularRiscoReentradaAtmosferica();

    public void AtualizarPosicao(CoordenadasOrbitais novaPosicao, double novaVelocidade)
    {
        PosicaoAtual = novaPosicao;
        Velocidade = novaVelocidade;
    }
}