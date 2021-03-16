namespace RJDev.Tyml.Tasks.Basic.ExtractFile
{
    public class ExtractFilesInputs
    {
        /// <summary>
        /// Pattern for source archive files.
        /// </summary>
        /// <remarks>
        /// Can contain base wildcard characters (* and ?).
        /// </remarks>
        public string ArchiveFilePattern { get; set; }

        /// <summary>
        /// Destination folder
        /// </summary>
        public string Destination { get; set; }
        
        /// <summary>
        /// Overwrite existing files if exists.
        /// </summary>
        public bool Overwrite { get; set; }
    }
}