using System.Collections.Generic;

namespace RJDev.Tyml.Executable
{
    public class CmdTaskConfig
    {
        public string Script { get; set; }

        public IDictionary<string, object> Args { get; set; }
    }
}