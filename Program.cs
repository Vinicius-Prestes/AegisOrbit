using Microsoft.EntityFrameworkCore;
using AegisOrbit.API.Data;
using AegisOrbit.API.Domain.Interface;
using AegisOrbit.API.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Banco de Dados Oracle
builder.Services.AddDbContext<AegisOrbitContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// 2. Injeção de Dependência 
builder.Services.AddScoped<ICollisionService, CollisionService>();

// 3. ATIVAÇÃO DOS CONTROLLERS (Essencial para o seu OrbitalController funcionar)
builder.Services.AddControllers();

// Configurações padrão do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do Pipeline de Requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 4. MAPEAMENTO DAS ROTAS DOS CONTROLLERS (Diz para a API expor os seus endpoints)
app.MapControllers();

app.Run();