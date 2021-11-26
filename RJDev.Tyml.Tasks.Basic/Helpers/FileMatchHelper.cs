using System.Collections.Generic;
using System.IO;

namespace RJDev.Tyml.Tasks.Basic.Helpers
{
	public static class FileMatchHelper
	{
		/// <summary>
		/// Splits string with patterns into collections of include and exclude patterns.
		/// </summary>
		/// <param name="contents"></param>
		/// <returns></returns>
		public static (string[] includes, string[] excludes) GetPatterns(string contents)
		{
			List<string> includes = new();
			List<string> excludes = new();

			using (StringReader sr = new(contents))
			{
				string? pattern;
				while ((pattern = sr.ReadLine()) != null)
				{
					pattern = pattern.Trim();

					if (pattern == string.Empty)
					{
						continue;
					}

					if (pattern.StartsWith('!'))
					{
						excludes.Add(pattern[1..]);
					}
					else
					{
						includes.Add(pattern);
					}
				}
			}

			return (includes.ToArray(), excludes.ToArray());
		}
	}
}