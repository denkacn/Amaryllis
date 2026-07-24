using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class EntityAction_ShowTutorialView : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField] 
        private string[] _tutorialsId;

        private int _index = 0;
        
        protected override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            ShowTutorial();
            
            return UniTask.FromResult(true);
        }

        private void ShowTutorial()
        {
            ShowNext();
        }

        private void ShowNext()
        {
            if (_index < _tutorialsId.Length)
            {
                // GameSceneManager.Inst.UiManager.ShowTutorial(_tutorialsId[_index], () =>
                // {
                //     _index++;
                //     
                //     ShowNext();
                // });
            }
        }
    }
}
