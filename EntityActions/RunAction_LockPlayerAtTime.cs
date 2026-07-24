using System.Collections;
using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Entities.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_LockPlayerAtTime : BaseRunAction
    {
        [ShowInInspector, PropertySpace] 
        
        [SerializeField] 
        private float _lockTime;

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            StartCoroutine(LockCharacterAtTime(entity));
            
            await Task.Yield();
            return true;
        }

        private IEnumerator LockCharacterAtTime(IEntity entity)
        {
            var character = entity as CharacterBaseEntity;
            
            if (character == null) yield break;
            
            character.SetEnableControl(false);
            yield return new WaitForSeconds(_lockTime);
            character.SetEnableControl(true);
        }
    }
}
