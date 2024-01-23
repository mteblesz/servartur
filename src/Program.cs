using Microsoft.OpenApi.Models;
using servartur.Entities;
using servartur.Services;
using NLog;
using NLog.Web;
using servartur.Middleware;
using servartur.Seeders;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

FirebaseInitializer.Initialize();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(addSwaggerOptions());
    builder.Services.AddDbContext<GameDbContext>();
    builder.Services.AddScoped<DbSeeder>();
    builder.Services.AddAutoMapper(typeof(Program).Assembly);
    builder.Services.AddScoped<IMatchupService, MatchupService>();
    builder.Services.AddScoped<IInfoService, InfoService>();
    builder.Services.AddScoped<IVoteService, VoteService>();
    builder.Services.AddScoped<IKillService, KillService>();
    builder.Services.AddScoped<ISquadService, SquadService>();
    builder.Services.AddScoped<ErrorHandlingMiddleware>();
    builder.Services.AddScoped<FirebaseAuthMiddleware>();
    builder.Services.AddScoped<RequestTimingMiddleware>();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Build
    var app = builder.Build();

    // Call services
    var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    seeder.Seed();

    // Configure the HTTP request pipeline. //middleware
    if (app.Environment.IsDevelopment())
    {
    }
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseMiddleware<RequestTimingMiddleware>(); 
    app.UseMiddleware<FirebaseAuthMiddleware>();

    app.UseHttpsRedirection();

    //app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}

static Action<Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions> addSwaggerOptions()
{
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    return options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Servartur API",
            Description = "An ASP.NET Core Web API for managing server side of Fluttartur - a local multiplayer game based on \"The Resistance: Avalon\"",
        });
    };
}