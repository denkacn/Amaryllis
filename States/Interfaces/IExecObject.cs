using System.Threading;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;

namespace Amaryllis.States.Interfaces
{
    public interface IExecObject
    {
        UniTask Exec(IEntity entity, bool isCheckConditions = true, CancellationToken cancellationToken = default);
    }
}
