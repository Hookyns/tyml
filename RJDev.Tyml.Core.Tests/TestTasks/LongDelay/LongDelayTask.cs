using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Tyml.Core.Tasks;

namespace RJDev.Tyml.Core.Tests.TestTasks.LongDelay
{
	[TymlTask("LongDelay")]
	public class LongDelayTask : ITask
	{
		public async Task<TaskCompletionStatus> Execute(TaskContext context, IDictionary inputs, CancellationToken cancellationToken)
		{
			await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
			return TaskCompletionStatus.Ok;
		}
	}
}