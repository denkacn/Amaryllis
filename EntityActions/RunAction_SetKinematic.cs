using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_SetKinematic : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField]
        private bool _isKinematic;
        [SerializeField] 
        private float _delayTime = 0;
        [SerializeField] 
        private Rigidbody _rigidbody;
        

        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (_delayTime > 0)
            {
                await UniTask.Delay((int)(_delayTime * 1000), cancellationToken: cancellationToken);
            }
            
            Set();
            return true;
        }

        private void Set()
        {
            _rigidbody.isKinematic = _isKinematic;
        }
    }
}
