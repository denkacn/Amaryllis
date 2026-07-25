using System.Collections.Generic;
using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal readonly struct GraphLayout
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
