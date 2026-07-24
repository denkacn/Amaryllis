using System;
using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Entities.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_LookAtPoint : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField]
        private Transform _pointToLook;
        [SerializeField]
        private float _lookTime;
        [SerializeField]
        private float _delay;
        
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            if (_delay > 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(_delay));
            }
            
            var character = entity as CharacterBaseEntity;
            
            character.LookAtPoint(_pointToLook.position, _lookTime);
            
            await Task.Yield();
            return true;
        }
    }
}
