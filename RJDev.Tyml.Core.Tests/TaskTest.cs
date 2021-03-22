using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RJDev.Outputter.Sinks.Console;
using RJDev.Outputter.Sinks.Console.Themes;
using RJDev.Tyml.Core.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RJDev.Tyml.Core.Tests
{
	public class TaskTest : TestBase
	{
		private readonly ITestOutputHelper testOutputHelper;

		public TaskTest(ITestOutputHelper testOutputHelper)
		{
			this.testOutputHelper = testOutputHelper;
		}

		[Fact]
		public async Task CmdTaskTest()
		{
			IServiceProvider serviceProvider = GetServiceProvider();
			TymlContext context = GetContext();
			TymlExecutor executor = serviceProvider.GetRequiredService<TymlExecutor>();
			
			using ConsoleSink sink = new ConsoleSink(
				new ConsoleSinkOptions(ColorTheme.DarkConsole)
				{
					ConsoleEncoding = Encoding.UTF8
				}
			);

			string yaml = @"
steps:
  - task: Cmd
    displayName: 'Echo the most important message'
    inputs:
      Script: 'echo Hello World!'
";

			await foreach (TaskExecution execution in executor.Execute(context, yaml))
			{
				await execution.OutputReader.Pipe(sink);
			}
		}

		[Fact]
		public async Task AbortTaskTest()
		{
			IServiceProvider serviceProvider = GetServiceProvider();
			TymlContext context = GetContext();
			TymlExecutor executor = serviceProvider.GetRequiredService<TymlExecutor>();

			string yaml = @"
steps:
  - task: LongDelay
    displayName: 'Task containing long delay to test abort'
";

			var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

			await Assert.ThrowsAsync<TaskCanceledException>(async () =>
			{
				await foreach (TaskExecution execution in executor.Execute(context, yaml, cts.Token)) ;
			});
		}

		[Fact]
		public async Task NotAbortTaskTest()
		{
			IServiceProvider serviceProvider = GetServiceProvider();
			TymlContext context = GetContext();
			TymlExecutor executor = serviceProvider.GetRequiredService<TymlExecutor>();

			string yaml = @"
steps:
  - task: LongDelay
    displayName: 'Task containing long delay to test abort'
";

			var cts = new CancellationTokenSource(TimeSpan.FromSeconds(4));
			await foreach (TaskExecution _ in executor.Execute(context, yaml, cts.Token)) ;
			Assert.False(cts.IsCancellationRequested);
		}
	}
}