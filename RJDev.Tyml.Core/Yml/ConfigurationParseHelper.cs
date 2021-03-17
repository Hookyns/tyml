using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace RJDev.Tyml.Core.Yml
{
	public class ConfigurationParseHelper
	{
		/// <summary>
		/// Convert dictionary into given type
		/// </summary>
		/// <param name="dictionary"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		/// <exception cref="NullReferenceException"></exception>
		public static object GetObject(IDictionary dictionary, Type targetType)
		{
			object instance = Activator.CreateInstance(targetType)
				?? throw new NullReferenceException($"Type '${targetType.FullName}' has no parameterless public constructor.");

			foreach (DictionaryEntry entry in dictionary)
			{
				PropertyInfo? property = targetType.GetProperty(entry.Key.ToString()!);

				// Skip unknown keys
				if (property == null)
				{
					continue;
				}

				object? value = entry.Value;
				// Type valueType = value.GetType();

				// If value is dictionary
				// if (valueType.IsGenericType && typeof(IDictionary<,>).IsAssignableFrom(valueType.GetGenericTypeDefinition()))
				if (value is Dictionary<object, object> nestedObjectDictionary)
				{
					// When target property is dictionary too but it is not <object, object>
					// if (property.PropertyType.IsGenericType && typeof(IDictionary<,>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()))
					if (!typeof(IDictionary<object, object>).IsAssignableFrom(property.PropertyType))
					{
						value = GetDictionaryOfType(property, nestedObjectDictionary);
					}
					else
					{
						value = GetObject(nestedObjectDictionary, property.PropertyType);
					}

					// Assign
					property.SetValue(instance, value, null);
				}

				// Non generic dictionary; never reachable with YAML parse output
				// else if (typeof(IDictionary).IsAssignableFrom(valueType))
				// {
				//     
				// }

				// Convert
				else
				{
					property.SetValue(instance, TypeDescriptor.GetConverter(property.PropertyType).ConvertFrom(value), null);
				}
			}

			return instance;
		}

		/// <summary>
		/// Get dictionary of corresponding type.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="nestedObjectDictionary"></param>
		/// <returns></returns>
		private static object GetDictionaryOfType(PropertyInfo property, Dictionary<object, object> nestedObjectDictionary)
		{
			// Converters
			Type[] genericArgs = property.PropertyType.GetGenericArguments();
			TypeConverter valueConverter = TypeDescriptor.GetConverter(genericArgs[1]);

			// If value is string (&& object of dictionary is string; which is always in case of YAML parse output)
			if (genericArgs[1] == typeof(string))
			{
				// return Dictionary<string, string?>
				return nestedObjectDictionary.ToDictionary(
					x => (string) x.Key,
					x => (string?) x.Value
				);
			}

			// If value is object
			if (genericArgs[1] == typeof(object))
			{
				// return Dictionary<string, object>
				return nestedObjectDictionary.ToDictionary(
					x => (string) x.Key,
					x => x.Value
				);
			}

			// return Dictionary<string, T>
			return nestedObjectDictionary.ToDictionary(
				x => (string) x.Key,
				x => valueConverter.ConvertFrom(x.Value)
			);
		}
	}
}