using System;

namespace RJDev.Tyml.Core
{
	[AttributeUsage(AttributeTargets.Class)]
	public class TymlTaskAttribute : Attribute
	{
		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Task description
		/// </summary>
		public string? Description { get; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="name">Case-insensitive name of the task.</param>
		/// <param name="description"></param>
		public TymlTaskAttribute(string name, string? description = null)
		{
			Name = name;
			Description = description;
		}
	}
}