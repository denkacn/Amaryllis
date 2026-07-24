using System.Collections.Generic;
using Amaryllis.States.Models;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal sealed class StateDebugNodeModel
    {
        public StateDebugNodeModel(StateObjectBase state, IReadOnlyList<StateDebugActionModel> actions)
        {
            State = state;
            StateId = state.StateId;
            NextStateId = state.NextStateId;
            Name = state.name;
            Actions = actions;
        }

        public StateObjectBase State { get; }
        public int StateId { get; }
        public int NextStateId { get; }
        public string Name { get; }
        public IReadOnlyList<StateDebugActionModel> Actions { get; }
    }
}
