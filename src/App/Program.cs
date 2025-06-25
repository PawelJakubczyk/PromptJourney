using Microsoft.EntityFrameworkCore;
using Persistans.Context;

try
{
    Console.WriteLine("Staring the web host");

    //Initial configuration

    var builder = WebApplication.CreateBuilder(new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = Directory.GetCurrentDirectory()
    });

    builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

    //Configure Services

    //builder.Services
        //.RegisterAppOptions()
        //.RegisterApplicationLayer()
        //.RegisterPersistenceLayer(builder.Environment, builder.Logging)
        //.RegisterInfrastructureLayer()
        //.RegisterPresentationLayer();

    //Build the application

    WebApplication webApplication = builder.Build();

    //Configure HTTP request pipeline

    //webApplication
        //.UseHttpsRedirection()
        //.UseApplicationLayer()
        //.UsePresentationLayer(builder.Environment)
        //.UsePersistenceLayer();

    webApplication.MapControllers();

    //Run the application

    using var scope = webApplication.Services.CreateScope();
    var dbcontext = scope.ServiceProvider.GetRequiredService<MidjourneyDbContext>();
    var pendingMigrations = dbcontext.Database.GetPendingMigrations();

    if (pendingMigrations.Any())
    {
        Console.WriteLine("Applying pending migrations...");
        dbcontext.Database.Migrate();
        Console.WriteLine("Migrations applied successfully.");
    }
    else
    {
        Console.WriteLine("No pending migrations found.");
    }

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

sealed partial class Program;
