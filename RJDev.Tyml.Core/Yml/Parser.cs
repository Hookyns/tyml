using System;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace RJDev.Tyml.Core.Yml
{
    public class Parser
    {
        /// <summary>
        /// Instance of deserializer
        /// </summary>
        private readonly IDeserializer deserializer;

        public Parser()
        {
            this.deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
        }

        /// <summary>
        /// Parse YAML string and return RootConfiguration.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="tymlContext"></param>
        /// <returns></returns>
        public RootConfiguration Parse(string config, TymlContext tymlContext)
        {
            config = this.ProcessVariables(config, tymlContext);
            return this.deserializer.Deserialize<RootConfiguration>(config);
        }

        /// <summary>
        /// Replace variables by values
        /// </summary>
        /// <param name="config"></param>
        /// <param name="tymlContext"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private string ProcessVariables(string config, TymlContext tymlContext)
        {
            VariablesConfiguration variablesConfiguration = this.deserializer.Deserialize<VariablesConfiguration>(config);
            
            return Regex.Replace(
                config,
                "\\$\\(([a-zA-Z0-9-_.]+)\\)(!?)",
                match => ResolveVariable(tymlContext, match, variablesConfiguration)
            );
        }

        /// <summary>
        /// Return resolved variable value
        /// </summary>
        /// <param name="tymlContext"></param>
        /// <param name="match"></param>
        /// <param name="variablesConfiguration"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static string ResolveVariable(TymlContext tymlContext, Match match, VariablesConfiguration variablesConfiguration)
        {
            string variableName = match.Groups[1].Value;
            bool required = match.Groups[2].Value == "!";

            // Try get value from parsed YAML variables
            if (variablesConfiguration.Variables.TryGetValue(variableName, out object? variable))
            {
                return variable.ToString() ?? string.Empty;
            }

            // Get value from context
            variable = tymlContext.GetVariable(variableName);

            if (variable == null)
            {
                // If variable is required
                if (required)
                {
                    throw new InvalidOperationException($"Variable '{variableName}' required but not defined.");
                }

                return match.Value;
            }

            return variable.ToString() ?? string.Empty;
        }
    }
}