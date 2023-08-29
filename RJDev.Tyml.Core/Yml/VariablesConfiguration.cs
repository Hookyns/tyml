using System.Collections.Generic;

namespace RJDev.Tyml.Core.Yml
{
	public class VariablesConfiguration
	{
		private Dictionary<string, object> variables = new(0);

		/// <summary>
		/// List of variables
		/// </summary>
		public Dictionary<string, object> Variables
		{
			get => variables;
			set => variables = (Dictionary<string, object>?)value ?? new Dictionary<string, object>(0);
		}
	}
}