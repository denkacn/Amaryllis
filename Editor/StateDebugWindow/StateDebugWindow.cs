using Amaryllis.Entities.Models;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Actions.Models;
using Amaryllis.Debugging;
using Amaryllis.States.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Amaryllis.Editor.StateDebugWindow.StateDebugViewSettings;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal sealed class StateDebugWindow : EditorWindow
    {
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

            var layout = StateDebugLayoutBuilder.Build(graph);
            var canvasRect = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            var contentRect = new Rect(0f, 0f, layout.ContentSize.x, layout.ContentSize.y);

            _scrollPosition = GUI.BeginScrollView(canvasRect, _scrollPosition, contentRect);

            StateDebugEdgeRenderer.DrawGrid(contentRect);
            StateDebugEdgeRenderer.DrawEdges(graph, layout);
            var nodeRenderer = new StateDebugNodeRenderer(_runningActionComponents, _lastActionResults);
            nodeRenderer.DrawEntityNode(graph, layout.EntityRect, ExecTargetEntity);

            foreach (var state in graph.States)
            {
                nodeRenderer.DrawStateNode(
                    graph,
                    state,
                    layout.StateRects[state],
                    _previousStateId,
                    _lastStateChangeTime,
                    stateId => SetDebugState(graph, stateId),
                    RunDebugAction);
            }

            GUI.EndScrollView();
        }

        private void RunDebugAction(StateDebugActionModel actionModel)
        {
            if (actionModel.Component == null || actionModel.Component is not IRunAction action)
            {
                return;
            }

            AmaryllisDebugCommands.RunAction(action, _targetEntity, actionModel.ExecTime);
        }

        private void ExecTargetEntity()
        {
            AddTimelineEntry(EditorApplication.timeSinceStartup, "Debug exec entity", new Color(0.95f, 0.78f, 0.25f));
            AmaryllisDebugCommands.ExecEntity(_targetEntity);
        }

        private void SetDebugState(StateDebugGraphModel graph, int stateId)
        {
            AddTimelineEntry(EditorApplication.timeSinceStartup, $"Debug set state -> {stateId}", new Color(0.95f, 0.78f, 0.25f));
            AmaryllisDebugCommands.MoveToState(graph.StatesObject, stateId);
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

    }
}
