using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Amaryllis.Editor.StateDebugWindow.StateDebugViewSettings;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal static class StateDebugLayoutBuilder
    {
        public static GraphLayout Build(StateDebugGraphModel graph)
        {
            var stateRects = new Dictionary<StateDebugNodeModel, Rect>();
            var entityRect = new Rect(CanvasPadding, CanvasPadding, EntityNodeWidth, EntityNodeHeight);
            var startY = entityRect.yMax + 72f;
            var maxStateHeight = 0f;

            for (var index = 0; index < graph.States.Count; index++)
            {
                var state = graph.States[index];
                var height = CalculateStateNodeHeight(state);
                maxStateHeight = Mathf.Max(maxStateHeight, height);

                var x = CanvasPadding + index * (StateNodeWidth + StateHorizontalSpacing);
                stateRects.Add(state, new Rect(x, startY, StateNodeWidth, height));
            }

            var stateCount = Mathf.Max(graph.States.Count, 1);
            var contentWidth = CanvasPadding * 2f + stateCount * StateNodeWidth + (stateCount - 1) * StateHorizontalSpacing;
            var contentHeight = startY + maxStateHeight + CanvasPadding + graph.States.Count * EdgeRouteStep;
            return new GraphLayout(entityRect, stateRects, new Vector2(contentWidth, contentHeight));
        }

        private static float CalculateStateNodeHeight(StateDebugNodeModel state)
        {
            var height = StateHeaderHeight;
            if (state.Conditions.Count > 0)
            {
                height += GroupHeaderHeight + state.Conditions.Count * ConditionRowHeight + SectionSpacing;
            }

            height += GroupHeaderHeight;
            if (state.Actions.Count == 0)
            {
                height += ActionRowHeight;
            }
            else
            {
                foreach (var group in state.Actions.GroupBy(action => action.ExecTime))
                {
                    height += GroupHeaderHeight;
                    foreach (var action in group)
                    {
                        height += CalculateActionNodeHeight(action);
                    }
                }
            }

            return height + 18f;
        }

        private static float CalculateActionNodeHeight(StateDebugActionModel action)
        {
            var height = ActionRowHeight;
            if (action.ChildActions == null)
            {
                return height;
            }

            foreach (var childAction in action.ChildActions)
            {
                height += CalculateActionNodeHeight(childAction);
            }

            return height;
        }
    }
}
