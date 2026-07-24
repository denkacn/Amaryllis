using System.Collections.Generic;
using Amaryllis.Entities.Models;
using Amaryllis.States.Models;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal sealed class StateDebugGraphModel
    {
        public StateDebugGraphModel(HasStateEntity entity, StatesObjectBase statesObject, IReadOnlyList<StateDebugNodeModel> states)
        {
            Entity = entity;
            StatesObject = statesObject;
            States = states;
        }

        public HasStateEntity Entity { get; }
        public StatesObjectBase StatesObject { get; }
        public IReadOnlyList<StateDebugNodeModel> States { get; }
    }
}
