using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Entities.Interfaces;
using Amaryllis.States.Models;
using Cysharp.Threading.Tasks;

namespace Amaryllis.Actions.Helpers
{
    public static class RunActionLogicHelper
    {
        public static async UniTask<bool> RunActionsAsync(ExecTimeType execTime, IEntity entity, List<IRunAction> actions, CancellationToken cancellationToken = default)
        {
            if (actions == null)
            {
                return true;
            }

            var correctAction = actions.FindAll(a => a.ExecTime == execTime)
                .OrderByDescending(a => a.ExecPriority);
            var isOk = true;
            
            foreach (var action in correctAction)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var result = await action.Run(entity, cancellationToken);
                
                if (!result)
                {
                    isOk = false;
                }
            }

            return isOk;
        }
    }
}
