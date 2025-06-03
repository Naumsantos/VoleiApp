using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuração do banco de dados em memória (para ambiente de desenvolvimento/testes)
        builder.Services.AddDbContext<VoleiApp.Data.VoleiContext>(opt => opt.UseInMemoryDatabase("VoleiDB"));

        // Adiciona serviços ao contêiner (injeção de dependência)
        builder.Services.AddControllers();

        // Configura Swagger (documentação da API)
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            // Permite gerar documentação XML se ativada no .csproj
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });

        // CORS liberado para desenvolvimento (ajustar domínio em produção)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DevPolicy",
                policy => policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("DevPolicy");

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}