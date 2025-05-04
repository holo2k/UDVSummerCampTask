using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Interfaces;
using Serilog;
using UDVSummerCampTask.DAL;
using UDVSummerCampTask.DAL.Repository;
using UDVSummerCampTask.Mapping;
using UDVSummerCampTask.Services;
using UDVSummerCampTask.Services.Analysis;
using UDVSummerCampTask.Services.Letter;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, lc) => lc
            .WriteTo.File("logs/log.txt")
            .ReadFrom.Configuration(ctx.Configuration));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Тестовое задание в UDV Summer Camp",
                Version = "v1",
                Description = "API для анализа постов ВКонтакте и подсчета частоты букв",

                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "Belikov Nikita",
                    Email = "nneketaa@yandex.ru",
                    Url = new Uri("https://t.me/holo21k")
                }
            });

            var xmlFile = $"{AppDomain.CurrentDomain.FriendlyName}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        builder.Services.AddHttpClient<VkService>();

        builder.Services.AddScoped<IAnalysisService, AnalysisService>();
        builder.Services.AddScoped<ILetterService, LetterService>();
        builder.Services.AddScoped<ILetterFrequencyRepository, LetterFrequencyRepository>();

        builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        using var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await appDbContext.Database.MigrateAsync();
        await appDbContext.SaveChangesAsync();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UDV Summer Camp API V1");
            c.RoutePrefix = string.Empty;
        });
        app.MapControllers();
        app.Run();
    }
}