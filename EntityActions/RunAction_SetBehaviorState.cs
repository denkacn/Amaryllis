using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
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
        
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            AmaryllisLog.Log("!!! [RunAction_EnterCommandDialogueInUi] Empty !!!");
            
            await Task.Yield();
            return true;
        }
    }
}
