using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal readonly struct DebugTimelineEntry
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
