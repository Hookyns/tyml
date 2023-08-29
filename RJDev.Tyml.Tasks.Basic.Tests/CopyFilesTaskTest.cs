using System;
using System.IO;
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
	public class CopyFilesTaskTest : TestBase
	{
		private readonly ITestOutputHelper testOutputHelper;

		public CopyFilesTaskTest(ITestOutputHelper testOutputHelper)
		{
			this.testOutputHelper = testOutputHelper;
		}

		protected IOutputterSink GetTestOutputSink()
		{
			return new SimpleLambdaSink(entry => testOutputHelper.WriteLine(entry.ToString().TrimEnd('\r', '\n')));
		}
		
		[Fact]
		public async Task CopyFilesTest()
		{
			IServiceProvider serviceProvider = GetServiceProvider();
			TymlContext context = GetContext();
			TymlExecutor executor = serviceProvider.GetRequiredService<TymlExecutor>();

			string yaml = @"
steps:
  - task: CopyFiles
    inputs:
      SourceFolder: ../../test-files/
      Contents: |-
        *
        !nope
        !*.rt?
        !nope.*
      TargetFolder: ./copy
      Overwrite: true
";

			IOutputterSink sink = GetTestOutputSink();

			await foreach (TaskExecution execution in executor.Execute(context, yaml))
			{
				await execution.OutputReader.Pipe(sink);
				await execution.Completion();
			}

			string targetDir = Path.Combine(context.WorkingDirectory, "copy");
			
			Assert.True(File.Exists(Path.Combine(targetDir, "file.txt")));
			Assert.True(File.Exists(Path.Combine(targetDir, "file2.txt")));
			Assert.False(File.Exists(Path.Combine(targetDir, "nope.rtf")));
			Assert.False(File.Exists(Path.Combine(targetDir, "nope.txt")));
			Assert.True(Directory.Exists(Path.Combine(targetDir, "sub")));
			Assert.True(Directory.Exists(Path.Combine(targetDir, "sub2")));
			Assert.False(Directory.Exists(Path.Combine(targetDir, "nope")));
			
			Assert.True(File.Exists(Path.Combine(targetDir, "sub", "file.txt")));
			Assert.True(File.Exists(Path.Combine(targetDir, "sub", "file2.txt")));
			Assert.True(Directory.Exists(Path.Combine(targetDir, "sub", "dir")));
			Assert.True(File.Exists(Path.Combine(targetDir, "sub", "dir", "file.txt")));
			Assert.True(File.Exists(Path.Combine(targetDir, "sub", "dir", "file2.txt")));
		}
	}
}