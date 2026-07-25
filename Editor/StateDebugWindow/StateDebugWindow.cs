using Amaryllis.Entities.Models;
using Amaryllis.Actions.Models;
using Amaryllis.Debugging;
using Amaryllis.States.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal sealed class StateDebugWindow : EditorWindow
    {
        private const float EntityNodeWidth = 260f;
        private const float EntityNodeHeight = 78f;
        private const float StateNodeWidth = 260f;
        private const float StateHeaderHeight = 72f;
        private const float ActionRowHeight = 20f;
        private const float ConditionRowHeight = 18f;
        private const float GroupHeaderHeight = 18f;
        private const float SectionSpacing = 8f;
        private const float CanvasPadding = 32f;
        private const float StateHorizontalSpacing = 34f;
        private const float StateVerticalSpacing = 42f;
        private const int MaxTimelineEntries = 80;

        [SerializeField] private HasStateEntity _targetEntity;
        [SerializeField] private bool _isTargetLocked;
        [SerializeField] private string _targetGlobalId;
        private Vector2 _scrollPosition;
        private Vector2 _timelineScrollPosition;
        private readonly HashSet<Component> _runningActionComponents = new HashSet<Component>();
        private readonly Dictionary<Component, RunActionResult> _lastActionResults = new Dictionary<Component, RunActionResult>();
        private readonly List<DebugTimelineEntry> _timelineEntries = new List<DebugTimelineEntry>();
        private int _previousStateId = -1;
        private double _lastStateChangeTime;

        public static void Open(HasStateEntity entity)
        {
            var window = GetWindow<StateDebugWindow>();
            window.titleContent = new GUIContent("State Debug");
            window.SetTarget(entity);
            window.Show();
        }

        [MenuItem("Tools/Amaryllis/State Debug Window")]
        private static void OpenFromMenu()
        {
            Open(GetSelectedEntity());
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            AmaryllisDebugEvents.StateChanged += OnStateChanged;
            AmaryllisDebugEvents.ActionStarted += OnActionStarted;
            AmaryllisDebugEvents.ActionFinished += OnActionFinished;

            RestoreTargetIfNeeded();
            if (_targetEntity == null)
            {
                SetTarget(GetSelectedEntity());
            }
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            AmaryllisDebugEvents.StateChanged -= OnStateChanged;
            AmaryllisDebugEvents.ActionStarted -= OnActionStarted;
            AmaryllisDebugEvents.ActionFinished -= OnActionFinished;
        }

        private void OnInspectorUpdate()
        {
            if (EditorApplication.isPlaying)
            {
                Repaint();
            }
        }

        private void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.Space(8f);

            var graph = StateDebugAdapter.Build(_targetEntity);
            DrawTargetInfo(graph);
            EditorGUILayout.Space(8f);
            DrawTimeline();
            EditorGUILayout.Space(8f);
            DrawGraph(graph);
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var locked = GUILayout.Toggle(_isTargetLocked, "Lock Target", EditorStyles.toolbarButton, GUILayout.Width(90f));
                if (locked != _isTargetLocked)
                {
                    _isTargetLocked = locked;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Use Selection", EditorStyles.toolbarButton, GUILayout.Width(100f)))
                {
                    SetTarget(GetSelectedEntity());
                }
            }
        }

        private void DrawTargetInfo(StateDebugGraphModel graph)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField("Entity", _targetEntity, typeof(HasStateEntity), true);
                }

                if (_targetEntity == null)
                {
                    EditorGUILayout.HelpBox("Select a HasStateEntity or open this window from its inspector.", MessageType.Info);
                    return;
                }

                EditorGUILayout.LabelField("GameObject", _targetEntity.gameObject.name);
                EditorGUILayout.LabelField("Instance ID", _targetEntity.GetInstanceID().ToString());

                if (graph.StatesObject == null)
                {
                    EditorGUILayout.HelpBox("No StatesObjectBase found in this entity children.", MessageType.Warning);
                    return;
                }

                EditorGUILayout.ObjectField("States Object", graph.StatesObject, typeof(UnityEngine.Object), true);
                EditorGUILayout.LabelField("States", graph.States.Count.ToString());
            }
        }

        private void DrawTimeline()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Timeline", EditorStyles.boldLabel);

                    using (new EditorGUI.DisabledScope(_timelineEntries.Count == 0))
                    {
                        if (GUILayout.Button("Clear", GUILayout.Width(60f)))
                        {
                            _timelineEntries.Clear();
                            Repaint();
                        }
                    }
                }

                if (_timelineEntries.Count == 0)
                {
                    EditorGUILayout.LabelField("No debug events yet.", EditorStyles.miniLabel);
                    return;
                }

                _timelineScrollPosition = EditorGUILayout.BeginScrollView(_timelineScrollPosition, GUILayout.Height(112f));
                foreach (var entry in _timelineEntries)
                {
                    var color = GUI.color;
                    GUI.color = entry.Color;
                    EditorGUILayout.LabelField(entry.Text, EditorStyles.miniLabel);
                    GUI.color = color;
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawGraph(StateDebugGraphModel graph)
        {
            if (graph.Entity == null || graph.StatesObject == null)
            {
                return;
            }

            var layout = BuildLayout(graph);
            var canvasRect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            var contentRect = new Rect(0f, 0f, layout.ContentSize.x, layout.ContentSize.y);

            _scrollPosition = GUI.BeginScrollView(canvasRect, _scrollPosition, contentRect);

            DrawGrid(contentRect);
            DrawEdges(graph, layout);
            DrawEntityNode(graph, layout.EntityRect);

            foreach (var state in graph.States)
            {
                DrawStateNode(graph, state, layout.StateRects[state]);
            }

            GUI.EndScrollView();
        }

        private static GraphLayout BuildLayout(StateDebugGraphModel graph)
        {
            const int columns = 3;
            var stateRects = new Dictionary<StateDebugNodeModel, Rect>();
            var entityRect = new Rect(CanvasPadding, CanvasPadding, EntityNodeWidth, EntityNodeHeight);
            var startY = entityRect.yMax + 72f;
            var rowHeights = new List<float>();

            for (var index = 0; index < graph.States.Count; index++)
            {
                var state = graph.States[index];
                var row = index / columns;
                var height = CalculateStateNodeHeight(state);
                while (rowHeights.Count <= row)
                {
                    rowHeights.Add(0f);
                }

                rowHeights[row] = Mathf.Max(rowHeights[row], height);
            }

            var rowY = new List<float>(rowHeights.Count);
            var currentY = startY;
            foreach (var rowHeight in rowHeights)
            {
                rowY.Add(currentY);
                currentY += rowHeight + StateVerticalSpacing;
            }

            for (var index = 0; index < graph.States.Count; index++)
            {
                var state = graph.States[index];
                var column = index % columns;
                var row = index / columns;
                var height = CalculateStateNodeHeight(state);
                var x = CanvasPadding + column * (StateNodeWidth + StateHorizontalSpacing);
                var y = rowY[row];
                var rect = new Rect(x, y, StateNodeWidth, height);
                stateRects.Add(state, rect);
            }

            var contentWidth = CanvasPadding * 2f + columns * StateNodeWidth + (columns - 1) * StateHorizontalSpacing;
            var contentHeight = currentY - StateVerticalSpacing + CanvasPadding;
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
                        height += ActionRowHeight;
                    }
                }
            }

            return height + 18f;
        }

        private static void DrawGrid(Rect contentRect)
        {
            const float spacing = 32f;
            Handles.BeginGUI();
            Handles.color = new Color(0f, 0f, 0f, 0.12f);

            for (var x = 0f; x < contentRect.width; x += spacing)
            {
                Handles.DrawLine(new Vector3(x, 0f), new Vector3(x, contentRect.height));
            }

            for (var y = 0f; y < contentRect.height; y += spacing)
            {
                Handles.DrawLine(new Vector3(0f, y), new Vector3(contentRect.width, y));
            }

            Handles.EndGUI();
        }

        private static void DrawEdges(StateDebugGraphModel graph, GraphLayout layout)
        {
            var statesById = graph.States.ToDictionary(state => state.StateId);

            Handles.BeginGUI();
            Handles.color = new Color(0.45f, 0.63f, 1f, 0.82f);

            foreach (var state in graph.States)
            {
                if (state.NextStateId == -1 || state.NextStateId == state.StateId || !statesById.TryGetValue(state.NextStateId, out var nextState))
                {
                    continue;
                }

                DrawArrow(layout.StateRects[state], layout.StateRects[nextState]);
            }

            Handles.color = new Color(0.7f, 0.7f, 0.7f, 0.45f);
            foreach (var state in graph.States)
            {
                DrawBezierLine(layout.EntityRect, layout.StateRects[state]);
            }

            Handles.EndGUI();
        }

        private static void DrawBezierLine(Rect from, Rect to)
        {
            var start = new Vector3(from.center.x, from.yMax);
            var end = new Vector3(to.center.x, to.yMin);
            var distance = Vector3.Distance(start, end);
            var tangentLength = Mathf.Clamp(distance * 0.45f, 48f, 160f);
            var startTangent = start + Vector3.up * tangentLength;
            var endTangent = end - Vector3.up * tangentLength;

            Handles.DrawBezier(start, end, startTangent, endTangent, Handles.color, null, 2f);
        }

        private static void DrawArrow(Rect from, Rect to)
        {
            var start = new Vector3(from.xMax, from.center.y);
            var end = new Vector3(to.xMin, to.center.y);

            var distance = Vector3.Distance(start, end);
            var tangentLength = Mathf.Clamp(distance * 0.45f, 56f, 180f);
            var startTangent = start + Vector3.right * tangentLength;
            var endTangent = end - Vector3.right * tangentLength;

            Handles.DrawBezier(start, end, startTangent, endTangent, Handles.color, null, 3f);

            var direction = Vector2.right;
            if (direction == Vector2.zero)
            {
                return;
            }

            var normal = new Vector2(-direction.y, direction.x);
            var tip = (Vector2)end;
            var left = tip - direction * 11f + normal * 5f;
            var right = tip - direction * 11f - normal * 5f;
            Handles.DrawAAConvexPolygon(tip, left, right);
        }

        private static void DrawEntityNode(StateDebugGraphModel graph, Rect rect)
        {
            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);
            var inner = RectOffset(rect, 10f, 8f);
            GUI.Label(new Rect(inner.x, inner.y, inner.width, 18f), "Entity", EditorStyles.boldLabel);
            GUI.Label(new Rect(inner.x, inner.y + 22f, inner.width, 18f), graph.Entity.name);
            GUI.Label(new Rect(inner.x, inner.y + 44f, inner.width, 18f), graph.StatesObject.name, EditorStyles.miniLabel);
        }

        private void DrawStateNode(StateDebugGraphModel graph, StateDebugNodeModel state, Rect rect)
        {
            var isCurrent = graph.StatesObject.CurrentStateId == state.StateId;
            var isPrevious = state.StateId == _previousStateId && EditorApplication.timeSinceStartup - _lastStateChangeTime < 0.6d;
            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = isCurrent ? new Color(0.35f, 1f, 0.48f) : isPrevious ? new Color(1f, 0.82f, 0.35f) : backgroundColor;
            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);
            GUI.backgroundColor = backgroundColor;

            var inner = RectOffset(rect, 10f, 8f);
            GUI.Label(new Rect(inner.x, inner.y, inner.width, 18f), $"{state.Name}  [{state.StateId}]", EditorStyles.boldLabel);
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

                foreach (var action in group)
                {
                    var actionRect = new Rect(inner.x, actionsY, inner.width, ActionRowHeight);
                    var isRunning = action.Component != null && _runningActionComponents.Contains(action.Component);
                    var lastResult = default(RunActionResult);
                    var hasLastResult = action.Component != null && _lastActionResults.TryGetValue(action.Component, out lastResult);
                    DrawActionRow(actionRect, action, isRunning, hasLastResult, lastResult);
                    actionsY += ActionRowHeight;
                }
            }
        }

        private static void DrawActionRow(Rect rect, StateDebugActionModel action, bool isRunning, bool hasLastResult, RunActionResult lastResult)
        {
            var backgroundColor = GUI.backgroundColor;
            if (isRunning)
            {
                GUI.backgroundColor = new Color(0.35f, 0.72f, 1f);
            }
            else if (hasLastResult && (lastResult == RunActionResult.Failed || lastResult == RunActionResult.Canceled))
            {
                GUI.backgroundColor = new Color(1f, 0.45f, 0.45f);
            }

            var status = isRunning ? "RUN" : hasLastResult ? lastResult.ToString() : string.Empty;
            var conditions = FormatConditionSummary(action.Conditions);
            var label = string.IsNullOrEmpty(status)
                ? $"{action.Priority} | {action.Name}"
                : $"{action.Priority} | {status} | {action.Name}";
            if (!string.IsNullOrEmpty(conditions))
            {
                label += $" | if {conditions}";
            }

            if (GUI.Button(rect, label, EditorStyles.miniButtonLeft) && action.Component != null)
            {
                Selection.activeObject = action.Component.gameObject;
                EditorGUIUtility.PingObject(action.Component.gameObject);
            }

            GUI.backgroundColor = backgroundColor;
        }

        private static float DrawConditionSection(Rect inner, StateDebugNodeModel state)
        {
            var y = inner.y + 48f;
            GUI.Label(new Rect(inner.x, y, inner.width, 16f), "Conditions", EditorStyles.miniBoldLabel);
            y += GroupHeaderHeight;

            foreach (var condition in state.Conditions)
            {
                DrawConditionRow(new Rect(inner.x, y, inner.width, ConditionRowHeight), condition);
                y += ConditionRowHeight;
            }

            return y;
        }

        private static void DrawConditionRow(Rect rect, StateDebugConditionModel condition)
        {
            GUI.Label(rect, $"- {FormatConditionLabel(condition)}", EditorStyles.miniLabel);
        }

        private static string FormatConditionSummary(IReadOnlyList<StateDebugConditionModel> conditions)
        {
            return conditions == null || conditions.Count == 0
                ? string.Empty
                : string.Join(", ", conditions.Select(FormatConditionLabel));
        }

        private static string FormatConditionLabel(StateDebugConditionModel condition)
        {
            var label = condition.Name;
            if (condition.Source is Component component && component.gameObject.name != condition.Name)
            {
                label += $" ({component.gameObject.name})";
            }

            if (TryGetSerializedBool(condition.Source, "_isCanExec", out var isCanExec))
            {
                label += $" = {isCanExec}";
            }

            return label;
        }

        private static bool TryGetSerializedBool(UnityEngine.Object source, string propertyName, out bool value)
        {
            value = false;
            if (source == null)
            {
                return false;
            }

            var serializedObject = new SerializedObject(source);
            var property = serializedObject.FindProperty(propertyName);
            if (property == null || property.propertyType != SerializedPropertyType.Boolean)
            {
                return false;
            }

            value = property.boolValue;
            return true;
        }

        private static Rect RectOffset(Rect rect, float horizontal, float vertical)
        {
            return new Rect(rect.x + horizontal, rect.y + vertical, rect.width - horizontal * 2f, rect.height - vertical * 2f);
        }

        private static string FormatNextState(int nextStateId)
        {
            return nextStateId == -1 ? "Terminal" : nextStateId.ToString();
        }

        private void OnSelectionChanged()
        {
            if (_isTargetLocked)
            {
                return;
            }

            var selectedEntity = GetSelectedEntity();
            if (selectedEntity == null || selectedEntity == _targetEntity)
            {
                return;
            }

            SetTarget(selectedEntity);
        }

        private void SetTarget(HasStateEntity entity)
        {
            _targetEntity = entity;
            CacheTargetGlobalId(entity);
            ClearLiveState();
            Repaint();
        }

        private void OnStateChanged(StateChangedDebugEvent debugEvent)
        {
            if (!IsEventForCurrentTarget(debugEvent))
            {
                return;
            }

            _previousStateId = debugEvent.PreviousStateId;
            _lastStateChangeTime = EditorApplication.timeSinceStartup;
            AddTimelineEntry(debugEvent.Time, $"State {debugEvent.PreviousStateId} -> {debugEvent.CurrentStateId}", new Color(0.36f, 0.9f, 0.42f));
            Repaint();
        }

        private void OnActionStarted(ActionDebugEvent debugEvent)
        {
            if (!IsActionForCurrentTarget(debugEvent) || debugEvent.ActionComponent == null)
            {
                return;
            }

            _runningActionComponents.Add(debugEvent.ActionComponent);
            _lastActionResults.Remove(debugEvent.ActionComponent);
            AddTimelineEntry(debugEvent.Time, $"Action start [{debugEvent.ExecTime}] {FormatActionContext(debugEvent.ActionComponent)}", new Color(0.35f, 0.72f, 1f));
            Repaint();
        }

        private void OnActionFinished(ActionDebugEvent debugEvent)
        {
            if (!IsActionForCurrentTarget(debugEvent) || debugEvent.ActionComponent == null)
            {
                return;
            }

            _runningActionComponents.Remove(debugEvent.ActionComponent);
            if (debugEvent.Result.HasValue)
            {
                _lastActionResults[debugEvent.ActionComponent] = debugEvent.Result.Value;
            }

            var resultText = debugEvent.Result.HasValue ? debugEvent.Result.Value.ToString() : "Unknown";
            var color = debugEvent.Result == RunActionResult.Failed || debugEvent.Result == RunActionResult.Canceled
                ? new Color(1f, 0.45f, 0.45f)
                : new Color(0.72f, 0.72f, 0.72f);
            AddTimelineEntry(debugEvent.Time, $"Action finish [{debugEvent.ExecTime}] {resultText} {FormatActionContext(debugEvent.ActionComponent)}", color);
            Repaint();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                CacheTargetGlobalId(_targetEntity);
                return;
            }

            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.EnteredEditMode)
            {
                RestoreTargetIfNeeded();
            }

            if (state == PlayModeStateChange.EnteredPlayMode
                || state == PlayModeStateChange.EnteredEditMode
                || state == PlayModeStateChange.ExitingPlayMode)
            {
                ClearLiveState();
                Repaint();
            }
        }

        private bool IsEventForCurrentTarget(StateChangedDebugEvent debugEvent)
        {
            return _targetEntity != null
                   && debugEvent.StatesObject != null
                   && debugEvent.StatesObject.GetComponentInParent<HasStateEntity>() == _targetEntity;
        }

        private bool IsActionForCurrentTarget(ActionDebugEvent debugEvent)
        {
            return _targetEntity != null
                   && debugEvent.ActionComponent != null
                   && debugEvent.ActionComponent.GetComponentInParent<HasStateEntity>() == _targetEntity;
        }

        private void ClearLiveState()
        {
            _runningActionComponents.Clear();
            _lastActionResults.Clear();
            _timelineEntries.Clear();
            _timelineScrollPosition = Vector2.zero;
            _previousStateId = -1;
            _lastStateChangeTime = 0d;
        }

        private void AddTimelineEntry(double time, string message, Color color)
        {
            _timelineEntries.Insert(0, new DebugTimelineEntry($"[{time:0.000}] {message}", color));
            if (_timelineEntries.Count > MaxTimelineEntries)
            {
                _timelineEntries.RemoveAt(_timelineEntries.Count - 1);
            }
        }

        private static string FormatActionContext(Component actionComponent)
        {
            var actionType = actionComponent.GetType().Name;
            var actionName = actionComponent.name;
            var actionText = actionType == actionName
                ? actionType
                : $"{actionType} ({actionName})";
            var state = actionComponent.GetComponentInParent<StateObjectBase>();
            if (state == null)
            {
                return actionText;
            }

            return $"{state.name} [{state.StateId}] -> {actionText}";
        }

        private void RestoreTargetIfNeeded()
        {
            if (_targetEntity != null || string.IsNullOrEmpty(_targetGlobalId))
            {
                return;
            }

            if (!GlobalObjectId.TryParse(_targetGlobalId, out var globalObjectId))
            {
                return;
            }

            var targetObject = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(globalObjectId);
            if (targetObject is HasStateEntity entity)
            {
                _targetEntity = entity;
                return;
            }

            if (targetObject is GameObject gameObject)
            {
                _targetEntity = gameObject.GetComponentInParent<HasStateEntity>();
            }
        }

        private void CacheTargetGlobalId(HasStateEntity entity)
        {
            _targetGlobalId = entity == null
                ? string.Empty
                : GlobalObjectId.GetGlobalObjectIdSlow(entity).ToString();
        }

        private static HasStateEntity GetSelectedEntity()
        {
            return Selection.activeGameObject == null
                ? null
                : Selection.activeGameObject.GetComponentInParent<HasStateEntity>();
        }

        private readonly struct GraphLayout
        {
            public GraphLayout(Rect entityRect, IReadOnlyDictionary<StateDebugNodeModel, Rect> stateRects, Vector2 contentSize)
            {
                EntityRect = entityRect;
                StateRects = stateRects;
                ContentSize = contentSize;
            }

            public Rect EntityRect { get; }
            public IReadOnlyDictionary<StateDebugNodeModel, Rect> StateRects { get; }
            public Vector2 ContentSize { get; }
        }

        private readonly struct DebugTimelineEntry
        {
            public DebugTimelineEntry(string text, Color color)
            {
                Text = text;
                Color = color;
            }

            public string Text { get; }
            public Color Color { get; }
        }
    }
}
