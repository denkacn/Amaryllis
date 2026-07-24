using System;
using System.Threading;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;

namespace Amaryllis.States.Interfaces
{
    public interface IStatesObject : IExecObject
    {
        event Action OnInitHandler;
        event Action<string> OnExecHandler;
        event Action<string> OnConditionFailHandler;
        event Action<int> OnStateChangedHandler;
        
        int CurrentStateId { get; }
        
        void Init();
        UniTask InitAsync(CancellationToken cancellationToken = default);
        UniTask MoveToStateByIdAsync(int stateId, CancellationToken cancellationToken = default);
        UniTask ConditionFailAsync(IEntity entity, CancellationToken cancellationToken = default);
    }
}
