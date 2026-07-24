using System.Collections.Generic;
using System.Linq;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Entities.Models;
using Amaryllis.States.Models;
using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal static class StateDebugAdapter
    {
        public static StateDebugGraphModel Build(HasStateEntity entity)
        {
            if (entity == null)
            {
                return new StateDebugGraphModel(null, null, new List<StateDebugNodeModel>());
            }

            var statesObject = entity.GetComponentInChildren<StatesObjectBase>(true);
            if (statesObject == null)
            {
                return new StateDebugGraphModel(entity, null, new List<StateDebugNodeModel>());
            }

            var states = statesObject
                .GetComponentsInChildren<StateObjectBase>(true)
                .Where(state => state != null)
                .OrderBy(state => state.StateId)
                .Select(state => new StateDebugNodeModel(state, BuildActions(state)))
                .ToList();

            return new StateDebugGraphModel(entity, statesObject, states);
        }

        private static IReadOnlyList<StateDebugActionModel> BuildActions(StateObjectBase state)
        {
            return state
                .GetComponentsInChildren<MonoBehaviour>(true)
                .OfType<IRunAction>()
                .OrderBy(action => action.ExecTime)
                .ThenBy(action => action.ExecPriority)
                .Select(action => new StateDebugActionModel(
                    action as Component,
                    (action as Component) == null ? action.GetType().Name : (action as Component).GetType().Name,
                    action.ExecTime,
                    action.ExecPriority))
                .ToList();
        }
    }
}
