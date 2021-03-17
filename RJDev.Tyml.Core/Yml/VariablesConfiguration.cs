using System.Collections.Generic;

namespace RJDev.Tyml.Core.Yml
{
	public class VariablesConfiguration
	{
		/// <summary>
		/// List of variables
		/// </summary>
		public Dictionary<string, object> Variables { get; set; } = new(0);
	}
}