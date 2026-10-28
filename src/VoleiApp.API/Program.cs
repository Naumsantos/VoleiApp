using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using VoleiApp.Application.Interfaces.Services;
using VoleiApp.Application.Services;
using VoleiApp.Domain.Interfaces.Repositories;
using VoleiApp.Infrastructure.AI;
using VoleiApp.Infrastructure.Persistence;
using VoleiApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Volei App API",
            Description = "Uma api para gerenciar sorteio de times, partidas e jogadores.",
            Contact = new OpenApiContact
            {
                Name = "Email",
                Email = "naum_santos@hotmail.com"
            }
        });

        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    }
);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<VoleiContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=voleiapp.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("Dev", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

//AI
builder.Services.Configure<AiOptions>(builder.Configuration.GetSection(AiOptions.SectionName));
builder.Services.AddHttpClient<OllamaAiInsightsService>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<AiOptions>>().Value;

    client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
});
builder.Services.AddScoped<FallbackInsightsService>();
builder.Services.AddScoped<IAiInsightsService, ResilienteAiInsightsService>();

// Services
builder.Services.AddScoped<ISorteioService, SorteioService>();
builder.Services.AddScoped<IAtletaService, AtletaService>();
builder.Services.AddScoped<IPartidaService, PartidaService>();
builder.Services.AddScoped<ITimeService, TimeService>();

// Repositories
builder.Services.AddScoped<IAtletaRepository, AtletaRepository>();
builder.Services.AddScoped<IPartidaRepository, PartidaRepository>();
builder.Services.AddScoped<ITimeRepository, TimeRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VoleiContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Dev");

app.Run();
