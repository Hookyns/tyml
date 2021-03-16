using System;
using System.Collections.Generic;

namespace RJDev.Tyml.Core
{
    public class TymlContextBuilder
    {
        /// <summary>
        /// Context being created.
        /// </summary>
        private readonly TymlContext context;

        /// <summary>
        /// Ctor
        /// </summary>
        public TymlContextBuilder()
        {
            this.context = new TymlContext();
        }

        /// <summary>
        /// Set tasks allowed for execution.
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public TymlContextBuilder UseTasks(params Type[] tasks)
        {
            this.context.Tasks = tasks;
            return this;
        }
        
        /// <summary>
        /// Set working directory of processing context.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        public TymlContextBuilder UseWorkingDirectory(string workingDirectory)
        {
            this.context.WorkingDirectory = workingDirectory;
            return this;
        }
        
        /// <summary>
        /// Set base variables.
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        public TymlContextBuilder WithBaseVariables(IDictionary<string, object> variables)
        {
            this.context.BaseVariables = variables;
            return this;
        }

        /// <summary>
        /// Return instance of TymlContext.
        /// </summary>
        /// <returns></returns>
        public TymlContext Build()
        {
            return this.context;
        }
    }
}