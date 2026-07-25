using System.Linq;
using System.Threading;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_ParalelActionsSequence  : CompositeRunActionBase
    {
        [SerializeField] private System.Collections.Generic.List<ActionsSequenceItem> _actionItems;
        [SerializeField] private float _actionTime;
        public override System.Collections.Generic.IReadOnlyList<IRunAction> ChildActions => _actionItems?
            .Where(item => item?.RunAction != null)
            .Select(item => (IRunAction)item.RunAction)
            .ToList()
            ?? new System.Collections.Generic.List<IRunAction>();
        
        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            var tasks = _actionItems
                .Where(item => item.RunAction != null)
                .Select(item => item.RunAction.Run(entity, cancellationToken));

            var results = await UniTask.WhenAll(tasks);

            if (_actionTime > 0)
            {
                await UniTask.Delay((int)(_actionTime * 1000), cancellationToken: cancellationToken);
            }
            
            return results.All(result => result != RunActionResult.Failed && result != RunActionResult.Canceled);
        }
    }
}
