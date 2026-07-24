using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Entities.Models;
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

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            var character = entity as CharacterBaseEntity;
            
            if (character != null)
            {
                //character.PutItem(_items);
            }

            await Task.Yield();
            return true;
        }
    }
}
