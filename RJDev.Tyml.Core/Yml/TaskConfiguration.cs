using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RJDev.Tyml.Core.Yml
{
    public class TaskConfiguration
    {
        /// <summary>
        /// Task to execute
        /// </summary>
        [Required(ErrorMessage = "Task name is required.")]
        public string Task { get; set; } = string.Empty;

        /// <summary>
        /// Display name of task
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Collection of input arguments
        /// </summary>
        public Dictionary<string, object> Inputs { get; set; } = new(0);
    }
}