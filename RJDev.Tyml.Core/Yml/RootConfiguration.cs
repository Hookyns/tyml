using System.Collections.Generic;

namespace RJDev.Tyml.Core.Yml
{
    public class RootConfiguration : VariablesConfiguration
    {
        /// <summary>
        /// Description of script
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Steps to do
        /// </summary>
        public IList<TaskConfiguration> Steps { get; set; } = new TaskConfiguration[0];
    }
}