using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
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

        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            await ShowDialogue(entity, cancellationToken);

            return true;
        }

        private async UniTask ShowDialogue(IEntity entity, CancellationToken cancellationToken)
        {
            await RunAction(_startDialogueAction, entity, cancellationToken);
            
            // GameSceneManager.Inst.UiManager.ShowDialogueMessage(_messageLocId,
            //     GameSceneManager.Inst.EntitysManager.GetMainCharacter().transform, () =>
            //     {
            //         RunAction(_endDialogueAction, entity, owner);
            //     }); 
        }

        private async UniTask RunAction(BaseRunAction[] actionList, IEntity entity, CancellationToken cancellationToken)
        {
            if (actionList == null || actionList.Length == 0) return;
            
            foreach (var action in actionList)
            {
                if (action != null)
                {
                    var result = await action.Run(entity, cancellationToken);
                    if (result == RunActionResult.Failed || result == RunActionResult.Canceled)
                    {
                        return;
                    }
                }
            }
        }
    }
}
