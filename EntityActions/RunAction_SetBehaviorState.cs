using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_SetBehaviorState : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField] 
        private GameObject _behaviorObject;
        [SerializeField] 
        private string _behaviorStateId;
        [SerializeField] 
        private bool _isFast;
        
        protected override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            AmaryllisLog.Log("!!! [RunAction_EnterCommandDialogueInUi] Empty !!!");
            
            return UniTask.FromResult(true);
        }
    }
}
