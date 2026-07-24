using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_LockPlayer : BaseRunAction
    {
        [SerializeField] private bool _isLock = false;
        
        protected override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (entity == null) return UniTask.FromResult(true);
            
            var character = entity as ICharacterActionTarget;
            if (character == null) return UniTask.FromResult(false);
            
            character.SetEnableControl(!_isLock);
            
            return UniTask.FromResult(true);
        }
    }
}
