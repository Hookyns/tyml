using System;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Outputter;
using RJDev.Tyml.Core.Yml;

namespace RJDev.Tyml.Core.Tasks
{
	public sealed class TaskExecution
	{
		private readonly TaskCompletionSource<TaskResult> taskCompletionSource = new();
		private readonly Outputter.Outputter outputter;

		/// <summary>
		/// Display name of task
		/// </summary>
		public string? DisplayName { get; }

		/// <summary>
		/// Name identifier of task
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Instance of output writer.
		/// </summary>
		internal OutputWriter OutputWriter => outputter.OutputWriter;

		/// <summary>
		/// Instance of output reader.
		/// </summary>
		public OutputReader OutputReader => outputter.OutputReader;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="step"></param>
		/// <param name="cancellationToken"></param>
		public TaskExecution(TaskConfiguration step, CancellationToken cancellationToken)
		{
			Name = step.Task;
			DisplayName = step.DisplayName;
			outputter = new Outputter.Outputter(cancellationToken);

			cancellationToken.Register(() =>
			{
				outputter.Complete();
				taskCompletionSource.TrySetCanceled();
			});
		}

		/// <summary>
		/// Return Task's completion task.
		/// </summary>
		/// <returns></returns>
		// ReSharper disable once UnusedMethodReturnValue.Global
		public async Task<TaskResult> Completion()
		{
			return await taskCompletionSource.Task.ConfigureAwait(false);
		}

		/// <summary>
		/// Bind executing task
		/// </summary>
		/// <param name="executingTask"></param>
		public void BindTask(Task<TaskCompletionStatus> executingTask)
		{
			executingTask.ContinueWith(task =>
			{
				// Complete Outputter
				outputter.Complete();

				// Should happen only on system failure; Tyml Task should not throw => so delegate this throw, it's maybe important.
				if (task.IsFaulted)
				{
					taskCompletionSource.TrySetException(task.Exception ?? new Exception("Unknown Tyml task failure."));
					return;
				}

				// Delegate cancellation
				if (task.IsCanceled)
				{
					taskCompletionSource.TrySetCanceled();
					return;
				}

				taskCompletionSource.SetResult(new TaskResult(Name, DisplayName, task.Result, OutputReader));
			});
		}
	}
}