using System;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Outputter;
using RJDev.Tyml.Core.Tasks;
using RJDev.Tyml.Core.Yml;

namespace RJDev.Tyml.Core
{
	public sealed class TymlExecutor
	{
		/// <summary>
		/// Width of HorizontalLine in text output of tasks
		/// </summary>
		private const int HrWidth = 80;

		/// <summary>
		/// Instance of service provider
		/// </summary>
		private readonly IServiceProvider serviceProvider;

		/// <summary>
		/// YAML config parser
		/// </summary>
		private readonly Parser parser;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="serviceProvider"></param>
		public TymlExecutor(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
			this.parser = new Parser();
		}

		/// <summary>
		/// Run processing of YAML configuration over given context
		/// </summary>
		/// <param name="context"></param>
		/// <param name="ymlContent">YAML configuration file content.</param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public TymlExecution Execute(TymlContext context, string ymlContent, CancellationToken cancellationToken = default)
		{
			RootConfiguration config = this.parser.Parse(ymlContent, context);
			return new TymlExecution(this, context, config, cancellationToken);
		}

		/// <summary>
		/// Execute task
		/// </summary>
		/// <param name="context"></param>
		/// <param name="step"></param>
		/// <param name="config"></param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="InvalidOperationException"></exception>
		internal TaskExecution ExecuteTask(TymlContext context, TaskConfiguration step, RootConfiguration config, CancellationToken cancellationToken)
		{
			string taskDisplayName = step.DisplayName ?? step.Task;
			TaskInfo taskInfo = context.GetTask(step.Task);

			// Get Task instance from ServiceProvider
			if (this.serviceProvider.GetService(taskInfo.Type) is not ITask task)
			{
				throw new InvalidOperationException($"Required service '{taskInfo.Type.FullName}' not registered.");
			}

			// Construct TaskExecution
			TaskExecution taskExecution = new(step.Task, taskDisplayName, cancellationToken);

			Task.Run(async () =>
			{
				TaskCompletionStatus status = TaskCompletionStatus.Error;
				
				try
				{
					// Construct TaskContext
					TaskContext taskContext = new(context, config.Variables, taskInfo, taskExecution.OutputWriter);

					// Execute task and log info about it into output
					status = await ExecuteTaskWithLog(step, taskContext, taskDisplayName, task, cancellationToken);
				}
				finally
				{
					// Complete TaskExecution
					taskExecution.Complete(status);
				}
			}, cancellationToken);

			return taskExecution;
		}

		/// <summary>
		/// Execute task and log informations around task
		/// </summary>
		/// <param name="step"></param>
		/// <param name="taskContext"></param>
		/// <param name="taskDisplayName"></param>
		/// <param name="task"></param>
		/// <param name="cancellationToken"></param>
		private static async Task<TaskCompletionStatus> ExecuteTaskWithLog(TaskConfiguration step, TaskContext taskContext, string taskDisplayName, ITask task, CancellationToken cancellationToken)
		{
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