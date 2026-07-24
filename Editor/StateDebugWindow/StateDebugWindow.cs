using Amaryllis.Entities.Models;
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
        private const float CanvasPadding = 32f;
        private const float StateHorizontalSpacing = 34f;
        private const float StateVerticalSpacing = 42f;

        private HasStateEntity _targetEntity;
        private bool _isTargetLocked;
        private Vector2 _scrollPosition;

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
            if (_targetEntity == null)
            {
                SetTarget(GetSelectedEntity());
            }
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.Space(8f);

            var graph = StateDebugAdapter.Build(_targetEntity);
            DrawTargetInfo(graph);
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

                EditorGUILayout.ObjectField("States Object", graph.StatesObject, typeof(Object), true);
                EditorGUILayout.LabelField("States", graph.States.Count.ToString());
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
            var maxBottom = startY;

            for (var index = 0; index < graph.States.Count; index++)
            {
                var state = graph.States[index];
                var column = index % columns;
                var row = index / columns;
                var height = StateHeaderHeight + Mathf.Max(1, state.Actions.Count) * ActionRowHeight + 18f;
                var x = CanvasPadding + column * (StateNodeWidth + StateHorizontalSpacing);
                var y = startY + row * (StateHeaderHeight + 8 * ActionRowHeight + StateVerticalSpacing);
                var rect = new Rect(x, y, StateNodeWidth, height);
                stateRects.Add(state, rect);
                maxBottom = Mathf.Max(maxBottom, rect.yMax);
            }

            var contentWidth = CanvasPadding * 2f + columns * StateNodeWidth + (columns - 1) * StateHorizontalSpacing;
            var contentHeight = maxBottom + CanvasPadding;
            return new GraphLayout(entityRect, stateRects, new Vector2(contentWidth, contentHeight));
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
                if (state.NextStateId == -1 || !statesById.TryGetValue(state.NextStateId, out var nextState))
                {
                    continue;
                }

                DrawArrow(layout.StateRects[state], layout.StateRects[nextState]);
            }

            Handles.color = new Color(0.7f, 0.7f, 0.7f, 0.45f);
            foreach (var state in graph.States)
            {
                DrawLine(layout.EntityRect, layout.StateRects[state]);
            }

            Handles.EndGUI();
        }

        private static void DrawLine(Rect from, Rect to)
        {
            var start = new Vector3(from.center.x, from.yMax);
            var end = new Vector3(to.center.x, to.yMin);
            Handles.DrawAAPolyLine(2f, start, end);
        }

        private static void DrawArrow(Rect from, Rect to)
        {
            var start = new Vector3(from.xMax, from.center.y);
            var end = new Vector3(to.xMin, to.center.y);

            if (to.xMin < from.xMax && Mathf.Abs(to.center.y - from.center.y) > 1f)
            {
                start = new Vector3(from.center.x, from.yMax);
                end = new Vector3(to.center.x, to.yMin);
            }

            Handles.DrawAAPolyLine(3f, start, end);

            var direction = ((Vector2)(end - start)).normalized;
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

        private static void DrawStateNode(StateDebugGraphModel graph, StateDebugNodeModel state, Rect rect)
        {
            var isCurrent = graph.StatesObject.CurrentStateId == state.StateId;
            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = isCurrent ? new Color(0.45f, 0.95f, 0.55f) : backgroundColor;
            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);
            GUI.backgroundColor = backgroundColor;

            var inner = RectOffset(rect, 10f, 8f);
            GUI.Label(new Rect(inner.x, inner.y, inner.width, 18f), $"{state.Name}  [{state.StateId}]", EditorStyles.boldLabel);
            GUI.Label(new Rect(inner.x, inner.y + 22f, inner.width, 16f), $"Next: {FormatNextState(state.NextStateId)}", EditorStyles.miniLabel);

            var actionsY = inner.y + 48f;
            GUI.Label(new Rect(inner.x, actionsY, inner.width, 16f), "Actions", EditorStyles.miniBoldLabel);
            actionsY += 18f;

            if (state.Actions.Count == 0)
            {
                GUI.Label(new Rect(inner.x, actionsY, inner.width, ActionRowHeight), "No actions", EditorStyles.miniLabel);
                return;
            }

            foreach (var action in state.Actions)
            {
                var actionRect = new Rect(inner.x, actionsY, inner.width, ActionRowHeight);
                DrawActionRow(actionRect, action);
                actionsY += ActionRowHeight;
            }
        }

        private static void DrawActionRow(Rect rect, StateDebugActionModel action)
        {
            var label = $"{action.ExecTime} | {action.Priority} | {action.Name}";
            if (GUI.Button(rect, label, EditorStyles.miniButtonLeft) && action.Component != null)
            {
                Selection.activeObject = action.Component.gameObject;
                EditorGUIUtility.PingObject(action.Component.gameObject);
            }
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
            Repaint();
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
    }
}
