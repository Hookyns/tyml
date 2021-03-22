using System.ComponentModel.DataAnnotations;

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
		[Required]
		public string ArchiveFilePattern { get; set; } = null!;

		/// <summary>
		/// Destination folder
		/// </summary>
		[Required]
		public string Destination { get; set; } = null!;

		/// <summary>
		/// Overwrite existing files if exists.
		/// </summary>
		public bool Overwrite { get; set; }
	}
}