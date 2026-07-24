using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.States.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_SetStateToStateObject : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField]
        private GameObject _iEOStateOwner;
        [SerializeField]
        private int _toStateId;
        [SerializeField] 
        private float _delayTime = 0;

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            if (_delayTime == 0)
            {
                SetState();
            }
            else
            {
                WaitAndSetState();
            }

            await Task.Yield();
            return true;
        }
        
        private async void WaitAndSetState()
        {
            var waitTime = (int)(_delayTime * 1000);
            await Task.Delay(waitTime);

            SetState();
        }
                                                      
        private void SetState()
        {
            var stateOwner = _iEOStateOwner.GetComponent<IStatesObject>();
            if (stateOwner != null)
            {
                stateOwner.MoveToStateByIdAsync(_toStateId);
            }
        }
    }
}
