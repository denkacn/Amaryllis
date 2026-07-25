using System.Collections.Generic;
using System.Linq;
using Amaryllis.Actions.Interfaces;

namespace Amaryllis.Actions.Helpers
{
    public static class CompositeRunActionUtility
    {
        public static List<IRunAction> GetRootActions(IEnumerable<IRunAction> actions)
        {
            var allActions = actions?.Where(action => action != null).ToList() ?? new List<IRunAction>();
            var childActions = new HashSet<IRunAction>();

            foreach (var action in allActions)
            {
                CollectChildActions(action, childActions);
            }

            return allActions
                .Where(action => !childActions.Contains(action))
                .ToList();
        }

        private static void CollectChildActions(IRunAction action, ISet<IRunAction> childActions)
        {
            if (action is not ICompositeRunAction composite || composite.ChildActions == null)
            {
                return;
            }

            foreach (var childAction in composite.ChildActions)
            {
                if (childAction == null || !childActions.Add(childAction))
                {
                    continue;
                }

                CollectChildActions(childAction, childActions);
            }
        }
    }
}
