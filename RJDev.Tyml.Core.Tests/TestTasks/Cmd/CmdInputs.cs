using System.Collections.Generic;

namespace RJDev.Tyml.Core.Tests.TestTasks.Cmd
{
	public class CmdInputs
	{
		/// <summary>
		/// Script to execute
		/// </summary>
		public string Script { get; set; } = string.Empty;

		/// <summary>
		/// Args
		/// </summary>
		public IDictionary<string, object> Args { get; set; }
	}
}