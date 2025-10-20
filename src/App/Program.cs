using Application.Registrations;
using Persistence.Registrations;
using App.Middleware;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

try
{
    Console.WriteLine("Starting the web host");

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
        .RegisterPersistenceLayer()
        //.RegisterInfrastructureLayer()
        .RegisterPresentationLayer();

    //Build the application

    WebApplication webApplication = builder.Build();

    //Configure HTTP request pipeline

    // register global exception handler early in the pipeline
    webApplication.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    webApplication
        //.UseHttpsRedirection()
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
    Console.WriteLine("Ending the web host");
    loggerFactory.Dispose();
}

return 0;

public sealed partial class Program;