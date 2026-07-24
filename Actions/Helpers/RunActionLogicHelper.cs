using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Entities.Interfaces;
using Amaryllis.States.Models;

namespace Amaryllis.Actions.Helpers
{
    public static class RunActionLogicHelper
    {
        public static async Task<bool> RunActionsAsync(ExecTimeType execTime, IEntity entity, List<IRunAction> actions)
        {
            var correctAction = actions.FindAll(a => a.ExecTime == execTime)
                .OrderByDescending(a => a.ExecPriority);
            var isOk = true;
            
            foreach (var action in correctAction)
            {
                var result = await action.Run(entity);
                
                if (!result)
                {
                    isOk = false;
                }
            }

            return isOk;
        }
    }
}