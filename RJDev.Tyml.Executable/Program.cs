using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RJDev.Core.Extensibility;
using RJDev.Tyml.Core;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace RJDev.Tyml.Executable
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class Program
    {
        private static readonly string AppPath = AppContext.BaseDirectory;

        private static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            try
            {
                await host.StartAsync().ConfigureAwait(false);
                await Run(host.Services);
                await host.StopAsync().ConfigureAwait(false);
            }
            finally
            {
                if (host is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                }
                else
                {
                    host.Dispose();
                }
            }
        }

        /// <summary>
        /// Run
        /// </summary>
        /// <param name="provider"></param>
        private static async Task Run(IServiceProvider provider)
        {
            IHostEnvironment environment = provider.GetRequiredService<IHostEnvironment>();

            string path = Path.Combine(environment.ContentRootPath, "test-yamls");
            string ymlPath = Path.Combine(path, "2.yml");
            string ymlContent = await File.ReadAllTextAsync(ymlPath);

            TymlContext tymlContext = new TymlContextBuilder()
                .UseTasks(typeof(CmdTask))
                .UseWorkingDirectory(path)
                .WithBaseVariables(new Dictionary<string, object>()
                {
                    {"foo", 5},
                    {"bar", "baz"}
                })
                .Build();

            TymlExecutor tymlExecutor = provider.GetRequiredService<TymlExecutor>();
            await tymlExecutor.Execute(tymlContext, ymlContent);
        }

        /// <summary>
        /// Returns instance of host builder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                    .UseSerilog(ConfigureLogger)
                    .WithAddons()
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddSingleton<TymlExecutor>();
                        services.AddTransient<CmdTask>();
                    })
                    // .UseContentRoot(AppContext.BaseDirectory);
                    .UseConsoleLifetime()
                ;
        }

        /// <summary>
        /// Configure logger
        /// </summary>
        /// <returns></returns>
        private static void ConfigureLogger(HostBuilderContext hostContext, IServiceProvider provider, LoggerConfiguration config)
        {
            var outputTemplate = "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}";

            config
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.Console(outputTemplate: outputTemplate)
                .WriteTo.File(new CompactJsonFormatter(), "logs/log.json", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext();

            if (hostContext.HostingEnvironment.IsDevelopment())
            {
                config
                    .MinimumLevel.Debug()
                    .WriteTo.Debug(outputTemplate: outputTemplate);
            }
        }
    }
}