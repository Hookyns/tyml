using System;
using System.IO;

namespace RJDev.Tyml.Core.Helpers
{
	public static class DirectoryHelper
	{
		/// <summary>
		/// Validate and normalize path of the directory.
		/// </summary>
		/// <param name="workingDirectory"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static string NormalizeDirectory(string workingDirectory)
		{
			if (!Path.IsPathFullyQualified(workingDirectory))
			{
				throw new ArgumentException("Directory path is not absolute.", nameof(workingDirectory));
			}

			return Path.GetFullPath(new Uri(workingDirectory).LocalPath);
		}
	}
}