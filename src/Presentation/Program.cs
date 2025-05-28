using Microsoft.Extensions.Configuration;
//using Persistans.Cpontext;
using System;
using System.IO;

public class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("connectionString.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Console.WriteLine("Hello world!");
        Console.WriteLine($"Connection string: {connectionString}");
    }
}
