using System.Collections.Generic;
using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_ParalelActionsSequence  : BaseRunAction
    {
        [SerializeField] private List<ActionsSequenceItem> _actionItems;
        [SerializeField] private float _actionTime;
        
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            foreach (var item in _actionItems)
            {
                item.RunAction.Run(entity);
            }

            await Task.Delay((int)(_actionTime * 1000));
            return true;
        }
    }
}