using System.Collections.Generic;

namespace Amaryllis.Actions.Interfaces
{
    public interface ICompositeRunAction : IRunAction
    {
        IReadOnlyList<IRunAction> ChildActions { get; }
    }
}
