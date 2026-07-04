using App.Configuration;
using App.Middleware;
using Application.Registrations;
using Microsoft.OpenApi.Models;
using Persistence.Registrations;
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
        .RegisterPresentationLayer()
        .RegisterAuthentication(builder.Configuration);

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.ConfigureCustomModelStateValidation();
        });

    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Input: Bearer {token}"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    //Build the application

    WebApplication webApplication = builder.Build();

    //Configure HTTP request pipeline

    // register global exception handler early in the pipeline
    webApplication.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    webApplication.UsePresentationLayer();
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