﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ArchDesign
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
            

            try
            {
                Log.Information("Getting the motors running...");

                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            //new HostBuilder()
            //   .ConfigureHostConfiguration(BuildConfiguration)
            //   .ConfigureServices(services => services.AddSingleton<IHostedService, PrintTimeService>())
            //   .UseSerilog()
            //   .Build();
            return WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .ConfigureServices(services => services.AddSingleton<IHostedService, PrintTimeService>())
                   .UseConfiguration(Configuration)
                   .UseSerilog()
                   .Build();
        }
    }
}
