using System;
using System.Threading;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Persistence;
using Cysharp.Threading.Tasks;

namespace Amaryllis.States.Interfaces
{
    public interface IStatesObject : IExecObject
    {
        event Action OnInitHandler;
        event Action<string> OnExecHandler;
        event Action<string> OnConditionFailHandler;
        event Action<int> OnStateChangedHandler;
        
        string SaveId { get; }
        int CurrentStateId { get; }
        
        void Init();
        UniTask InitAsync(CancellationToken cancellationToken = default);
        UniTask MoveToStateByIdAsync(int stateId, CancellationToken cancellationToken = default);
        UniTask ConditionFailAsync(IEntity entity, CancellationToken cancellationToken = default);
        StatesObjectSnapshot CaptureSnapshot();
        UniTask RestoreSnapshotAsync(StatesObjectSnapshot snapshot, CancellationToken cancellationToken = default);
    }
}
