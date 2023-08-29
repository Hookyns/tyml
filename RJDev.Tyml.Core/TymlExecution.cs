using System;
using System.Collections.Generic;
using System.Threading;
using RJDev.Tyml.Core.Tasks;
using RJDev.Tyml.Core.Yml;

namespace RJDev.Tyml.Core
{
	public sealed class TymlExecution : IAsyncEnumerable<TaskExecution>
	{
		/// <summary>
		/// YAML root configuration.
		/// </summary>
		private readonly RootConfiguration rootConfiguration;

		/// <summary>
		/// Cancellation token.
		/// </summary>
		private readonly CancellationToken cancellationToken;

		/// <summary>
		/// Task executor.
		/// </summary>
		private readonly TaskExecutor taskExecutor;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <param name="context"></param>
		/// <param name="rootConfiguration"></param>
		/// <param name="cancellationToken"></param>
		public TymlExecution(
			IServiceProvider serviceProvider,
			TymlContext context,
			RootConfiguration rootConfiguration,
			CancellationToken cancellationToken
		)
		{
			this.rootConfiguration = rootConfiguration;
			this.cancellationToken = cancellationToken;

			// Create task executor to run all the steps in YAML file
			taskExecutor = new TaskExecutor(serviceProvider, context, this.rootConfiguration);
		}

		/// <summary>
		/// Return async enumerable.
		/// </summary>
		/// <param name="enumeratorCancellationToken"></param>
		/// <returns></returns>
		public async IAsyncEnumerator<TaskExecution> GetAsyncEnumerator(CancellationToken enumeratorCancellationToken = default)
		{
			using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, enumeratorCancellationToken);

			foreach (TaskConfiguration step in rootConfiguration.Steps)
			{
				if (cts.Token.IsCancellationRequested)
				{
					yield break;
				}

				TaskExecution execution = taskExecutor.Execute(step, cts.Token);
				yield return execution;
				await execution.Completion();
			}
		}

		// TODO: Make TymlExecution awaitable.
		// /// <summary>
		// /// Return Awaiter.
		// /// </summary>
		// /// <param name="cancellationToken"></param>
		// /// <returns></returns>
		// public TaskAwaiter<IList<TaskResult>> GetAwaiter(CancellationToken cancellationToken = default)
		// {
		// 	this.task = Task.Run<IList<TaskResult>>(async () =>
		// 		{
		// 			List<TaskResult> results = new();
		//
		// 			await foreach (TaskExecution execution in this)
		// 			{
		// 				results.Add(await execution.Completion());
		// 			}
		//
		// 			return results;
		// 		}
		// 		, cancellationToken
		// 	);
		//
		// 	return this.task.GetAwaiter();
		// }
	}
}