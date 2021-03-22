using System.Threading;
using System.Threading.Tasks;
using RJDev.Outputter;

namespace RJDev.Tyml.Core.Tasks
{
	public sealed class TaskExecution
	{
		private readonly TaskCompletionSource<TaskResult> taskCompletionSource = new();
		private readonly Outputter.Outputter outputter;

		/// <summary>
		/// Display name of task
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// Name identifier of task
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Instance of output writer.
		/// </summary>
		internal OutputWriter OutputWriter => this.outputter.OutputWriter;

		/// <summary>
		/// Instance of output reader.
		/// </summary>
		public OutputReader OutputReader => this.outputter.OutputReader;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="name"></param>
		/// <param name="displayName"></param>
		/// <param name="cancellationToken"></param>
		public TaskExecution(string name, string displayName, CancellationToken cancellationToken)
		{
			this.Name = name;
			this.DisplayName = displayName;
			this.outputter = new(cancellationToken);

			cancellationToken.Register(() =>
			{
				this.outputter.Complete();
				this.taskCompletionSource.TrySetCanceled();
			});
		}

		/// <summary>
		/// Return Task's completion task.
		/// </summary>
		/// <returns></returns>
		public Task<TaskResult> Completion()
		{
			return this.taskCompletionSource.Task;
		}

		/// <summary>
		/// Complete task execution
		/// </summary>
		/// <param name="taskResult"></param>
		internal void Complete(TaskCompletionStatus taskResult)
		{
			this.outputter.Complete();
			this.taskCompletionSource.SetResult(new TaskResult(this.Name, this.DisplayName, taskResult, this.OutputReader));
		}
	}
}