using System.Diagnostics;
using System.Threading.Tasks;
using RJDev.Tyml.Core;

namespace RJDev.Tyml.Tasks.Basic.Cmd
{
    [TymlTask("Cmd")]
    public class CmdTask : TaskBase<CmdInputs>
    {
        protected override Task Execute(TaskContext context, CmdInputs inputs)
        {
            Process cmd = new();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(inputs.Script);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();

            return cmd.WaitForExitAsync()
                .ContinueWith(_ => context.Output.WriteLineAsync(cmd.StandardOutput.ReadToEnd()))
                .Unwrap();
        }
    }
}