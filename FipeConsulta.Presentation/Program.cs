using FipeConsulta.Application.Services;
using FipeConsulta.Infrastructure.Repositories;
using FipeConsulta.Domain.Interfaces;
using FipeConsulta.Application.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner.
builder.Services.AddControllers();

// Adicionar Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar HttpClient para injeção de dependência
builder.Services.AddHttpClient<IVehicleQueryService, VehicleQueryService>();

// Injeção de dependências para o repositório
builder.Services.AddScoped<IVehicleQueryProcessor, VehicleQueryRepository>();

// Registrar DelayHandler com TimeSpan definido explicitamente
builder.Services.AddScoped(provider => new DelayHandler(TimeSpan.FromMinutes(1))); // Definir o TimeSpan aqui

var app = builder.Build();

// Configurar o middleware do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FipeConsultaAPI v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
