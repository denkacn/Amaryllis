using System;
using System.Collections.Generic;
using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_ActionsSequence : BaseRunAction
    {
        [SerializeField] private List<ActionsSequenceItem> _actionItems;

        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            var isOk = true;
            
            foreach (var item in _actionItems)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (item.PreDaley > 0)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(item.PreDaley), cancellationToken: cancellationToken);
                }

                if (item.RunAction == null)
                {
                    isOk = false;
                    continue;
                }
                
                var result = await item.RunAction.Run(entity, cancellationToken);
                isOk &= result != RunActionResult.Failed && result != RunActionResult.Canceled;
            }
            
            return isOk;
        }
    }

    [Serializable]
    public class ActionsSequenceItem
    {
        public float PreDaley;
        public BaseRunAction RunAction;
    }
}
