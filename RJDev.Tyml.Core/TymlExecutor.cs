using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

			// Change Console.Out to NULL
			TextWriter consoleTextWriter = Console.Out;
			Console.SetOut(TextWriter.Null);

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
				// Restore Console.Out
				Console.SetOut(consoleTextWriter);
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
			ITask task = (ITask) (this.serviceProvider.GetService(taskInfo.Type)
				?? throw new InvalidOperationException($"Required service '{taskInfo.Type.FullName}' not registered."));

			// Construct TaskContext
			TaskContext taskContext = new(context, config.Variables, taskInfo);

			// Execute task and log info about it into output
			await ExecuteTaskWithLog(step, taskContext, taskDisplayName, task, cancellationToken);

			outputs.Add(new TaskOutput(step.Task, taskDisplayName, taskContext.OutputStringBuilder.ToString()));
		}

		/// <summary>
		/// Execute task and log informations around task
		/// </summary>
		/// <param name="step"></param>
		/// <param name="taskContext"></param>
		/// <param name="taskDisplayName"></param>
		/// <param name="task"></param>
		/// <param name="cancellationToken"></param>
		private static async Task ExecuteTaskWithLog(TaskConfiguration step, TaskContext taskContext, string taskDisplayName, ITask task, CancellationToken cancellationToken)
		{
			// Starting
			await taskContext.Output.WriteLineAsync("Starting: " + taskDisplayName);
			await taskContext.Output.WriteLineAsync("=".PadLeft(HrWidth, '='));
			await taskContext.Output.WriteLineAsync("Task: " + taskContext.TaskInfo.Attribute.Name);
			await taskContext.Output.WriteLineAsync("Description: " + taskContext.TaskInfo.Attribute.Description);
			await taskContext.Output.WriteLineAsync("=".PadLeft(HrWidth, '='));

			await task.Execute(taskContext, step.Inputs, cancellationToken);

			// Finishing
			await taskContext.Output.WriteLineAsync("=".PadLeft(HrWidth, '='));
			await taskContext.Output.WriteLineAsync("Finishing: " + taskDisplayName);
		}
	}
}