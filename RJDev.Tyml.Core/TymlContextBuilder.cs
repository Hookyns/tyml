using RJDev.Tyml.Core.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RJDev.Tyml.Core
{
	public class TymlContextBuilder
	{
		private readonly List<Type> taskTypes = new();
		private string? workingDirectory;
		private readonly Dictionary<string, object> baseVariables = new();

		/// <summary>
		/// Ctor
		/// </summary>
		public TymlContextBuilder()
		{
		}

		/// <summary>
		/// Return instance of TymlContext.
		/// </summary>
		/// <returns></returns>
		public TymlContext Build()
		{
			if (this.workingDirectory == null)
			{
				throw new InvalidOperationException($"Working directory not specified in {nameof(TymlContext)}");
			}

			var tasks = GetTasks(taskTypes);

			return new TymlContext(tasks, this.workingDirectory, this.baseVariables ?? new(0));
		}

		/// <summary>
		/// Add task allowed for execution.
		/// </summary>
		/// <param name="taskTypes"></param>
		/// <returns></returns>
		public TymlContextBuilder AddTask(Type taskType)
		{
			this.taskTypes.Add(taskType);
			return this;
		}

		/// <summary>
		/// Add tasks allowed for execution.
		/// </summary>
		/// <param name="taskTypes"></param>
		/// <returns></returns>
		public TymlContextBuilder AddTasks(params Type[] taskTypes)
		{
			this.taskTypes.AddRange(taskTypes);
			return this;
		}

		/// <summary>
		/// Set working directory of processing context.
		/// </summary>
		/// <param name="workingDirectory"></param>
		/// <returns></returns>
		public TymlContextBuilder UseWorkingDirectory(string workingDirectory)
		{
			this.workingDirectory = workingDirectory;
			return this;
		}

		/// <summary>
		/// Set base variable.
		/// </summary>
		/// <param name="variables"></param>
		/// <returns></returns>
		public TymlContextBuilder WithBaseVariable(string name, object value)
		{
			this.baseVariables[name] = value;
			return this;
		}

		/// <summary>
		/// Set base variables.
		/// </summary>
		/// <param name="variables"></param>
		/// <returns></returns>
		public TymlContextBuilder WithBaseVariables(Dictionary<string, object> baseVariables)
		{
			foreach ((string name, object value) in baseVariables)
			{
				this.baseVariables[name] = value;
			}
			return this;
		}

		/// <summary>
		/// Return dictionary of YamlTask types.
		/// </summary>
		/// <param name="taskTypes"></param>
		/// <returns></returns>
		private static Dictionary<string, TaskInfo> GetTasks(IEnumerable<Type> taskTypes)
		{
			return taskTypes
				.Where(x => x.IsClass && !x.IsAbstract)
				.Select(t => new
				{
					Type = t,
					Attribute = (TymlTaskAttribute?)t.GetCustomAttributes(typeof(TymlTaskAttribute), true).FirstOrDefault()
				})
				.Where(x => x.Attribute != null)
				.ToDictionary(x => x.Attribute!.Name.ToLower(), x => new TaskInfo(x.Type, x.Attribute!));
		}
	}
}