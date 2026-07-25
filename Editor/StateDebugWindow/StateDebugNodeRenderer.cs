using System;
using System.Collections.Generic;
using System.Linq;
using Amaryllis.Actions.Models;
using Amaryllis.States.Models;
using UnityEditor;
using UnityEngine;
using static Amaryllis.Editor.StateDebugWindow.StateDebugViewSettings;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal sealed class StateDebugNodeRenderer
    {
        private readonly IReadOnlyCollection<Component> _runningActionComponents;
        private readonly IReadOnlyDictionary<Component, RunActionResult> _lastActionResults;

        public StateDebugNodeRenderer(
            IReadOnlyCollection<Component> runningActionComponents,
            IReadOnlyDictionary<Component, RunActionResult> lastActionResults)
        {
            _runningActionComponents = runningActionComponents;
            _lastActionResults = lastActionResults;
        }

        public void DrawEntityNode(StateDebugGraphModel graph, Rect rect, Action onExec)
        {
            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);
            var inner = RectOffset(rect, 10f, 8f);
            GUI.Label(new Rect(inner.x, inner.y, inner.width - 54f, 18f), "Entity", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying || graph.Entity == null))
            {
                if (GUI.Button(new Rect(inner.xMax - 50f, inner.y, 50f, 18f), "Exec", EditorStyles.miniButton))
                {
                    onExec?.Invoke();
                }
            }

            GUI.Label(new Rect(inner.x, inner.y + 22f, inner.width, 18f), graph.Entity.name);
            GUI.Label(new Rect(inner.x, inner.y + 44f, inner.width, 18f), graph.StatesObject.name, EditorStyles.miniLabel);
        }

        public void DrawStateNode(
            StateDebugGraphModel graph,
            StateDebugNodeModel state,
            Rect rect,
            int previousStateId,
            double lastStateChangeTime,
            Action<int> onSetState,
            Action<StateDebugActionModel> onRunAction)
        {
            var isCurrent = graph.StatesObject.CurrentStateId == state.StateId;
            var isPrevious = state.StateId == previousStateId && EditorApplication.timeSinceStartup - lastStateChangeTime < 0.6d;
            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = isCurrent ? new Color(0.35f, 1f, 0.48f) : isPrevious ? new Color(1f, 0.82f, 0.35f) : backgroundColor;
            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);
            GUI.backgroundColor = backgroundColor;

            var inner = RectOffset(rect, 10f, 8f);
            GUI.Label(new Rect(inner.x, inner.y, inner.width - 48f, 18f), $"{state.Name}  [{state.StateId}]", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying || isCurrent))
            {
                if (GUI.Button(new Rect(inner.xMax - 44f, inner.y, 44f, 18f), "Set", EditorStyles.miniButton))
                {
                    onSetState?.Invoke(state.StateId);
                }
            }

            GUI.Label(new Rect(inner.x, inner.y + 22f, inner.width, 16f), $"Next: {FormatNextState(state.NextStateId)}", EditorStyles.miniLabel);

            var actionsY = inner.y + 48f;
            if (state.Conditions.Count > 0)
            {
                actionsY = DrawConditionSection(inner, state);
                actionsY += SectionSpacing;
            }

            GUI.Label(new Rect(inner.x, actionsY, inner.width, 16f), "Actions", EditorStyles.miniBoldLabel);
            actionsY += 18f;

            if (state.Actions.Count == 0)
            {
                GUI.Label(new Rect(inner.x, actionsY, inner.width, ActionRowHeight), "No actions", EditorStyles.miniLabel);
                return;
            }

            foreach (var group in state.Actions.GroupBy(action => action.ExecTime))
            {
                GUI.Label(new Rect(inner.x + 8f, actionsY, inner.width - 8f, GroupHeaderHeight), group.Key.ToString(), EditorStyles.miniBoldLabel);
                actionsY += GroupHeaderHeight;

                foreach (var action in group.OrderByDescending(action => action.Priority))
                {
                    actionsY = DrawActionTree(inner, actionsY, action, 0, onRunAction);
                }
            }
        }

        private float DrawActionTree(Rect inner, float y, StateDebugActionModel action, int depth, Action<StateDebugActionModel> onRunAction)
        {
            var indent = depth * 14f;
            var actionRect = new Rect(inner.x + indent, y, inner.width - indent, ActionRowHeight);
            var isRunning = action.Component != null && _runningActionComponents.Contains(action.Component);
            var lastResult = default(RunActionResult);
            var hasLastResult = action.Component != null && _lastActionResults.TryGetValue(action.Component, out lastResult);
            DrawActionRow(actionRect, action, depth, isRunning, hasLastResult, lastResult, onRunAction);
            y += ActionRowHeight;

            if (action.ChildActions == null)
            {
                return y;
            }

            foreach (var childAction in action.ChildActions)
            {
                y = DrawActionTree(inner, y, childAction, depth + 1, onRunAction);
            }

            return y;
        }

        private static void DrawActionRow(
            Rect rect,
            StateDebugActionModel action,
            int depth,
            bool isRunning,
            bool hasLastResult,
            RunActionResult lastResult,
            Action<StateDebugActionModel> onRunAction)
        {
            var backgroundColor = GUI.backgroundColor;
            if (isRunning)
            {
                GUI.backgroundColor = new Color(0.35f, 0.72f, 1f);
            }
            else if (!action.IsEnabled)
            {
                GUI.backgroundColor = new Color(0.46f, 0.46f, 0.46f);
            }
            else if (hasLastResult && (lastResult == RunActionResult.Failed || lastResult == RunActionResult.Canceled))
            {
                GUI.backgroundColor = new Color(1f, 0.45f, 0.45f);
            }

            var label = BuildActionLabel(action, depth, isRunning, hasLastResult, lastResult);
            var runButtonRect = new Rect(rect.xMax - 42f, rect.y, 42f, rect.height);
            var labelRect = new Rect(rect.x, rect.y, rect.width - 44f, rect.height);

            if (GUI.Button(labelRect, label, EditorStyles.miniButtonLeft) && action.Component != null)
            {
                Selection.activeObject = action.Component.gameObject;
                EditorGUIUtility.PingObject(action.Component.gameObject);
            }

            using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying || isRunning || action.Component == null || !action.IsEnabled))
            {
                if (GUI.Button(runButtonRect, "Run", EditorStyles.miniButton))
                {
                    onRunAction?.Invoke(action);
                }
            }

            GUI.backgroundColor = backgroundColor;
        }

        private static string BuildActionLabel(StateDebugActionModel action, int depth, bool isRunning, bool hasLastResult, RunActionResult lastResult)
        {
            var status = isRunning ? "RUN" : hasLastResult ? lastResult.ToString() : string.Empty;
            var conditions = StateDebugConditionFormatter.FormatSummary(action.Conditions);
            var childMarker = action.ChildActions != null && action.ChildActions.Count > 0 ? " [+]" : string.Empty;
            var depthMarker = depth > 0 ? "> " : string.Empty;
            var enabledMarker = action.IsEnabled ? string.Empty : "DISABLED | ";
            var label = string.IsNullOrEmpty(status)
                ? $"{depthMarker}{enabledMarker}P:{action.Priority} | {action.Name}{childMarker}"
                : $"{depthMarker}{enabledMarker}P:{action.Priority} | {status} | {action.Name}{childMarker}";
            if (!string.IsNullOrEmpty(conditions))
            {
                label += $" | if {conditions}";
            }

            return label;
        }

        private static float DrawConditionSection(Rect inner, StateDebugNodeModel state)
        {
            var y = inner.y + 48f;
            GUI.Label(new Rect(inner.x, y, inner.width, 16f), "Conditions", EditorStyles.miniBoldLabel);
            y += GroupHeaderHeight;

            foreach (var condition in state.Conditions)
            {
                GUI.Label(new Rect(inner.x, y, inner.width, ConditionRowHeight), $"- {StateDebugConditionFormatter.FormatLabel(condition)}", EditorStyles.miniLabel);
                y += ConditionRowHeight;
            }

            return y;
        }

        private static Rect RectOffset(Rect rect, float horizontal, float vertical)
        {
            return new Rect(rect.x + horizontal, rect.y + vertical, rect.width - horizontal * 2f, rect.height - vertical * 2f);
        }

        private static string FormatNextState(int nextStateId)
        {
            return nextStateId == -1 ? "Terminal" : nextStateId.ToString();
        }
    }
}
