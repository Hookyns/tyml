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
		protected abstract Task Execute(TaskContext context, TInputs inputs, CancellationToken cancellationToken);

		/// <summary>
		/// Implementation of "generic" execution with unspecified inputs type. 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="inputs"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task Execute(TaskContext context, IDictionary inputs, CancellationToken cancellationToken)
		{
			return this.Execute(context, (TInputs) ConfigurationParseHelper.GetObject(inputs, typeof(TInputs)), cancellationToken);
		}
	}
}