using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Outputter;
using RJDev.Tyml.Core;
using RJDev.Tyml.Core.Tasks;

namespace RJDev.Tyml.Tasks.Basic.DownloadFile
{
	[TymlTask("DownloadFile")]
	public class DownloadFileTask : TaskBase<DownloadFileInputs>
	{
		protected override Task<TaskCompletionStatus> Execute(TaskContext context, DownloadFileInputs inputs, CancellationToken cancellationToken)
		{
			using WebClient client = new();
			TaskCompletionSource<TaskCompletionStatus> tcs = new();

			try
			{
				int percentageCompleted = 0;

				// Download events event
				client.DownloadFileCompleted += (_, args) =>
				{
					if (args.Cancelled)
					{
						tcs.SetResult(TaskCompletionStatus.Error);
					}
					else if (args.Error != null)
					{
						context.Out.WriteLine("File download failed.", EntryType.Error);
						context.Out.WriteLine(args.Error.Message, EntryType.Error);
						context.Out.WriteLine(args.Error.StackTrace ?? string.Empty, EntryType.Minor);
						tcs.SetResult(TaskCompletionStatus.Error);
					}
					else
					{
						tcs.SetResult(TaskCompletionStatus.Ok);
					}
				};
				client.DownloadProgressChanged += (sender, progressArgs) =>
				{
					if (progressArgs.ProgressPercentage > percentageCompleted + 10)
					{
						percentageCompleted = progressArgs.ProgressPercentage;

						// Write into output
						context.Out.WriteLine($"Progress: {progressArgs.BytesReceived / 1000.0:N2}/{progressArgs.TotalBytesToReceive / 1000.0:N2} kB", EntryType.Minor);
					}
				};

				Uri fileUri = new(inputs.Url);
				string destination = ResolveDestinationPath(context, inputs, fileUri);

				// Ensure destination directory
				if (Path.GetDirectoryName(destination) is string destinationDirectory)
				{
					Directory.CreateDirectory(destinationDirectory);
				}

				// Write into output
				context.Out.WriteLine($"Downloading file {fileUri} into {destination}");

				// Register cance action
				cancellationToken.Register(client.CancelAsync);

				// Download
				client.DownloadFileAsync(fileUri, destination);
			}
			catch (Exception ex)
			{
				context.Out.WriteLine("File download failed.", EntryType.Error);
				context.Out.WriteLine(ex.Message, EntryType.Error);
				context.Out.WriteLine(ex.StackTrace ?? string.Empty, EntryType.Minor);
				tcs.SetResult(TaskCompletionStatus.Error);
			}

			return tcs.Task;
		}

		/// <summary>
		/// Return absolute destination path for file
		/// </summary>
		/// <param name="context"></param>
		/// <param name="inputs"></param>
		/// <param name="fileUrl"></param>
		/// <returns></returns>
		private static string ResolveDestinationPath(TaskContext context, DownloadFileInputs inputs, Uri fileUrl)
		{
			string destination = inputs.Destination;

			// Resolve relative path if it is not absolute
			if (!Path.IsPathRooted(destination))
			{
				destination = Path.GetFullPath(destination, context.TymlContext.WorkingDirectory);
			}

			// Add directory to destination path by input file name
			destination = Path.Combine(destination, Path.GetFileName(fileUrl.LocalPath));

			return destination;
		}
	}
}