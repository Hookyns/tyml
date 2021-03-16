using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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

            var results = await executor.Execute(context, yaml);

            foreach (TaskOutput taskOutput in results)
            {
                testOutputHelper.WriteLine(taskOutput.Output);
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

            var results = await executor.Execute(context, yaml);

            foreach (TaskOutput taskOutput in results)
            {
                testOutputHelper.WriteLine(taskOutput.Output);
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
      ArchiveFilePattern: '*.zip'
      Destination: './'
      Overwrite: true
";

            var results = await executor.Execute(context, yaml);

            foreach (TaskOutput taskOutput in results)
            {
                testOutputHelper.WriteLine(taskOutput.Output);
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
    displayName: 'Download 10 MB test file'
    inputs:
      Url: 'http://212.183.159.230/10MB.zip'
      Destination: './'
";

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            var results = await executor.Execute(context, yaml, cts.Token);

            foreach (TaskOutput taskOutput in results)
            {
                testOutputHelper.WriteLine(taskOutput.Output);
            }
        }
    }
}