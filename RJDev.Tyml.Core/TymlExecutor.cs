using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Outputter;
using RJDev.Tyml.Core.Tasks;
using RJDev.Tyml.Core.Yml;

namespace RJDev.Tyml.Core
{
	public class TymlExecutor
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
		public async Task<IList<TaskOutput>> Execute(TymlContext context, string ymlContent, CancellationToken cancellationToken = default)
		{
			RootConfiguration config = this.parser.Parse(ymlContent, context);
			List<TaskOutput> outputs = new(config.Steps.Count);

			// Change Console.Out & Error to NULL
			TextWriter consoleTextWriter = Console.Out;
			TextWriter consoleErrorTextWriter = Console.Error;
			Console.SetOut(TextWriter.Null);
			Console.SetError(TextWriter.Null);

			try
			{
				// List of outputs
				foreach (TaskConfiguration step in config.Steps)
				{
					await this.ExecuteTask(context, step, config, cancellationToken, outputs);
				}
			}
			finally
			{
				// Restore Console.Out & Error
				Console.SetOut(consoleTextWriter);
				Console.SetOut(consoleErrorTextWriter);
			}

			return outputs;
		}

		/// <summary>
		/// Execute task
		/// </summary>
		/// <param name="context"></param>
		/// <param name="step"></param>
		/// <param name="config"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="outputs"></param>
		/// <exception cref="InvalidOperationException"></exception>
		private async Task ExecuteTask(TymlContext context, TaskConfiguration step, RootConfiguration config, CancellationToken cancellationToken, ICollection<TaskOutput> outputs)
		{
			string taskDisplayName = step.DisplayName ?? step.Task;
			TaskInfo taskInfo = context.GetTask(step.Task);

			// Get Task instance from ServiceProvider
			if (this.serviceProvider.GetService(taskInfo.Type) is not ITask task)
			{
				throw new InvalidOperationException($"Required service '{taskInfo.Type.FullName}' not registered.");
			}

			Outputter.Outputter outputter = new(cancellationToken);

			// Construct TaskContext
			TaskContext taskContext = new(context, config.Variables, taskInfo, outputter.OutputWriter);

			// Execute task and log info about it into output
			TaskCompletionStatus status = await ExecuteTaskWithLog(step, taskContext, taskDisplayName, task, cancellationToken);

			// Complete outputter
			outputter.Complete();
			
			outputs.Add(new TaskOutput(step.Task, taskDisplayName, status, outputter.OutputReader));
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