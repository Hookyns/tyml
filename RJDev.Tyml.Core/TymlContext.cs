using System;
using System.Collections.Generic;
using System.Linq;

namespace RJDev.Tyml.Core
{
    public class TymlContext
    {
        /// <summary>
        /// Working directory
        /// </summary>
        private string? workingDirectory;

        /// <summary>
        /// Tasks
        /// </summary>
        private Dictionary<string, Type> availableTasks = new(0);

        /// <summary>
        /// Working directory used as base directory for all operations.
        /// </summary>
        public string WorkingDirectory
        {
            get => this.workingDirectory ?? throw new InvalidOperationException($"Working directory not specified in {nameof(TymlContext)}");
            internal set => this.workingDirectory = value;
        }

        /// <summary>
        /// Tasks
        /// </summary>
        internal IEnumerable<Type> Tasks
        {
            set => this.availableTasks = GetTaskDictionary(value);
        }

        /// <summary>
        /// Variables
        /// </summary>
        public IDictionary<string, object> BaseVariables { get; internal set; } = new Dictionary<string, object>(0);

        /// <summary>
        /// Construct Tyml context
        /// </summary>
        internal TymlContext()
        {
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
        internal Type GetTask(string taskName)
        {
            if (!this.availableTasks.TryGetValue(taskName.ToLower(), out Type? taskType))
            {
                throw new InvalidOperationException($"YAML configuration contains unknown task '{taskName}'.");
            }

            return taskType;
        }

        /// <summary>
        /// Return dictionary of YamlTask types.
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        private static Dictionary<string, Type> GetTaskDictionary(IEnumerable<Type> enumerable)
        {
            return enumerable
                .Where(x => x.IsClass && !x.IsAbstract)
                .Select(t => new
                {
                    Type = t,
                    Attr = (TymlTaskAttribute?) t.GetCustomAttributes(typeof(TymlTaskAttribute), true).FirstOrDefault()
                })
                .Where(x => x.Attr != null)
                .ToDictionary(x => x.Attr!.Name.ToLower(), x => x.Type);
        }
    }
}