using Application.Registrations;
using Persistence.Registrations;

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
    Console.Error.WriteLine($"Host terminated unexpectedly. Exception: {exception.Message}");
    return 1;
}
finally
{
    Console.WriteLine("Ending the web host");
}

return 0;

public sealed partial class Program;