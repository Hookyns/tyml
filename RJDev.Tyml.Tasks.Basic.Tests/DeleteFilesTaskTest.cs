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
	public class DeleteFilesTaskTest : TestBase
	{
		private readonly ITestOutputHelper testOutputHelper;

		public DeleteFilesTaskTest(ITestOutputHelper testOutputHelper)
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
      Contents: '*'
      TargetFolder: .

  - task: DeleteFiles
    inputs:
      SourceFolder: .
      Contents: |-
        nope
        sub2
        sub/*
        !sub/dir
        !sub/dir/*
        nope.*
        !nope.r??
      Recursive: true

";

			IOutputterSink sink = GetTestOutputSink();

			await foreach (TaskExecution execution in executor.Execute(context, yaml))
			{
				await execution.OutputReader.Pipe(sink);
				await execution.Completion();
			}
			
			Assert.False(Directory.Exists(Path.Combine(context.WorkingDirectory, "nope")));
			Assert.False(Directory.Exists(Path.Combine(context.WorkingDirectory, "sub2")));
			
			Assert.True(Directory.Exists(Path.Combine(context.WorkingDirectory, "sub")));
			Assert.True(Directory.Exists(Path.Combine(context.WorkingDirectory, "sub", "dir")));
			Assert.False(File.Exists(Path.Combine(context.WorkingDirectory, "sub", "file.txt")));
			Assert.False(File.Exists(Path.Combine(context.WorkingDirectory, "sub", "file2.txt")));
			Assert.True(File.Exists(Path.Combine(context.WorkingDirectory, "sub", "dir", "file.txt")));
			
			Assert.True(File.Exists(Path.Combine(context.WorkingDirectory, "nope.rtf")));
			Assert.True(File.Exists(Path.Combine(context.WorkingDirectory, "file.txt")));
			Assert.True(File.Exists(Path.Combine(context.WorkingDirectory, "file2.txt")));
		}
	}
}