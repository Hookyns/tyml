using System.Collections.Generic;
using RJDev.Outputter;

namespace RJDev.Tyml.Core.Tasks
{
	public class TaskContext
	{
		/// <summary>
		/// YAML variables.
		/// </summary>
		private readonly IDictionary<string, object> variables;
		
		/// <summary>
		/// Root YAML context object.
		/// </summary>
		public TymlContext TymlContext { get; }

		/// <summary>
		/// Task output writer.
		/// </summary>
		public OutputWriter Out { get; }

		/// <summary>
		/// Information about task
		/// </summary>
		internal TaskInfo TaskInfo { get; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="tymlContext"></param>
		/// <param name="variables"></param>
		/// <param name="taskInfo"></param>
		/// <param name="outputWriter"></param>
		public TaskContext(TymlContext tymlContext, IDictionary<string, object> variables, TaskInfo taskInfo, OutputWriter outputWriter)
		{
			this.variables = variables;
			this.TymlContext = tymlContext;
			this.TaskInfo = taskInfo;
			this.Out = outputWriter;
		}

		/// <summary>
		/// Return variable by name.
		/// </summary>
		/// <remarks>
		/// Returns YAML variable, context variable or environment variable, in this order.
		/// </remarks>
		/// <param name="variableName"></param>
		/// <returns></returns>
		public object? GetVariable(string variableName)
		{
			if (this.variables.TryGetValue(variableName, out object? variable))
			{
				return variable;
			}

			return this.TymlContext.GetVariable(variableName);
		}
	}
}