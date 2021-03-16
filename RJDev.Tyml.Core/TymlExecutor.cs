using System;
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
        public async Task Execute(TymlContext context, string ymlContent)
        {
            RootConfiguration config = this.parser.Parse(ymlContent, context);

            foreach (TaskConfiguration step in config.Steps)
            {
                Type taskType = context.GetTask(step.Task);
                ITask task = (ITask)(this.serviceProvider.GetService(taskType) ?? throw new InvalidOperationException($"Required service '{taskType.FullName}' not registered."));
                await task.Execute(new TaskContext(context, config.Variables), step.Inputs);
            }
        }
    }
}