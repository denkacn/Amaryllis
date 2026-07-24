using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
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
        

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            if (_delayTime == 0)
            {
                Set();
            }
            else
            {
                WaitAndSet();
            }

            await Task.Yield();
            return true;
        }

        private async void WaitAndSet()
        {
            var waitTime = (int)(_delayTime * 1000);
            await Task.Delay(waitTime);

            Set();
        }

        private void Set()
        {
            _rigidbody.isKinematic = _isKinematic;
        }
    }
}
