using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
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
        
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            ShowTutorial();
            
            await Task.Yield();
            return true;
        }

        private async void ShowTutorial()
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
