using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class EntityAction_ShowMessage : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        //[SerializeField] 
        //private EInfoMessageType _massageType;
        [SerializeField] 
        private string _messageLocId;
        [SerializeField] 
        private AudioClip _clip;
        
        protected override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {            
            ShowMessage();
            
            return UniTask.FromResult(true);
        }

        private void ShowMessage()
        {
            //GameSceneManager.Inst.UiManager.ShowInfoMessage(_messageLocId, _massageType, true, _clip);
        }
    }
}
