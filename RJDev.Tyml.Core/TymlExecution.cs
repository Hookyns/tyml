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
		/// Tyml executor.
		/// </summary>
		private readonly TymlExecutor tymlExecutor;

		/// <summary>
		/// Tyml context
		/// </summary>
		private readonly TymlContext context;

		/// <summary>
		/// YAML root configuration.
		/// </summary>
		private readonly RootConfiguration rootConfiguration;

		/// <summary>
		/// Cancellation token
		/// </summary>
		private readonly CancellationToken cancellationToken;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="tymlExecutor"></param>
		/// <param name="context"></param>
		/// <param name="rootConfiguration"></param>
		/// <param name="cancellationToken"></param>
		public TymlExecution(TymlExecutor tymlExecutor, TymlContext context, RootConfiguration rootConfiguration, CancellationToken cancellationToken)
		{
			this.tymlExecutor = tymlExecutor;
			this.context = context;
			this.rootConfiguration = rootConfiguration;
			this.cancellationToken = cancellationToken;
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

		/// <summary>
		/// Return async enumerable.
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public async IAsyncEnumerator<TaskExecution> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			// List of outputs
			foreach (TaskConfiguration step in this.rootConfiguration.Steps)
			{
				using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(this.cancellationToken, cancellationToken))
				{
					TaskExecution execution = this.tymlExecutor.ExecuteTask(this.context, step, this.rootConfiguration, cts.Token);
					yield return execution;
					await execution.Completion();
				}
			}
		}
	}
}