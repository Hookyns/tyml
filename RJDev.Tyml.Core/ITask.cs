using System.Collections;
using System.Threading.Tasks;

namespace RJDev.Tyml.Core
{
    public interface ITask
    {
        /// <summary>
        /// Execute task
        /// </summary>
        /// <param name="context"></param>
        /// <param name="inputs"></param>
        /// <returns></returns>
        Task Execute(TaskContext context, IDictionary inputs);
    }
}