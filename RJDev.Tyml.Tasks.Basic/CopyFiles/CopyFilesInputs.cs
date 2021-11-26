using System.ComponentModel.DataAnnotations;

namespace RJDev.Tyml.Tasks.Basic.CopyFiles
{
    public class CopyFilesInputs
    {
        /// <summary>
        /// List of patterns for source files and/or directories separated by lines.
        /// </summary>
        /// <remarks>
        /// Can contain base wildcard characters (* and ?).
        /// Patterns starting with ! means exclude.
        /// </remarks>
        [Required]
        public string Contents { get; set; } = null!;

        /// <summary>
        /// Source folder.
        /// </summary>
        [Required]
        public string SourceFolder { get; set; } = null!;

        /// <summary>
        /// Destination folder.
        /// </summary>
        [Required]
        public string TargetFolder { get; set; } = null!;

        /// <summary>
        /// Overwrite existing files if exists.
        /// </summary>
        public bool Overwrite { get; set; }
    }
}