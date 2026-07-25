using System.Collections.Generic;
using Amaryllis.States.Models;
using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal sealed class StateDebugActionModel
    {
        public StateDebugActionModel(
            Component component,
            string name,
            ExecTimeType execTime,
            int priority,
            bool isEnabled,
            IReadOnlyList<StateDebugConditionModel> conditions,
            IReadOnlyList<StateDebugActionModel> childActions)
        {
            Component = component;
            Name = name;
            ExecTime = execTime;
            Priority = priority;
            IsEnabled = isEnabled;
            Conditions = conditions;
            ChildActions = childActions;
        }

        public Component Component { get; }
        public string Name { get; }
        public ExecTimeType ExecTime { get; }
        public int Priority { get; }
        public bool IsEnabled { get; }
        public IReadOnlyList<StateDebugConditionModel> Conditions { get; }
        public IReadOnlyList<StateDebugActionModel> ChildActions { get; }
    }
}
