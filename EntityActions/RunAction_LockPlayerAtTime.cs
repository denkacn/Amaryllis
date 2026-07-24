using System;
using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_LockPlayerAtTime : BaseRunAction
    {
        [ShowInInspector, PropertySpace] 
        
        [SerializeField] 
        private float _lockTime;

        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            var character = entity as ICharacterActionTarget;
            
            if (character == null) return true;
            
            character.SetEnableControl(false);
            await UniTask.Delay(TimeSpan.FromSeconds(_lockTime), cancellationToken: cancellationToken);
            character.SetEnableControl(true);
            
            return true;
        }
    }
}
