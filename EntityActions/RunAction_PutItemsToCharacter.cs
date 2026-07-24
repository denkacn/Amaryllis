using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_PutItemsToCharacter : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        /*[SerializeField]
        [TableList]
        private ItemAmountInfo[] _items;*/

        protected override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            var character = entity as ICharacterActionTarget;
            
            if (character != null)
            {
                //character.PutItem(_items);
            }

            return UniTask.FromResult(true);
        }
    }
}
