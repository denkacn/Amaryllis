using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.States.Interfaces;
using Cysharp.Threading.Tasks;
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

        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (_delayTime > 0)
            {
                await UniTask.Delay((int)(_delayTime * 1000), cancellationToken: cancellationToken);
            }

            await SetState(cancellationToken);
            return true;
        }
                                                      
        private async UniTask SetState(CancellationToken cancellationToken)
        {
            var stateOwner = _iEOStateOwner.GetComponent<IStatesObject>();
            if (stateOwner != null)
            {
                await stateOwner.MoveToStateByIdAsync(_toStateId, cancellationToken);
            }
        }
    }
}
