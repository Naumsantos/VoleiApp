using VoleiApp.Application.Interfaces.Services;
using VoleiApp.Application.Services;
using VoleiApp.Domain.Interfaces.Repositories;
using VoleiApp.Infrastructure.Persistence;
using VoleiApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Services
builder.Services.AddScoped<ISorteioService, SorteioService>();
builder.Services.AddScoped<IAtletaService, AtletaService>();

// Repositories
builder.Services.AddScoped<IAtletaRepository, AtletaRepository>();
builder.Services.AddScoped<IPartidaRepository, PartidaRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();