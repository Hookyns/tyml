using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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

			string yaml = @"
steps:
  - task: Cmd
    displayName: 'Echo the most important message'
    inputs:
      Script: 'echo Hello World!'
";

			var results = await executor.Execute(context, yaml);

			foreach (TaskOutput taskOutput in results)
			{
				this.testOutputHelper.WriteLine(taskOutput.Output);
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

			var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

			await Assert.ThrowsAsync<TaskCanceledException>(async () => { await executor.Execute(context, yaml, cts.Token); });
		}
	}
}