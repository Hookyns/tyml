using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RJDev.Outputter;
using RJDev.Tyml.Core;
using RJDev.Tyml.Core.Tasks;

namespace RJDev.Tyml.Tasks.Basic.Cmd
{
	[TymlTask("Cmd")]
	public class CmdTask : TaskBase<CmdInputs>
	{
		private class State
		{
			public bool error;
		}

		protected override Task<TaskCompletionStatus> Execute(TaskContext context, CmdInputs inputs, CancellationToken cancellationToken)
		{
			State state = new();
			Process cmd = ExecutePlatformCmd(context, inputs, state);

			cancellationToken.Register(() =>
			{
				cmd.Kill();
			});
			
			cmd.BeginOutputReadLine();
			cmd.BeginErrorReadLine();
			
			cmd.WaitForExit();

			if (cmd.ExitCode != 0 || state.error)
			{
				return ErrorSync();
			}

			return OkSync();
		}

		/// <summary>
		/// Execute process depending on current OS
		/// </summary>
		/// <param name="context"></param>
		/// <param name="inputs"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		private static Process ExecutePlatformCmd(TaskContext context, CmdInputs inputs, State state)
		{
			Process cmd = GetBaseProcess(context);

			cmd.ErrorDataReceived += (_, args) =>
			{
				context.Out.WriteLine(args.Data, EntryType.Error);

				if (inputs.FailOnStdError)
				{
					state.error = true;
					
					try
					{
						cmd.Kill();
					}
					catch (NotSupportedException)
					{
					}
					catch (InvalidOperationException)
					{
					}
				}
			};

			cmd.OutputDataReceived += (_, args) => { context.Out.WriteLine(args.Data); };

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

			// Try to set right encoding. If it is not available, ignore.
			// System.Text.Encoding.CodePages package may be required.
			// System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
			try
			{
				Encoding encoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);

				cmd.StartInfo.StandardOutputEncoding = encoding;
				cmd.StartInfo.StandardErrorEncoding = encoding;
			}
			catch (ArgumentException)
			{
			}
			catch (NotSupportedException)
			{
			}

			return cmd;
		}
	}
}