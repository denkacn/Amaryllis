using System.Collections.Generic;
using System.Linq;
using Amaryllis.Actions.Helpers;
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
            var actions = state
                .GetComponentsInChildren<MonoBehaviour>(true)
                .OfType<IRunAction>()
                .ToList();

            return CompositeRunActionUtility.GetRootActions(actions)
                .OrderBy(action => action.ExecTime)
                .ThenByDescending(action => action.ExecPriority)
                .Select(BuildActionModel)
                .ToList();
        }

        private static StateDebugActionModel BuildActionModel(IRunAction action)
        {
            var component = action as Component;
            return new StateDebugActionModel(
                component,
                component == null ? action.GetType().Name : component.GetType().Name,
                action.ExecTime,
                action.ExecPriority,
                IsActionEnabled(action),
                BuildActionConditions(action),
                BuildChildActions(action));
        }

        private static IReadOnlyList<StateDebugActionModel> BuildChildActions(IRunAction action)
        {
            return action is ICompositeRunAction composite && composite.ChildActions != null
                ? composite.ChildActions
                    .Where(childAction => childAction != null)
                    .Select(BuildActionModel)
                    .ToList()
                : new List<StateDebugActionModel>();
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

        private static bool IsActionEnabled(IRunAction action)
        {
            return action is not BaseRunAction baseRunAction || baseRunAction.IsEnabled;
        }
    }
}
