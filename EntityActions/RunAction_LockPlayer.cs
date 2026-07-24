using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Entities.Models;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_LockPlayer : BaseRunAction
    {
        [SerializeField] private bool _isLock = false;
        
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            if (entity == null) return true;
            
            var character = entity as CharacterBaseEntity;
            
            character.SetEnableControl(!_isLock);
            
            await Task.Yield();
            return true;
        }
    }
}
