using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Tyml.Core;
using RJDev.Tyml.Core.Tasks;

namespace RJDev.Tyml.Executable
{
	[TymlTask("test")]
	public class TestTask : TaskBase<TestTaskConfig>
	{
		protected override Task<TaskCompletionStatus> Execute(TaskContext context, TestTaskConfig inputs, CancellationToken cancellationToken)
		{
			context.Out.WriteLine($"Script: {inputs.Script} with args: {string.Join("; ", inputs.Args.Select(entry => entry.Key + ":" + entry.Value))}");
			return this.OkSync();
		}
	}
}