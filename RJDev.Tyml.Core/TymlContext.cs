using System;
using System.Collections.Generic;
using RJDev.Tyml.Core.Helpers;
using RJDev.Tyml.Core.Tasks;

namespace RJDev.Tyml.Core
{
	public class TymlContext
	{
		/// <summary>
		/// Tasks
		/// </summary>
		private readonly IDictionary<string, TaskInfo> tasks;

		/// <summary>
		/// Working directory used as base directory for all operations.
		/// </summary>
		public string WorkingDirectory { get; }

		/// <summary>
		/// Variables
		/// </summary>
		public IDictionary<string, object> BaseVariables { get; }

		/// <summary>
		/// Construct Tyml context
		/// </summary>
		internal TymlContext(IDictionary<string, TaskInfo> tasks, string workingDirectory, IDictionary<string, object> baseVariables)
		{
			this.tasks = tasks;
			this.WorkingDirectory = DirectoryHelper.NormalizeDirectory(workingDirectory);
			this.BaseVariables = baseVariables;
		}

		/// <summary>
		/// eturn variable by name.
		/// </summary>
		/// <remarks>
		/// Returns YAML variable, context variable or environment variable, in this order.
		/// </remarks>
		/// <param name="variableName"></param>
		/// <returns></returns>
		public object? GetVariable(string variableName)
		{
			if (this.BaseVariables.TryGetValue(variableName, out object? variable))
			{
				return variable;
			}

			return Environment.GetEnvironmentVariable(variableName);
		}

		/// <summary>
		/// Return Type of task matched by name.
		/// </summary>
		/// <param name="taskName"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		internal TaskInfo GetTask(string taskName)
		{
			if (!this.tasks.TryGetValue(taskName.ToLower(), out TaskInfo? taskType))
			{
				throw new InvalidOperationException($"YAML configuration contains unknown task '{taskName}'.");
			}

			return taskType;
		}
	}
}