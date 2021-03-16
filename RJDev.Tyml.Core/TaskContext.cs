using System.Collections.Generic;

namespace RJDev.Tyml.Core
{
    public class TaskContext
    {
        /// <summary>
        /// YAML variables
        /// </summary>
        private readonly IDictionary<string, object> variables;

        /// <summary>
        /// Root YAML context object
        /// </summary>
        public TymlContext TymlContext { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tymlContext"></param>
        /// <param name="variables"></param>
        public TaskContext(TymlContext tymlContext, IDictionary<string, object> variables)
        {
            this.variables = variables;
            this.TymlContext = tymlContext;
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
            if (this.variables.TryGetValue(variableName, out object? variable))
            {
                return variable;
            }

            return this.TymlContext.GetVariable(variableName);
        }
    }
}