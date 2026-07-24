using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using Amaryllis.States.Interfaces;
using Cysharp.Threading.Tasks;
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
        private CancellationTokenSource _stateCancellationTokenSource;
        private bool _isInitialized;
        private bool _isExecuting;
        private bool _isTransitioning;

        public int CurrentStateId => _currentState?.StateId ?? -1;

        public void Init()
        {
            InitAsync(this.GetCancellationTokenOnDestroy()).Forget(Debug.LogException);
        }

        public async UniTask InitAsync(CancellationToken cancellationToken = default)
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            _states = GetComponentsInChildren<IStateObject>().ToList();
            
            await MoveToStateAsync(_startState, cancellationToken);
            
            OnInitHandler?.Invoke();
            
            AmaryllisLog.Log("[StatesObjectBase] Init");
        }

        public async UniTask Exec(IEntity entity, bool isCheckConditions = true, CancellationToken cancellationToken = default)
        {
            if (_isExecuting || _isTransitioning)
            {
                AmaryllisLog.Log("[StatesObjectBase] Exec skipped: state object is busy");
                return;
            }

            if (_currentState == null)
            {
                AmaryllisLog.Log("[StatesObjectBase] Exec skipped: current state is empty");
                return;
            }

            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, GetStateCancellationToken(), this.GetCancellationTokenOnDestroy());
            _isExecuting = true;

            try
            {
            if (isCheckConditions)
            {
                var isReady = _currentState.IsReadyForExec(entity);

                if (!isReady)
                {
                    await ConditionFailAsync(entity, linkedCancellation.Token);
                    
                    OnConditionFailHandler?.Invoke(entity == null ? string.Empty : entity.Id);
                    return;
                }
            }

            AmaryllisLog.Log("[StatesObjectBase] entity = " + entity);
            
            await _currentState.ExecAsync(entity, linkedCancellation.Token);
            
            OnExecHandler?.Invoke(entity == null ? string.Empty : entity.Id);

            await OnExecCompleted(_currentState.NextStateId, cancellationToken);

            AmaryllisLog.Log("[StatesObjectBase] Exec");
            }
            catch (OperationCanceledException)
            {
                AmaryllisLog.Log("[StatesObjectBase] Exec canceled");
            }
            finally
            {
                _isExecuting = false;
            }
        }

        public async UniTask MoveToStateByIdAsync(int stateId, CancellationToken cancellationToken = default)
        {
            await MoveToStateAsync(stateId, cancellationToken);
        }

        public async UniTask ConditionFailAsync(IEntity entity, CancellationToken cancellationToken = default)
        {
            if (_currentState == null)
            {
                return;
            }

            await _currentState.RunConditionFailActions(entity, cancellationToken);
        }
        
        private async UniTask OnExecCompleted(int stateId, CancellationToken cancellationToken)
        {
            await MoveToStateAsync(stateId, cancellationToken);
        }

        private async UniTask MoveToStateAsync(int stateId, CancellationToken cancellationToken)
        {
            if (stateId == -1) return;
            if (_isTransitioning)
            {
                AmaryllisLog.Log($"[StatesObjectBase] MoveToState skipped: transition already running ({stateId})");
                return;
            }

            AmaryllisLog.Log($@"[StatesObjectBase] MoveToState {stateId}");

            _isTransitioning = true;
            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.GetCancellationTokenOnDestroy());
            var oldStateCancellation = _stateCancellationTokenSource;
            _stateCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(linkedCancellation.Token);
            oldStateCancellation?.Cancel();

            try
            {
                await DiscardOldStateAsync(linkedCancellation.Token);
                oldStateCancellation?.Dispose();
                
                await InitNewStateAsync(stateId, _stateCancellationTokenSource.Token);
                
                OnStateChangedHandler?.Invoke(stateId);
                
                AmaryllisLog.Log($@"[StatesObjectBase] MoveToState {stateId} End");
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        private async UniTask InitNewStateAsync(int stateId, CancellationToken cancellationToken)
        {
            _currentState = _states.Find(s => s.StateId == stateId);

            if (_currentState == null) return;

            await _currentState.PreInitAsync(cancellationToken);
            await _currentState.InitAsync(cancellationToken);
        }
        
        private async UniTask DiscardOldStateAsync(CancellationToken cancellationToken)
        {
            if (_currentState == null) return;

            await _currentState.DiscardAsync(cancellationToken);
            
            await _currentState.PostDiscardAsync(cancellationToken);
        }

        private CancellationToken GetStateCancellationToken()
        {
            return _stateCancellationTokenSource?.Token ?? CancellationToken.None;
        }
    }
}
