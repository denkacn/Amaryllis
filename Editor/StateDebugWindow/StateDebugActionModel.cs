using System.Collections.Generic;
using Amaryllis.States.Models;
using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal sealed class StateDebugActionModel
    {
        public StateDebugActionModel(Component component, string name, ExecTimeType execTime, int priority, IReadOnlyList<StateDebugConditionModel> conditions)
        {
            Component = component;
            Name = name;
            ExecTime = execTime;
            Priority = priority;
            Conditions = conditions;
        }

        public Component Component { get; }
        public string Name { get; }
        public ExecTimeType ExecTime { get; }
        public int Priority { get; }
        public IReadOnlyList<StateDebugConditionModel> Conditions { get; }
    }
}
