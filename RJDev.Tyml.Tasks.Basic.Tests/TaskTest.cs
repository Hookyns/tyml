using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RJDev.Tyml.Core;
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
                testOutputHelper.WriteLine(taskOutput.DisplayName);
                testOutputHelper.WriteLine(taskOutput.Output);
            }
        }
    }
}