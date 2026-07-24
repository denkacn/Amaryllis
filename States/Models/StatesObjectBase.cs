using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using Amaryllis.States.Interfaces;
using UnityEngine;

namespace Amaryllis.States.Models
{
    public class StatesObjectBase : MonoBehaviour, IStatesObject
    {
        public event Action OnInitHandler;
        public event Action<string> OnExecHandler;
        public event Action<string> OnConditionFailHandler;
        public event Action<int> OnStateChangedHandler;
        
        [SerializeField] private int _startState;

        private List<IStateObject> _states = new List<IStateObject>();
        
        private IStateObject _currentState;

        public int CurrentStateId => _currentState.StateId;

        public void Init()
        {
            _states = GetComponentsInChildren<IStateObject>().ToList();
            
            MoveToStateAsync(_startState);
            
            OnInitHandler?.Invoke();
            
            AmaryllisLog.Log("[StatesObjectBase] Init");
        }

        public async Task Exec(IEntity entity, bool isCheckConditions = true)
        {
            if (isCheckConditions)
            {
                var isReady = _currentState.IsReadyForExec(entity);

                if (!isReady)
                {
                    await ConditionFailAsync(entity);
                    
                    OnConditionFailHandler?.Invoke(entity == null ? string.Empty : entity.Id);
                    return;
                }
            }

            AmaryllisLog.Log("[StatesObjectBase] entity = " + entity);
            
            await _currentState.ExecAsync(entity);
            
            OnExecHandler?.Invoke(entity == null ? string.Empty : entity.Id);

            OnExecCompleted(_currentState.NextStateId);

            AmaryllisLog.Log("[StatesObjectBase] Exec");
        }

        public async Task MoveToStateByIdAsync(int stateId)
        {
            await MoveToStateAsync(stateId);
        }

        public async Task ConditionFailAsync(IEntity entity)
        {
            await _currentState.RunConditionFailActions(entity);
        }
        
        private void OnExecCompleted(int stateId)
        {
            MoveToStateAsync(stateId);
        }

        private async Task MoveToStateAsync(int stateId)
        {
            if (stateId == -1) return;

            AmaryllisLog.Log($@"[StatesObjectBase] MoveToState {stateId}");

            await DiscardOldStateAsync();
            await InitNewStateAsync(stateId);
            
            OnStateChangedHandler?.Invoke(stateId);
            
            AmaryllisLog.Log($@"[StatesObjectBase] MoveToState {stateId} End");
        }

        private async Task InitNewStateAsync(int stateId)
        {
            _currentState = _states.Find(s => s.StateId == stateId);

            if (_currentState == null) return;

            await _currentState.PreInitAsync();
            await _currentState.InitAsync();
        }
        
        private async Task DiscardOldStateAsync( )
        {
            if (_currentState == null) return;

            await _currentState.DiscardAsync();
            
            _currentState.PostDiscard();
        }
    }
}