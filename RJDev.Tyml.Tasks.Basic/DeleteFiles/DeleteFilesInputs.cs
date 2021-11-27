using System.ComponentModel.DataAnnotations;

namespace RJDev.Tyml.Tasks.Basic.DeleteFiles
{
    public class DeleteFilesInputs
    {
        /// <summary>
        /// Source folder.
        /// </summary>
        [Required]
        public string SourceFolder { get; set; } = null!;
        
        /// <summary>
        /// List of patterns for source files and/or directories separated by lines.
        /// </summary>
        /// <remarks>
        /// Can contain base wildcard characters (* and ?).
        /// Patterns starting with ! means exclude.
        /// </remarks>
        public string Contents { get; set; } = null!;
        
        /// <summary>
        /// Delete folders recursively. It means delete folders even they have content.
        /// </summary>
        public bool Recursive { get; set; }
    }
}