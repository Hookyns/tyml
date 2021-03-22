using System;
using System.Threading;
using RJDev.Tyml.Core.Yml;

namespace RJDev.Tyml.Core
{
	public sealed class TymlExecutor
	{
		/// <summary>
		/// Instance of service provider.
		/// </summary>
		private readonly IServiceProvider serviceProvider;

		/// <summary>
		/// YAML config parser.
		/// </summary>
		private readonly Parser parser;

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="serviceProvider"></param>
		public TymlExecutor(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
			this.parser = new Parser();
		}

		/// <summary>
		/// Run processing of YAML configuration over given context.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="ymlContent">YAML configuration file content.</param>
		/// <param name="cancellationToken"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public TymlExecution Execute(TymlContext context, string ymlContent, CancellationToken cancellationToken = default)
		{
			RootConfiguration config = this.parser.Parse(ymlContent, context);
			return new TymlExecution(this.serviceProvider, context, config, cancellationToken);
		}
	}
}