using Amaryllis.Entities.Interfaces;
using Amaryllis.Entities.Managers;
using Amaryllis.Logs;
using UnityEngine;

namespace Amaryllis.Entities.Initiators
{
    public class EntityInitiator : MonoBehaviour
    {
        private IEntity _entity;
        
        private void Awake()
        {
            AmaryllisLog.IsLogEnable = true;
            
            if (SceneEntitiesManager.IsInit)
            {
                Init();
            }
            else
            {
                SceneEntitiesManager.SceneEntitiesManagerInitHandler += SceneEntitiesManagerInit;
            }
        }

        private void Init()
        {
            _entity = GetComponent<IEntity>();
            _entity.Create();
        }
        
        private void SceneEntitiesManagerInit()
        {
            SceneEntitiesManager.SceneEntitiesManagerInitHandler -= SceneEntitiesManagerInit;
            Init();
        }
    }
}