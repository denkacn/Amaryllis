using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amaryllis.Actions.Helpers;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using Amaryllis.States.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Amaryllis.States.Models
{
    public class StateObjectBase : MonoBehaviour, IStateObject
    {
        public int StateId => _stateId;
        public int NextStateId => _nextStateId;
        
        [SerializeField] private int _stateId;
        [SerializeField] private int _nextStateId;

        private List<IRunAction> _actions;
        private List<IStateCondition> _conditions;

        public async UniTask PreInitAsync(CancellationToken cancellationToken = default)
        {
            AmaryllisLog.Log($@"[StateObjectBase] PreInit {_stateId}");

             _actions = CompositeRunActionUtility.GetRootActions(GetComponentsInChildren<IRunAction>(true));
             _conditions = GetComponentsInChildren<IStateCondition>(true).Where(condition => condition != null).ToList();
             
             await RunActionLogicHelper.RunActionsAsync(ExecTimeType.PreInit, null, _actions, cancellationToken);
        }

        public async UniTask InitAsync(CancellationToken cancellationToken = default)
        {
            AmaryllisLog.Log($@"[StateObjectBase] Init {_stateId}");

            await RunActionLogicHelper.RunActionsAsync(ExecTimeType.Init, null, _actions, cancellationToken);
        }

        public async UniTask<bool> ExecAsync(IEntity entity, CancellationToken cancellationToken = default)
        {
            AmaryllisLog.Log($@"[StateObjectBase] Exec {_stateId}");

            var result = await RunActionLogicHelper.RunActionsAsync(ExecTimeType.Exec, entity, _actions, cancellationToken);

            return result;
        }

        public async UniTask DiscardAsync(CancellationToken cancellationToken = default)
        {
            AmaryllisLog.Log($@"[StateObjectBase] Discard {_stateId}");
            
            await RunActionLogicHelper.RunActionsAsync(ExecTimeType.Discard, null, _actions, cancellationToken);
        }
        
        public async UniTask PostDiscardAsync(CancellationToken cancellationToken = default)
        {
            AmaryllisLog.Log($@"[StateObjectBase] PostDiscard {_stateId}");
            AmaryllisLog.Log("------------------------------------------");
            
            await RunActionLogicHelper.RunActionsAsync(ExecTimeType.PostDiscard, null, _actions, cancellationToken);
        }

        public bool IsReadyForExec(IEntity entity)
        {
            if (_conditions == null)
            {
                _conditions = GetComponentsInChildren<IStateCondition>(true).Where(condition => condition != null).ToList();
            }
            
            foreach (var condition in _conditions)
            {
                if (!condition.IsCanExec(entity))
                {
                    return false;
                }
            }
            
            return true;
        }

        public async UniTask RunConditionFailActions(IEntity entity, CancellationToken cancellationToken = default)
        {
            await RunActionLogicHelper.RunActionsAsync(ExecTimeType.ConditionFail, entity, _actions, cancellationToken);
        }
    }
}
