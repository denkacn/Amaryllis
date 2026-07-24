using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_EnableEntity : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField] private GameObject _entityObject;
        [SerializeField] private bool _isEnable = true;
        
        protected override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            //var sceneEntity = _entityObject.GetComponent<ISceneEntity>();
            
            // if (sceneEntity != null)
            // {
            //     if(_isEnable)
            //         sceneEntity.SetEnable();
            //     else 
            //         sceneEntity.SetDisable();
            // }              
            
            return UniTask.FromResult(true);
        }
    }
}
