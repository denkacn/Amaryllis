using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using Amaryllis.Persistence;
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
        
        [SerializeField] private string _saveId;
        [SerializeField] private int _startState;

        private List<IStateObject> _states = new List<IStateObject>();
        private Dictionary<int, IStateObject> _statesById = new Dictionary<int, IStateObject>();
        
        private IStateObject _currentState;
        private CancellationTokenSource _stateCancellationTokenSource;
        private bool _isInitialized;
        private bool _isExecuting;
        private bool _isTransitioning;

        public string SaveId => GetSaveId();
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
            BuildStateCache();
            ValidateStateGraph(true);
            
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
                var executedState = _currentState;
                
                if (isCheckConditions)
                {
                    var isReady = executedState.IsReadyForExec(entity);

                    if (!isReady)
                    {
                        await ConditionFailAsync(entity, linkedCancellation.Token);
                    
                        OnConditionFailHandler?.Invoke(entity == null ? string.Empty : entity.Id);
                        return;
                    }
                }

                AmaryllisLog.Log("[StatesObjectBase] entity = " + entity);
            
                await executedState.ExecAsync(entity, linkedCancellation.Token);
            
                OnExecHandler?.Invoke(entity == null ? string.Empty : entity.Id);

                if (ReferenceEquals(_currentState, executedState))
                {
                    await OnExecCompleted(executedState.NextStateId, cancellationToken);
                }

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

        public StatesObjectSnapshot CaptureSnapshot()
        {
            return new StatesObjectSnapshot
            {
                SaveId = SaveId,
                StateId = CurrentStateId
            };
        }

        public async UniTask RestoreSnapshotAsync(StatesObjectSnapshot snapshot, CancellationToken cancellationToken = default)
        {
            if (snapshot == null)
            {
                return;
            }

            await InitAsync(cancellationToken);
            await MoveToStateByIdAsync(snapshot.StateId, cancellationToken);
        }
        
        private async UniTask OnExecCompleted(int stateId, CancellationToken cancellationToken)
        {
            await MoveToStateAsync(stateId, cancellationToken);
        }

        private async UniTask MoveToStateAsync(int stateId, CancellationToken cancellationToken)
        {
            if (stateId == -1) return;
            
            if (!_statesById.ContainsKey(stateId))
            {
                Debug.LogError($"[Amaryllis] [StatesObjectBase] State id {stateId} not found in {name}", this);
                return;
            }
            
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
            _statesById.TryGetValue(stateId, out _currentState);

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

        private void BuildStateCache()
        {
            _states = GetComponentsInChildren<IStateObject>(true).Where(state => state != null).ToList();
            _statesById = new Dictionary<int, IStateObject>();

            foreach (var state in _states)
            {
                if (_statesById.ContainsKey(state.StateId))
                {
                    continue;
                }

                _statesById.Add(state.StateId, state);
            }
        }

        private bool ValidateStateGraph(bool logErrors)
        {
            var isValid = true;

            if (_states.Count == 0)
            {
                LogValidationError("State graph is empty", logErrors);
                return false;
            }

            var duplicateStateIds = _states
                .GroupBy(state => state.StateId)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key);

            foreach (var duplicateStateId in duplicateStateIds)
            {
                isValid = false;
                LogValidationError($"Duplicate state id {duplicateStateId}", logErrors);
            }

            if (!_statesById.ContainsKey(_startState))
            {
                isValid = false;
                LogValidationError($"Start state id {_startState} not found", logErrors);
            }

            foreach (var state in _states)
            {
                if (state.NextStateId == -1)
                {
                    continue;
                }

                if (!_statesById.ContainsKey(state.NextStateId))
                {
                    isValid = false;
                    LogValidationError($"State {state.StateId} points to missing next state {state.NextStateId}", logErrors);
                }
            }

            return isValid;
        }

        private void LogValidationError(string message, bool logErrors)
        {
            if (logErrors)
            {
                Debug.LogError($"[Amaryllis] [StatesObjectBase] {message} in {name}", this);
            }
        }

        private void OnValidate()
        {
            BuildStateCache();
            ValidateStateGraph(true);
        }

        private string GetSaveId()
        {
            if (!string.IsNullOrWhiteSpace(_saveId))
            {
                return _saveId;
            }

            var entity = GetComponentInParent<IEntity>();
            if (entity != null && !string.IsNullOrWhiteSpace(entity.Id))
            {
                return entity.Id;
            }

            return GetTransformPath(transform);
        }

        private static string GetTransformPath(Transform target)
        {
            var names = new Stack<string>();
            var current = target;

            while (current != null)
            {
                names.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", names);
        }
    }
}
