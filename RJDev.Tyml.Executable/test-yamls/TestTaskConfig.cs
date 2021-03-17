using System.Collections.Generic;

namespace RJDev.Tyml.Executable
{
	public class TestTaskConfig
	{
		public string Script { get; set; }

		public IDictionary<string, object> Args { get; set; }
	}
}