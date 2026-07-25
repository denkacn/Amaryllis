using Amaryllis.Actions.Interfaces;

namespace Amaryllis.Actions.Models
{
    public abstract class CompositeRunActionBase : BaseRunAction, ICompositeRunAction
    {
        public abstract System.Collections.Generic.IReadOnlyList<IRunAction> ChildActions { get; }
    }
}
