using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using RJDev.Tyml.Core.Tests.TestTasks.Cmd;
using RJDev.Tyml.Core.Tests.TestTasks.LongDelay;

namespace RJDev.Tyml.Core.Tests
{
    public class TestBase
    {
        /// <summary>
        /// Return context istance
        /// </summary>
        /// <returns></returns>
        protected static TymlContext GetContext()
        {
            return new TymlContextBuilder()
                .UseTasks(typeof(CmdTask), typeof(LongDelayTask))
                .UseWorkingDirectory(Path.Combine(Directory.GetCurrentDirectory(), "work-dir"))
                .WithBaseVariables(new Dictionary<string, object>()
                {
                    {"foo", 5},
                    {"bar", "string"},
                    {"lipsum", "Lorem Ipsum dolor sit amet."},
                    {"assemblyVersion", typeof(TestBase).Assembly.GetName().Version?.ToString() ?? string.Empty},
                })
                .Build();
        }

        /// <summary>
        /// Prepare IServiceProvider
        /// </summary>
        /// <returns></returns>
        protected static IServiceProvider GetServiceProvider()
        {
            ServiceCollection collection = new();
            collection.AddSingleton<TymlExecutor>();

            collection.AddTransient<CmdTask>();
            collection.AddTransient<LongDelayTask>();

            ServiceProvider provider = collection.BuildServiceProvider();
            return provider;
        }
    }
}