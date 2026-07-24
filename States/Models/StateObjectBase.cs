using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amaryllis.Actions.Helpers;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using Amaryllis.States.Interfaces;
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

        public async Task PreInitAsync()
        {
            AmaryllisLog.Log($@"[StateObjectBase] PreInit {_stateId}");

             _actions = GetComponentsInChildren<IRunAction>().ToList();
             _conditions = GetComponentsInChildren<IStateCondition>().ToList();
             
             await RunActionLogicHelper.RunActionsAsync(ExecTimeType.PreInit, null, _actions);
        }

        public async Task InitAsync()
        {
            AmaryllisLog.Log($@"[StateObjectBase] Init {_stateId}");

            await RunActionLogicHelper.RunActionsAsync(ExecTimeType.Init, null, _actions);
        }

        public async Task<bool> ExecAsync(IEntity entity)
        {
            AmaryllisLog.Log($@"[StateObjectBase] Exec {_stateId}");

            var result = await RunActionLogicHelper.RunActionsAsync(ExecTimeType.Exec, entity, _actions);

            return result;
        }

        public async Task DiscardAsync()
        {
            AmaryllisLog.Log($@"[StateObjectBase] Discard {_stateId}");
            
            await RunActionLogicHelper.RunActionsAsync(ExecTimeType.Discard, null, _actions);
        }
        
        public void PostDiscard()
        {
            AmaryllisLog.Log($@"[StateObjectBase] PostDiscard {_stateId}");
            AmaryllisLog.Log("------------------------------------------");
            
            RunActionLogicHelper.RunActionsAsync(ExecTimeType.PostDiscard, null, _actions);
        }

        public bool IsReadyForExec(IEntity entity)
        {
            foreach (var condition in _conditions)
            {
                if (!condition.IsCanExec(entity))
                {
                    return false;
                }
            }
            
            return true;
        }

        public async Task RunConditionFailActions(IEntity entity)
        {
            await RunActionLogicHelper.RunActionsAsync(ExecTimeType.ConditionFail, entity, _actions);
        }
    }
}