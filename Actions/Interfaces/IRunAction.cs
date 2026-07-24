using System.Threading;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Actions.Models;
using Amaryllis.States.Models;
using Cysharp.Threading.Tasks;

namespace Amaryllis.Actions.Interfaces
{
    public interface IRunAction
    {
        int ExecPriority { get; }
        ExecTimeType ExecTime { get; }
        UniTask<RunActionResult> Run(IEntity entity, CancellationToken cancellationToken = default);
    }
}
