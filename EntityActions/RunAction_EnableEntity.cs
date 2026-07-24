using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_EnableEntity : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField] private GameObject _entityObject;
        [SerializeField] private bool _isEnable = true;
        
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            //var sceneEntity = _entityObject.GetComponent<ISceneEntity>();
            
            // if (sceneEntity != null)
            // {
            //     if(_isEnable)
            //         sceneEntity.SetEnable();
            //     else 
            //         sceneEntity.SetDisable();
            // }              
            
            await Task.Yield();
            
            return true;
        }
    }
}
