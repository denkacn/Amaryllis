using System.Threading.Tasks;
using Amaryllis.Entities.Interfaces;
using Amaryllis.States.Models;

namespace Amaryllis.Actions.Interfaces
{
    public interface IRunAction
    {
        int ExecPriority { get; }
        ExecTimeType ExecTime { get; }
        Task<bool> Run(IEntity entity);
    }
}