namespace RJDev.Tyml.Core.Tasks
{
    public class TaskOutput
    {
        /// <summary>
        /// Display name of task
        /// </summary>
        public string DisplayName { get; }
        
        /// <summary>
        /// Name identifier of task
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Output of task
        /// </summary>
        public string Output { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <param name="output"></param>
        public TaskOutput(string name, string displayName, string output)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.Output = output;
        }
    }
}