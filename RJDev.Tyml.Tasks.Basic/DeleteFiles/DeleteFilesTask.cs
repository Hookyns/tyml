using System;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Tyml.Core;
using RJDev.Tyml.Core.Tasks;
using RJDev.Tyml.Tasks.Basic.Helpers;

namespace RJDev.Tyml.Tasks.Basic.DeleteFiles
{
	[TymlTask("DeleteFiles")]
	public class DeleteFilesTask : TaskBase<DeleteFilesInputs>
	{
		/// <inheritdoc />
		protected override Task<TaskCompletionStatus> Execute(TaskContext context, DeleteFilesInputs inputs, CancellationToken cancellationToken)
		{
			(string[] includes, string[] excludes) = FileMatchHelper.GetPatterns(inputs.Contents);

			foreach (string pattern in includes)
			{
				Delete(pattern, excludes, inputs, context);
			}

			return OkSync();
		}

		private static string Normalize(string path)
		{
			return Path.GetFullPath(new Uri(path).LocalPath);
		}

		private void Delete(string pattern, string[] excludes, DeleteFilesInputs inputs, TaskContext context)
		{
			string sourceFolder = Normalize(Path.Combine(context.TymlContext.WorkingDirectory, inputs.SourceFolder));
			
			string[] files = Directory.GetFiles(sourceFolder, pattern);
			string[] directories = Directory.GetDirectories(sourceFolder, pattern);

			DeleteFiles(files, excludes, sourceFolder, context);
			DeleteDirectories(directories, excludes, sourceFolder, inputs, context);
		}

		private void DeleteFiles(string[] files, string[] excludes, string sourceFolder, TaskContext context)
		{
			foreach (string file in files)
			{
				string pathFromRoot = Path.GetRelativePath(sourceFolder, file);
				
				if (excludes.Any(excludePattern => FileSystemName.MatchesSimpleExpression(excludePattern, pathFromRoot.Replace("\\", "/"), ignoreCase: true)))
				{
					continue;
				}

				context.Out.WriteLine($"Delete file {file}");
				File.Delete(file);
			}
		}

		private void DeleteDirectories(string[] directories, string[] excludes, string sourceFolder, DeleteFilesInputs inputs, TaskContext context)
		{
			foreach (string directory in directories)
			{
				string pathFromRoot = Path.GetRelativePath(sourceFolder, directory);
				
				if (excludes.Any(excludePattern => FileSystemName.MatchesSimpleExpression(excludePattern, pathFromRoot.Replace("\\", "/"), ignoreCase: true)))
				{
					continue;
				}

				context.Out.WriteLine($"Delete directory {directory}");
				Directory.Delete(directory, inputs.Recursive);
			}
		}
	}
}