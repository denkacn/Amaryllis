using System;
using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
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
        
        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (_delay > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: cancellationToken);
            }
            
            var character = entity as ICharacterActionTarget;
            if (character == null || _pointToLook == null)
            {
                return false;
            }
            
            character.LookAtPoint(_pointToLook.position, _lookTime);
            
            return true;
        }
    }
}
