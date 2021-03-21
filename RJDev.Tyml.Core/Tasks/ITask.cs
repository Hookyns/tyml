using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace RJDev.Tyml.Core.Tasks
{
	public interface ITask
	{
		/// <summary>
		/// Execute task
		/// </summary>
		/// <param name="context"></param>
		/// <param name="inputs"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<TaskCompletionStatus> Execute(TaskContext context, IDictionary inputs, CancellationToken cancellationToken);
	}
}