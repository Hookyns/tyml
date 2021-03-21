using System.Threading.Tasks;

namespace RJDev.Tyml.Core.Tasks
{
	internal class CompletedTaskResults
	{
		internal static readonly Task<TaskCompletionStatus> CompletedOk = Task.FromResult(TaskCompletionStatus.Ok);
		internal static readonly Task<TaskCompletionStatus> CompletedError = Task.FromResult(TaskCompletionStatus.Error);
	}
}