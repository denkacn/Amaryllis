using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal sealed class StateDebugConditionModel
    {
        public StateDebugConditionModel(Object source, string name)
        {
            Source = source;
            Name = name;
        }

        public Object Source { get; }
        public string Name { get; }
    }
}
