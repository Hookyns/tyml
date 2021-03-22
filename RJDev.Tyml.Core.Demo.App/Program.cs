using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RJDev.Outputter.Sinks.Console;
using RJDev.Outputter.Sinks.Console.Themes;
using RJDev.Tyml.Core.Tasks;
using RJDev.Tyml.Core.Tests;
using RJDev.Tyml.Core.Tests.TestTasks.Cmd;
using RJDev.Tyml.Core.Tests.TestTasks.LongDelay;

namespace RJDev.Tyml.Core.Demo.App
{
	class Program
	{
		static async Task Main(string[] args)
		{
			IServiceProvider serviceProvider = GetServiceProvider();
			TymlContext context = GetContext();
			TymlExecutor executor = serviceProvider.GetRequiredService<TymlExecutor>();
			
			using ConsoleSink sink = new ConsoleSink(
				new ConsoleSinkOptions(ColorTheme.DarkConsole)
				{
					ConsoleEncoding = Encoding.Default
				}
			);

			string yaml = @"
steps:
  - task: Cmd
    displayName: 'Echo the most important message'
    inputs:
      Script: 'echo Hello World!'

  - task: LongDelay
    displayName: 'Task containing long delay to test abort'

  - task: Cmd
    displayName: 'Echo the most important message'
    inputs:
      Script: 'echo First delay done'

  - task: LongDelay
    displayName: 'Task containing long delay to test abort'

  - task: Cmd
    displayName: 'Echo the most important message'
    inputs:
      Script: 'echo Second delay done'
";

			await foreach (TaskExecution execution in executor.Execute(context, yaml))
			{
				await execution.OutputReader.Pipe(sink);
			}
		}
		
		/// <summary>
		/// Return context istance
		/// </summary>
		/// <returns></returns>
		protected static TymlContext GetContext()
		{
			return new TymlContextBuilder()
				.AddTasks(typeof(CmdTask), typeof(LongDelayTask))
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