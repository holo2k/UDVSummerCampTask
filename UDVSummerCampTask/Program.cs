using Microsoft.EntityFrameworkCore;
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
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient<VkService>();

        builder.Services.AddScoped<IAnalysisService, AnalysisService>();
        builder.Services.AddScoped<ILetterService, LetterService>();
        builder.Services.AddScoped<ILetterFrequencyRepository, LetterFrequencyRepository>();

        builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

        var app = builder.Build();

        using var scope = app.Services.CreateScope();
        using var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await appDbContext.Database.MigrateAsync();
        await appDbContext.SaveChangesAsync();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();
        app.Run();
    }
}