using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Amaryllis.Editor.StateDebugWindow.StateDebugViewSettings;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal static class StateDebugEdgeRenderer
    {
        public static void DrawGrid(Rect contentRect)
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

        public static void DrawEdges(StateDebugGraphModel graph, GraphLayout layout)
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

                DrawArrow(layout.StateRects[state], layout.StateRects[nextState], layout.StateRects.Values);
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

        private static void DrawArrow(Rect from, Rect to, IEnumerable<Rect> stateRects)
        {
            var points = BuildArrowPath(from, to, stateRects);
            Handles.DrawAAPolyLine(3f, points);

            var tip = (Vector2)points[points.Length - 1];
            var direction = ((Vector2)points[points.Length - 1] - (Vector2)points[points.Length - 2]).normalized;
            if (direction == Vector2.zero)
            {
                return;
            }

            var normal = new Vector2(-direction.y, direction.x);
            var left = tip - direction * 11f + normal * 5f;
            var right = tip - direction * 11f - normal * 5f;
            Handles.DrawAAConvexPolygon(tip, left, right);
        }

        private static Vector3[] BuildArrowPath(Rect from, Rect to, IEnumerable<Rect> stateRects)
        {
            var start = new Vector3(from.xMax, from.center.y);
            var isForward = to.xMin > from.xMin;
            var distanceInStates = Mathf.Max(1, Mathf.RoundToInt(Mathf.Abs(to.xMin - from.xMin) / (StateNodeWidth + StateHorizontalSpacing)));
            var exitX = from.xMax + StateHorizontalSpacing * 0.5f;
            var enterX = to.xMin - StateHorizontalSpacing * 0.5f;

            if (isForward && distanceInStates == 1)
            {
                var centerEnd = new Vector3(to.xMin, to.center.y);
                if (Mathf.Abs(start.y - centerEnd.y) < 1f)
                {
                    return new[] { start, centerEnd };
                }

                return new[]
                {
                    start,
                    new Vector3(exitX, start.y),
                    new Vector3(exitX, centerEnd.y),
                    centerEnd
                };
            }

            var routeOffset = EdgeRoutePadding + (distanceInStates - 1) * EdgeRouteStep;
            var left = Mathf.Min(from.xMin, to.xMin);
            var right = Mathf.Max(from.xMax, to.xMax);
            var spannedRects = stateRects.Where(rect => rect.xMax >= left && rect.xMin <= right);
            var routeY = isForward
                ? spannedRects.Max(rect => rect.yMax) + routeOffset
                : spannedRects.Min(rect => rect.yMin) - routeOffset;
            var entryY = GetStateEdgePointY(to, isForward ? 4 : 2);
            var end = new Vector3(to.xMin, entryY);

            return new[]
            {
                start,
                new Vector3(exitX, start.y),
                new Vector3(exitX, routeY),
                new Vector3(enterX, routeY),
                new Vector3(enterX, entryY),
                end
            };
        }

        private static float GetStateEdgePointY(Rect rect, int point)
        {
            return rect.yMin + rect.height * Mathf.Clamp01((point - 1) / 4f);
        }
    }
}
