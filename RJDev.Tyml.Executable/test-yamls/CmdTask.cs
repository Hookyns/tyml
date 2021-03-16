using System.Linq;
using System.Threading.Tasks;
using RJDev.Tyml.Core;

namespace RJDev.Tyml.Executable
{
    [TymlTask("cmd")]
    public class CmdTask : TaskBase<CmdTaskConfig>
    {
        protected override Task Execute(TaskContext context, CmdTaskConfig inputs)
        {
            context.Output.WriteLine($"Script: {inputs.Script} with args: {string.Join("; ", inputs.Args.Select(entry => entry.Key + ":" + entry.Value))}");
            return Task.CompletedTask;
        }
    }
}