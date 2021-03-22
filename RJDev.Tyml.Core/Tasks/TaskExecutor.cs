using System;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Outputter;
using RJDev.Tyml.Core.Yml;

namespace RJDev.Tyml.Core.Tasks
{
	public class TaskExecutor
	{
		/// <summary>
		/// Width of HorizontalLine in text output of tasks
		/// </summary>
		private const int HrWidth = 100;

		private readonly IServiceProvider serviceProvider;
		private readonly TymlContext context;
		private readonly RootConfiguration rootConfiguration;

		public TaskExecutor(IServiceProvider serviceProvider, TymlContext context, RootConfiguration rootConfiguration)
		{
			this.serviceProvider = serviceProvider;
			this.context = context;
			this.rootConfiguration = rootConfiguration;
		}

		/// <summary>
		/// Execute given step.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public TaskExecution Execute(TaskConfiguration step, CancellationToken cancellationToken)
		{
			TaskInfo taskInfo = this.context.GetTask(step.Task);

			// Get Task instance from ServiceProvider
			if (this.serviceProvider.GetService(taskInfo.Type) is not ITask task)
			{
				throw new InvalidOperationException($"Required service '{taskInfo.Type.FullName}' not registered in service collection.");
			}

			// Construct TaskExecution
			TaskExecution taskExecution = new(step, cancellationToken);

			// Construct TaskContext
			TaskContext taskContext = new(this.context, this.rootConfiguration.Variables, taskInfo, taskExecution.OutputWriter);

			// Create executing task
			Task<Task<TaskCompletionStatus>> executionTask = CreateExecutionTask(step, taskContext, task, cancellationToken);

			taskExecution.BindTask(executionTask.Unwrap());

			return taskExecution;
		}

		/// <summary>
		/// Create system Task running Tyml Task.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="taskContext"></param>
		/// <param name="task"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private static Task<Task<TaskCompletionStatus>> CreateExecutionTask(
			TaskConfiguration step,
			TaskContext taskContext,
			ITask task,
			CancellationToken cancellationToken
		)
		{
			return Task.Factory.StartNew<Task<TaskCompletionStatus>>(
				async () => await ExecuteTaskWithLog(step, taskContext, task, cancellationToken).ConfigureAwait(false),
				cancellationToken,
				TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach,
				TaskScheduler.Default
			);
		}

		/// <summary>
		/// Execute task and log informations around task.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="taskContext"></param>
		/// <param name="task"></param>
		/// <param name="cancellationToken"></param>
		private static async Task<TaskCompletionStatus> ExecuteTaskWithLog(TaskConfiguration step, TaskContext taskContext, ITask task, CancellationToken cancellationToken)
		{
			string taskDisplayName = step.DisplayName ?? step.Task;
			
			// Starting
			taskContext.Out.WriteLine($"Starting: {taskDisplayName}", EntryType.Success);
			taskContext.Out.WriteLine("=".PadLeft(HrWidth, '='), EntryType.Minor);
			taskContext.Out.WriteLine($"Task: {taskContext.TaskInfo.Attribute.Name}", EntryType.Minor);
			taskContext.Out.WriteLine($"Description: {taskContext.TaskInfo.Attribute.Description}", EntryType.Minor);
			taskContext.Out.WriteLine("=".PadLeft(HrWidth, '='), EntryType.Minor);

			TaskCompletionStatus status = await task.Execute(taskContext, step.Inputs, cancellationToken);

			// Finishing
			taskContext.Out.WriteLine("=".PadLeft(HrWidth, '='), EntryType.Minor);
			taskContext.Out.WriteLine($"Finishing: {taskDisplayName}", status == TaskCompletionStatus.Ok ? EntryType.Success : EntryType.Error);

			return status;
		}
	}
}