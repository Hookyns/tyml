using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using RJDev.Tyml.Core;
using RJDev.Tyml.Tasks.Basic.Cmd;
using RJDev.Tyml.Tasks.Basic.DownloadFile;
using RJDev.Tyml.Tasks.Basic.ExtractFile;

namespace RJDev.Tyml.Tasks.Basic.Tests.Infrastructure
{
	public class TestBase
	{
		/// <summary>
		/// Return context istance
		/// </summary>
		/// <returns></returns>
		protected static TymlContext GetContext()
		{
			return new TymlContextBuilder()
				.UseTasks(typeof(CmdTask), typeof(ExtractFilesTask), typeof(DownloadFileTask))
				.UseWorkingDirectory(Path.Combine(Directory.GetCurrentDirectory(), "work-dir"))
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

			ServiceProvider provider = collection.BuildServiceProvider();
			return provider;
		}
	}
}