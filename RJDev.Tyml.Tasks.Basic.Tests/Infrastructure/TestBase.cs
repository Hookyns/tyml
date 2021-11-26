using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using RJDev.Tyml.Core;
using RJDev.Tyml.Tasks.Basic.Cmd;
using RJDev.Tyml.Tasks.Basic.CopyFiles;
using RJDev.Tyml.Tasks.Basic.DownloadFile;
using RJDev.Tyml.Tasks.Basic.ExtractFile;

namespace RJDev.Tyml.Tasks.Basic.Tests.Infrastructure
{
	public class TestBase
	{
		public TestBase()
		{
			WorkDirFixture.Fix();
		}

		/// <summary>
		/// Return context istance
		/// </summary>
		/// <returns></returns>
		protected static TymlContext GetContext()
		{
			string workingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "work-dir", Guid.NewGuid().ToString());
			Directory.CreateDirectory(workingDirectory);

			return new TymlContextBuilder()
				.AddTasks(typeof(CmdTask), typeof(ExtractFilesTask), typeof(DownloadFileTask), typeof(CopyFilesTask))
				.UseWorkingDirectory(workingDirectory)
				.WithBaseVariables(new Dictionary<string, object>()
				{
					{"foo", 5},
					{"bar", "string"},
					{"lipsum", "Lorem Ipsum dolor sit amet."},
					{"assemblyVersion", typeof(TestBase).Assembly.GetName().Version?.ToString() ?? string.Empty},
				})
				.Build();
		}

		/// <summary>
		/// Prepare IServiceProvider
		/// </summary>
		/// <returns></returns>
		protected static IServiceProvider GetServiceProvider()
		{
			ServiceCollection collection = new();
			collection.AddSingleton<TymlExecutor>();

			collection.AddTransient<CmdTask>();
			collection.AddTransient<ExtractFilesTask>();
			collection.AddTransient<DownloadFileTask>();
			collection.AddTransient<CopyFilesTask>();

			ServiceProvider provider = collection.BuildServiceProvider();
			return provider;
		}
	}
}