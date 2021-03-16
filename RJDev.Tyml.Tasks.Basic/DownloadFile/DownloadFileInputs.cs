namespace RJDev.Tyml.Tasks.Basic.DownloadFile
{
    public class DownloadFileInputs
    {
        /// <summary>
        /// Url address of file which should be downladed.
        /// </summary>
        public string Url { get; set; }
        
        /// <summary>
        /// Destination folder
        /// </summary>
        public string Destination { get; set; }
    }
}