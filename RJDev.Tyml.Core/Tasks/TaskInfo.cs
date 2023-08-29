using System;

namespace RJDev.Tyml.Core.Tasks
{
	public class TaskInfo
	{
		/// <summary>
		/// Type of task
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Attribute of task
		/// </summary>
		public TymlTaskAttribute Attribute { get; }

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="type"></param>
		/// <param name="attribute"></param>
		public TaskInfo(Type type, TymlTaskAttribute attribute)
		{
			Type = type;
			Attribute = attribute;
		}
	}
}