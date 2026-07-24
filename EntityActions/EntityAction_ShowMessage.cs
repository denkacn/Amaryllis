using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
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
        
        protected override async Task<bool> RunLogic(IEntity entity)
        {            
            ShowMessage();
            
            await Task.Yield();
            return true;
        }

        private void ShowMessage()
        {
            //GameSceneManager.Inst.UiManager.ShowInfoMessage(_messageLocId, _massageType, true, _clip);
        }
    }
}
