using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_SetActiveEntity : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField] private GameObject _entityObject;
        [SerializeField] private bool _isActive = true;
        
        protected override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (_entityObject != null)
            {
                _entityObject.SetActive(_isActive);
            }
            
            return UniTask.FromResult(true);
        }
    }
}
