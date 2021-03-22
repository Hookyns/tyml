using System.ComponentModel.DataAnnotations;

namespace RJDev.Tyml.Tasks.Basic.DownloadFile
{
	public class DownloadFileInputs
	{
		/// <summary>
		/// Url address of file which should be downladed.
		/// </summary>
		[Required]
		public string Url { get; set; } = null!;

		/// <summary>
		/// Destination folder
		/// </summary>
		[Required]
		public string Destination { get; set; } = null!;
	}
}