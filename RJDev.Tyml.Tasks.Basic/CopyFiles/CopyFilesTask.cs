using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Tyml.Core;
using RJDev.Tyml.Core.Tasks;
using RJDev.Tyml.Tasks.Basic.Helpers;

namespace RJDev.Tyml.Tasks.Basic.CopyFiles
{
	[TymlTask("CopyFiles")]
	public class CopyFilesTask : TaskBase<CopyFilesInputs>
	{
		/// <summary>
		/// Set of directories ensured that exist.
		/// </summary>
		private readonly HashSet<string> ensuredTargetDirectories = new();

		/// <inheritdoc />
		protected override Task<TaskCompletionStatus> Execute(TaskContext context, CopyFilesInputs inputs, CancellationToken cancellationToken)
		{
			(string[] includes, string[] excludes) = FileMatchHelper.GetPatterns(inputs.Contents);

			foreach (string pattern in includes)
			{
				this.Copy(pattern, excludes, inputs, context);
			}

			return this.OkSync();
		}

		private static string Normalize(string path)
		{
			return Path.GetFullPath(new Uri(path).LocalPath);
		}

		private void Copy(string pattern, string[] excludes, CopyFilesInputs inputs, TaskContext context)
		{
			string sourceFolder = Normalize(Path.Combine(context.TymlContext.WorkingDirectory, inputs.SourceFolder));
			string destination = Normalize(Path.Combine(context.TymlContext.WorkingDirectory, inputs.TargetFolder));
			
			string[] files = Directory.GetFiles(sourceFolder, pattern);
			string[] directories = Directory.GetDirectories(sourceFolder, pattern);

			this.CopyFiles(files, excludes, sourceFolder, destination, inputs, context);
			this.CopyDirectories(directories, excludes, sourceFolder, destination, inputs, context);
		}

		private void CopyFiles(string[] files, string[] excludes, string sourceFolder, string destination, CopyFilesInputs inputs, TaskContext context)
		{
			foreach (string file in files)
			{
				string pathFromRoot = Path.GetRelativePath(sourceFolder, file);
				
				if (excludes.Any(excludePattern => FileSystemName.MatchesSimpleExpression(excludePattern, pathFromRoot.Replace("\\", "/"), ignoreCase: true)))
				{
					continue;
				}

				string targetFilePath = Path.GetFullPath(new Uri(Path.Combine(destination, pathFromRoot)).LocalPath);
				string fileDirectory = Path.GetDirectoryName(targetFilePath) ?? throw new Exception("Invalid path. Unable to get directory name.");

				context.Out.WriteLine($"Copy file {file}\n\tto {targetFilePath}");

				// Check in memory, to prevent multiple IO operations
				if (!this.ensuredTargetDirectories.Contains(file))
				{
					Directory.CreateDirectory(fileDirectory);
					this.ensuredTargetDirectories.Add(fileDirectory);
				}

				File.Copy(file, targetFilePath, inputs.Overwrite);
			}
		}

		private void CopyDirectories(string[] directories, string[] excludes, string sourceFolder, string destination, CopyFilesInputs inputs, TaskContext context)
		{
			foreach (string directory in directories)
			{
				string pathFromRoot = Path.GetRelativePath(sourceFolder, directory);

				if (excludes.Any(excludePattern => FileSystemName.MatchesSimpleExpression(excludePattern, pathFromRoot.Replace("\\", "/"), ignoreCase: true)))
				{
					continue;
				}

				string targetDirectoryPath = Path.GetFullPath(new Uri(Path.Combine(destination, pathFromRoot)).LocalPath);
				context.Out.WriteLine($"Copy directory {directory}\n\tto {targetDirectoryPath}");
				
				this.CopyDirectory(directory, targetDirectoryPath, excludes, inputs);
			}
		}

		private void CopyDirectory(string sourceDirectory, string targetDirectory, string[] excludes, CopyFilesInputs inputs)
		{
			Directory.CreateDirectory(targetDirectory);
			
			string[] files = Directory.GetFiles(sourceDirectory);
			string[] directories = Directory.GetDirectories(sourceDirectory);

			foreach (string file in files)
			{
				File.Copy(file, Path.Join(targetDirectory, Path.GetFileName(file)), inputs.Overwrite);
			}

			foreach (string directory in directories)
			{
				this.CopyDirectory(directory, Path.Join(targetDirectory, Path.GetFileName(directory)), excludes, inputs);
			}
		}
	}
}