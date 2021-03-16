using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RJDev.Tyml.Core.Yml;

namespace RJDev.Tyml.Core
{
    public class TymlExecutor
    {
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
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IList<TaskOutput>> Execute(TymlContext context, string ymlContent)
        {
            RootConfiguration config = this.parser.Parse(ymlContent, context);
            
            // Change Console.Out to NULL
            TextWriter consoleTextWriter = Console.Out;
            Console.SetOut(TextWriter.Null);

            // List of outputs
            List<TaskOutput> outputs = new(config.Steps.Count);
            
            foreach (TaskConfiguration step in config.Steps)
            {
                Type taskType = context.GetTask(step.Task);
                ITask task = (ITask)(this.serviceProvider.GetService(taskType) ?? throw new InvalidOperationException($"Required service '{taskType.FullName}' not registered."));
                
                TaskContext taskContext = new(context, config.Variables);
                await task.Execute(taskContext, step.Inputs);
                
                outputs.Add(new TaskOutput(step.Task, step.DisplayName, taskContext.OutputStringBuilder.ToString()));
            }
            
            // Restore Console.Out
            Console.SetOut(consoleTextWriter);

            return outputs;
        }
    }
}