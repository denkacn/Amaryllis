using System.Collections.Generic;
using System.Linq;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Models;
using Amaryllis.States.Models;
using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal static class StateDebugAdapter
    {
        private const string RunActionConditionsFieldName = "_runActionConditions";

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
                .Select(state => new StateDebugNodeModel(state, BuildActions(state), BuildConditions(state)))
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
                    action.ExecPriority,
                    BuildActionConditions(action)))
                .ToList();
        }

        private static IReadOnlyList<StateDebugConditionModel> BuildConditions(StateObjectBase state)
        {
            return state
                .GetComponentsInChildren<MonoBehaviour>(true)
                .OfType<IStateCondition>()
                .Select(condition => new StateDebugConditionModel(
                    condition as Object,
                    condition.GetType().Name))
                .ToList();
        }

        private static IReadOnlyList<StateDebugConditionModel> BuildActionConditions(IRunAction action)
        {
            if (action is not BaseRunAction baseRunAction)
            {
                return new List<StateDebugConditionModel>();
            }

            var field = typeof(BaseRunAction).GetField(
                RunActionConditionsFieldName,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field == null)
            {
                return new List<StateDebugConditionModel>();
            }

            return (field.GetValue(baseRunAction) as IEnumerable<IRunActionCondition>)?
                .Where(condition => condition != null)
                .Select(condition => new StateDebugConditionModel(null, condition.GetType().Name))
                .ToList()
                ?? new List<StateDebugConditionModel>();
        }
    }
}
