using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ContextAwareServiceSample
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try {
                Log.Information("Getting the motors running...");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex) {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                    services.AddHostedService<Worker>()
                        .AddSingleton<WorkExecutor>())
                .UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.Seq("http://localhost:5341")
                    .Enrich.FromLogContext()
                    .WriteTo.Console());
    }
}