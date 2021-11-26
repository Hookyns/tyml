using System;
using System.IO;

namespace RJDev.Tyml.Tasks.Basic.Tests.Infrastructure
{
	public sealed class WorkDirFixture : IDisposable
	{
		public static readonly WorkDirFixture Current = new();

		private WorkDirFixture()
		{
			string workingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "work-dir");
			Directory.Delete(workingDirectory, true);
			Directory.CreateDirectory(workingDirectory);
		}

		~WorkDirFixture()
		{
			Dispose();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			// Run at end
		}

		public static void Fix()
		{
			WorkDirFixture _ = Current;
		}
	}
}