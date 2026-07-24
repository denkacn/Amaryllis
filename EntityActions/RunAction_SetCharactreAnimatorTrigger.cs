using System;
using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Entities.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_SetCharactreAnimatorTrigger : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField] 
        private string SetTriggerName;
        [SerializeField] 
        private float LockTime;
        [SerializeField] 
        private bool IsWaitEndLockTime = false;
       
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            var character = entity as CharacterBaseEntity;
            character.SetAnimationTrigger(SetTriggerName, LockTime);

            if (IsWaitEndLockTime)
            {
                await Task.Delay(TimeSpan.FromSeconds(LockTime));
            }
            
            await Task.Yield();
            return true;
        }
    }
}
