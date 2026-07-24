using System.Threading;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;

namespace Amaryllis.States.Interfaces
{
    public interface IStateObject
    {
        public int StateId { get; }
        public int NextStateId { get; }
        UniTask PreInitAsync(CancellationToken cancellationToken = default);
        UniTask InitAsync(CancellationToken cancellationToken = default);
        UniTask<bool> ExecAsync(IEntity entity, CancellationToken cancellationToken = default);
        UniTask DiscardAsync(CancellationToken cancellationToken = default);
        UniTask PostDiscardAsync(CancellationToken cancellationToken = default);
        bool IsReadyForExec(IEntity entity);
        UniTask RunConditionFailActions(IEntity entity, CancellationToken cancellationToken = default);
    }
}
