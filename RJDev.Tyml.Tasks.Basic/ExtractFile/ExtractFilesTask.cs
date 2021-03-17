using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Tyml.Core;
using RJDev.Tyml.Core.Tasks;

namespace RJDev.Tyml.Tasks.Basic.ExtractFile
{
	[TymlTask("ExtractFiles")]
	public class ExtractFilesTask : TaskBase<ExtractFilesInputs>
	{
		protected override Task Execute(TaskContext context, ExtractFilesInputs inputs, CancellationToken _)
		{
			string[] filePaths = Directory.GetFiles(context.TymlContext.WorkingDirectory, inputs.ArchiveFilePattern);

			foreach (string filePath in filePaths)
			{
				try
				{
					string destination = ResolveDestinationPath(context, inputs, filePath);

					// Log extraction
					context.Output.WriteLine($"Extracting file {filePath} into {destination}");

					ZipFile.ExtractToDirectory(
						filePath,
						destination,
						inputs.Overwrite
					);
				}
				catch (Exception ex)
				{
					context.Output.WriteLine("ERROR: " + ex.Message + Environment.NewLine + ex.StackTrace);
				}
			}

			return Task.CompletedTask;
		}

		/// <summary>
		/// Return absolute destination path for file
		/// </summary>
		/// <param name="context"></param>
		/// <param name="inputs"></param>
		/// <param name="filePath"></param>
		/// <returns></returns>
		private static string ResolveDestinationPath(TaskContext context, ExtractFilesInputs inputs, string filePath)
		{
			string destination = inputs.Destination;

			// Resolve relative path if it is not absolute
			if (!Path.IsPathRooted(destination))
			{
				destination = Path.GetFullPath(inputs.Destination, context.TymlContext.WorkingDirectory);
			}

			// Add directory to destination path by input file name
			destination = Path.Combine(destination, Path.GetFileNameWithoutExtension(filePath));

			return destination;
		}
	}
}