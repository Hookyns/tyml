using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RJDev.Tyml.Core;
using RJDev.Tyml.Core.Tasks;

namespace RJDev.Tyml.Tasks.Basic.Cmd
{
    [TymlTask("Cmd")]
    public class CmdTask : TaskBase<CmdInputs>
    {
        protected override Task Execute(TaskContext context, CmdInputs inputs)
        {
            Process cmd = ExecutePlatformCmd(inputs);
            
            return cmd.WaitForExitAsync()
                .ContinueWith(_ => context.Output.WriteLineAsync(cmd.StandardOutput.ReadToEnd()))
                .Unwrap();
        }

        /// <summary>
        /// Execute process depending on current OS
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        private static Process ExecutePlatformCmd(CmdInputs inputs)
        {
            Process cmd = GetBaseProcess();

            if (OperatingSystem.IsWindows())
            {
                ExecuteCmd(inputs, cmd);
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
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
        /// <returns></returns>
        private static Process GetBaseProcess()
        {
            Process cmd = new();
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            return cmd;
        }
    }
}