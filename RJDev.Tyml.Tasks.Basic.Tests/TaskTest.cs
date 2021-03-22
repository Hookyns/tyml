using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RJDev.Outputter.Sinks;
using RJDev.Tyml.Core;
using RJDev.Tyml.Core.Tasks;
using RJDev.Tyml.Tasks.Basic.Tests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace RJDev.Tyml.Tasks.Basic.Tests
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
      Script: 'echo Hello Tyml!'
";

			SimpleLambdaSink sink = new SimpleLambdaSink(entry => this.testOutputHelper.WriteLine(entry.ToString()));

			await foreach (TaskExecution execution in executor.Execute(context, yaml))
			{
				await execution.OutputReader.Pipe(sink);
				await execution.Completion();
			}
		}

		[Fact]
		public async Task ExtractFilesTaskTest()
		{
			IServiceProvider serviceProvider = GetServiceProvider();
			TymlContext context = GetContext();
			TymlExecutor executor = serviceProvider.GetRequiredService<TymlExecutor>();

			string yaml = @"
steps:
  - task: ExtractFiles
    displayName: 'Extract test.zip file'
    inputs:
      ArchiveFilePattern: 'te*.zip'
      Destination: './'
      Overwrite: true
";

			SimpleLambdaSink sink = new SimpleLambdaSink(entry => this.testOutputHelper.WriteLine(entry.ToString()));

			await foreach (TaskExecution execution in executor.Execute(context, yaml))
			{
				await execution.OutputReader.Pipe(sink);
				await execution.Completion();
			}
		}

		[Fact]
		public async Task MultipleTasksTest()
		{
			IServiceProvider serviceProvider = GetServiceProvider();
			TymlContext context = GetContext();
			TymlExecutor executor = serviceProvider.GetRequiredService<TymlExecutor>();

			string yaml = @"
steps:
  - task: Cmd
    displayName: 'Echo the most important message'
    inputs:
      Script: 'echo Hello Tyml!'

  - task: ExtractFiles
    displayName: 'Extract test.zip file'
    inputs:
      ArchiveFilePattern: 'tes*.zip'
      Destination: './'
      Overwrite: true
";

			SimpleLambdaSink sink = new SimpleLambdaSink(entry => this.testOutputHelper.WriteLine(entry.ToString()));

			await foreach (TaskExecution execution in executor.Execute(context, yaml))
			{
				await execution.OutputReader.Pipe(sink);
				TaskResult result = await execution.Completion();
				Assert.Equal(TaskCompletionStatus.Ok, result.Status);
			}
		}

		[Fact]
		public async Task CmdWorkDirectoryTest()
		{
			IServiceProvider serviceProvider = GetServiceProvider();
			TymlContext context = GetContext();
			TymlExecutor executor = serviceProvider.GetRequiredService<TymlExecutor>();
			string pwdCmd;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				pwdCmd = "cd";
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				// Commented out, cuz of Github Actions; I don't know how to echo pwd, all is unknown.
				// pwdCmd = "$PWD";
				return;
			}
			else
			{
				Assert.True(false, "This test is not implemented on this platform.");
				return;
			}
			
			string yaml = $@"
steps:
  - task: Cmd
    displayName: 'Echo working directory'
    inputs:
      Script: '{pwdCmd}'
";

			StringBuilder sb = new();
			SimpleLambdaSink sink = new(entry => sb.Append(entry));

			await foreach (TaskExecution execution in executor.Execute(context, yaml))
			{
				await execution.OutputReader.Pipe(sink);
				await execution.Completion();

				string workDir = Path.Combine(Directory.GetCurrentDirectory(), "work-dir");

				// Output contains PWD
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					Assert.Contains(pwdCmd + Environment.NewLine + workDir + Environment.NewLine, sb.ToString());
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					Assert.Contains(pwdCmd + Environment.NewLine + "bash: " + workDir + ":", sb.ToString());
				}
			}
		}

		[Fact]
		public async Task DownloadTasksTest()
		{
			IServiceProvider serviceProvider = GetServiceProvider();
			TymlContext context = GetContext();
			TymlExecutor executor = serviceProvider.GetRequiredService<TymlExecutor>();

			string yaml = @"
steps:
  - task: DownloadFile
    displayName: 'Download 5 MB test file'
    inputs:
      Url: 'http://212.183.159.230/5MB.zip'
      Destination: './'
";

			var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
			SimpleLambdaSink sink = new SimpleLambdaSink(entry => this.testOutputHelper.WriteLine(entry.ToString().TrimEnd('\r', '\n')));

			await foreach (TaskExecution execution in executor.Execute(context, yaml, cts.Token))
			{
				await execution.OutputReader.Pipe(sink);
				TaskResult result = await execution.Completion();
				Assert.Equal(TaskCompletionStatus.Ok, result.Status);
			}
		}

		[Fact]
		public async Task DownloadTasksTest_CreateDirectory()
		{
			IServiceProvider serviceProvider = GetServiceProvider();
			TymlContext context = GetContext();
			TymlExecutor executor = serviceProvider.GetRequiredService<TymlExecutor>();

			string yaml = @"
steps:
  - task: DownloadFile
    displayName: 'Download 5 MB test file'
    inputs:
      Url: 'https://github.com/google/googletest/archive/refs/tags/release-1.10.0.zip'
      Destination: non/existing/directory
";

			var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
			SimpleLambdaSink sink = new SimpleLambdaSink(entry => this.testOutputHelper.WriteLine(entry.ToString().TrimEnd()));

			await foreach (TaskExecution execution in executor.Execute(context, yaml, cts.Token))
			{
				await execution.OutputReader.Pipe(sink);
				TaskResult result = await execution.Completion();
				Assert.Equal(TaskCompletionStatus.Ok, result.Status);
			}
		}
	}
}