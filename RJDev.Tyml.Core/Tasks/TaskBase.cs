using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Tyml.Core.Yml;

namespace RJDev.Tyml.Core.Tasks
{
	public abstract class TaskBase<TInputs> : ITask
	{
		/// <summary>
		/// Execute task.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="inputs"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		protected abstract Task<TaskCompletionStatus> Execute(TaskContext context, TInputs inputs, CancellationToken cancellationToken);

		/// <summary>
		/// Implementation of "generic" execution with unspecified inputs type. 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="inputs"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<TaskCompletionStatus> Execute(TaskContext context, IDictionary inputs, CancellationToken cancellationToken)
		{
			return this.Execute(context, (TInputs) ConfigurationParseHelper.GetObject(inputs, typeof(TInputs)), cancellationToken);
		}

		/// <summary>
		/// Returns OK status
		/// </summary>
		/// <returns></returns>
		protected TaskCompletionStatus Ok()
		{
			return TaskCompletionStatus.Ok;
		}

		/// <summary>
		/// Returns Error status
		/// </summary>
		/// <returns></returns>
		protected TaskCompletionStatus Error()
		{
			return TaskCompletionStatus.Error;
		}

		/// <summary>
		/// Returns OK status
		/// </summary>
		/// <returns></returns>
		protected Task<TaskCompletionStatus> OkSync()
		{
			return CompletedTaskResults.CompletedOk;
		}
		
		/// <summary>
		/// Returns Error status
		/// </summary>
		/// <returns></returns>
		protected Task<TaskCompletionStatus> ErrorSync()
		{
			return CompletedTaskResults.CompletedError;
		}
	}
}