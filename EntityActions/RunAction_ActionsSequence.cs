using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_ActionsSequence : BaseRunAction
    {
        [SerializeField] private List<ActionsSequenceItem> _actionItems;

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            foreach (var item in _actionItems)
            {
                await Task.Delay(TimeSpan.FromSeconds(item.PreDaley));
                
                await item.RunAction.Run(entity);
            }
            
            await Task.Yield();
            return true;
        }
    }

    [Serializable]
    public class ActionsSequenceItem
    {
        public float PreDaley;
        public BaseRunAction RunAction;
    }
}
