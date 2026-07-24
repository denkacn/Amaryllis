using System;
using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_Delay : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField] 
        private float _delayTime = 0;
        
        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (_delayTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_delayTime), cancellationToken: cancellationToken);
            }
            
            return true;
        }
    }
}
