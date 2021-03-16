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
        /// Ctor
        /// </summary>
        /// <param name="name">Case-insensitive name of the task.</param>
        public TymlTaskAttribute(string name)
        {
            this.Name = name;
        }
    }
}