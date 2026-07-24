using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_ShowDialogue : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField] 
        private string _messageLocId;

        [SerializeField] private BaseRunAction[] _startDialogueAction;
        [SerializeField] private BaseRunAction[] _endDialogueAction;

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            ShowDialogue(entity);

            await Task.Yield();
            return true;
        }

        private void ShowDialogue(IEntity entity)
        {
            RunAction(_startDialogueAction, entity);
            
            // GameSceneManager.Inst.UiManager.ShowDialogueMessage(_messageLocId,
            //     GameSceneManager.Inst.EntitysManager.GetMainCharacter().transform, () =>
            //     {
            //         RunAction(_endDialogueAction, entity, owner);
            //     }); 
        }

        private void RunAction(BaseRunAction[] actionList, IEntity entity)
        {
            if (actionList == null || actionList.Length == 0) return;
            
            foreach (var action in actionList)
            {
                action.Run(entity);
            }
        }
    }
}
