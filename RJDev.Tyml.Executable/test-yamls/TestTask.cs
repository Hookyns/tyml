using System.Linq;
using System.Threading.Tasks;
using RJDev.Tyml.Core;
using RJDev.Tyml.Core.Tasks;

namespace RJDev.Tyml.Executable
{
	[TymlTask("test")]
	public class TestTask : TaskBase<TestTaskConfig>
	{
		protected override Task Execute(TaskContext context, TestTaskConfig inputs)
		{
			context.Output.WriteLine($"Script: {inputs.Script} with args: {string.Join("; ", inputs.Args.Select(entry => entry.Key + ":" + entry.Value))}");
			return Task.CompletedTask;
		}
	}
}