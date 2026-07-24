using Amaryllis.States.Models;
using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal sealed class StateDebugActionModel
    {
        public StateDebugActionModel(Component component, string name, ExecTimeType execTime, int priority)
        {
            Component = component;
            Name = name;
            ExecTime = execTime;
            Priority = priority;
        }

        public Component Component { get; }
        public string Name { get; }
        public ExecTimeType ExecTime { get; }
        public int Priority { get; }
    }
}
