using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Entities.Models;
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
            var character = entity as CharacterBaseEntity;
            
            if (character != null)
            {
                //character.PutItem(_items);
            }

            return UniTask.FromResult(true);
        }
    }
}
