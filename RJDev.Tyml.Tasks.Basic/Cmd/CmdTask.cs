using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Tyml.Core;
using RJDev.Tyml.Core.Tasks;

namespace RJDev.Tyml.Tasks.Basic.Cmd
{
	[TymlTask("Cmd")]
	public class CmdTask : TaskBase<CmdInputs>
	{
		protected override Task<TaskCompletionStatus> Execute(TaskContext context, CmdInputs inputs, CancellationToken _)
		{
			Process cmd = ExecutePlatformCmd(context, inputs);
			cmd.WaitForExit();
			context.Out.WriteLine(cmd.StandardOutput.ReadToEnd());
			return this.OkSync();
		}

		/// <summary>
		/// Execute process depending on current OS
		/// </summary>
		/// <param name="context"></param>
		/// <param name="inputs"></param>
		/// <returns></returns>
		private static Process ExecutePlatformCmd(TaskContext context, CmdInputs inputs)
		{
			Process cmd = GetBaseProcess(context);

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				ExecuteCmd(inputs, cmd);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				ExecuteBash(inputs, cmd);
			}

			return cmd;
		}

		/// <summary>
		/// Execute script on Windows Cmd
		/// </summary>
		/// <param name="inputs"></param>
		/// <param name="cmd"></param>
		private static void ExecuteCmd(CmdInputs inputs, Process cmd)
		{
			cmd.StartInfo.FileName = "cmd.exe";
			cmd.Start();

			cmd.StandardInput.WriteLine(inputs.Script);
			cmd.StandardInput.Flush();
			cmd.StandardInput.Close();
		}

		/// <summary>
		/// Execute script in unix Bash
		/// </summary>
		/// <param name="inputs"></param>
		/// <param name="cmd"></param>
		private static void ExecuteBash(CmdInputs inputs, Process cmd)
		{
			string escapedScript = inputs.Script.Replace("\"", "\\\"");

			cmd.StartInfo.FileName = "bash";
			cmd.StartInfo.Arguments = $"-c \"{escapedScript}\"";

			cmd.Start();
		}

		/// <summary>
		/// Return base configured Process instance
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		private static Process GetBaseProcess(TaskContext context)
		{
			Process cmd = new();
			cmd.StartInfo.WorkingDirectory = context.TymlContext.WorkingDirectory;
			cmd.StartInfo.CreateNoWindow = true;
			cmd.StartInfo.UseShellExecute = false;
			cmd.StartInfo.RedirectStandardInput = true;
			cmd.StartInfo.RedirectStandardOutput = true;
			cmd.StartInfo.RedirectStandardError = true;
			return cmd;
		}
	}
}