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
		public async Task Execute(TaskContext context, IDictionary inputs, CancellationToken cancellationToken)
		{
			await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
		}
	}
}