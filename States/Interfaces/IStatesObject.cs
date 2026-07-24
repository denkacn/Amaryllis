using System;
using System.Threading.Tasks;
using Amaryllis.Entities.Interfaces;

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
        Task Exec(IEntity entity, bool isCheckConditions = true);
        Task MoveToStateByIdAsync(int stateId);
        Task ConditionFailAsync(IEntity entity);
    }
}