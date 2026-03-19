using Application.Registrations;
using Persistence.Registrations;
using App.Middleware;
using App.Configuration;
using Presentation.Registrations;
using System.Text.Json.Serialization;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

try
{
    logger.LogInformation("Starting the web host");

    //Initial configuration

    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = Directory.GetCurrentDirectory()
    });

    builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

    //Configure Services

    builder.Services
        //.RegisterAppOptions()
        .RegisterApplicationLayer()
        .RegisterPersistenceLayer(builder.Environment)
        //.RegisterInfrastructureLayer()
        .RegisterPresentationLayer();

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.ConfigureCustomModelStateValidation();
        });

    //Build the application

    WebApplication webApplication = builder.Build();

    //Configure HTTP request pipeline

    // register global exception handler early in the pipeline
    webApplication.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    webApplication
        .UseHttpsRedirection()
        //.UseApplicationLayer()
        .UsePresentationLayer();
        //.UsePersistenceLayer();

    webApplication.MapControllers();

    //Run the application
    webApplication.Run();
}
catch (Exception exception)
{
    logger.LogCritical(exception, "Host terminated unexpectedly.");

    if (exception is AggregateException aggregateException)
    {
        foreach (var innerException in aggregateException.Flatten().InnerExceptions)
        {
            logger.LogCritical(innerException, "Inner exception");
        }
    }
    return 1;
}
finally
{
    logger.LogInformation("Ending the web host");
    loggerFactory.Dispose();
}

return 0;

public sealed partial class Program;