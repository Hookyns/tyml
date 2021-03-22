using System.Collections.Generic;

namespace RJDev.Tyml.Executable
{
	public class TestTaskConfig
	{
		public string Script { get; set; } = string.Empty;

		public IDictionary<string, object> Args { get; set; } = new Dictionary<string, object>(0);
	}
}