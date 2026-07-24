using System;
using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
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
       
        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            var character = entity as ICharacterActionTarget;
            if (character == null)
            {
                return false;
            }
            
            character.SetAnimationTrigger(SetTriggerName, LockTime);

            if (IsWaitEndLockTime)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(LockTime), cancellationToken: cancellationToken);
            }
            
            return true;
        }
    }
}
