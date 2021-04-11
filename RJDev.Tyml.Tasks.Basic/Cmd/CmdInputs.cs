namespace RJDev.Tyml.Tasks.Basic.Cmd
{
	public class CmdInputs
	{
		/// <summary>
		/// Script to execute
		/// </summary>
		public string Script { get; set; } = string.Empty;
		
		/// <summary>
		/// Exit Cmd task if there were something written into std error stream.
		/// </summary>
		public bool FailOnStdError { get; set; }
	}
}