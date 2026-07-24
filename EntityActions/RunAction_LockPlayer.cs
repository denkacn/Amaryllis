using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Entities.Models;
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
            
            var character = entity as CharacterBaseEntity;
            
            character.SetEnableControl(!_isLock);
            
            return UniTask.FromResult(true);
        }
    }
}
