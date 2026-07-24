using System.Threading.Tasks;
using Amaryllis.Entities.Interfaces;

namespace Amaryllis.States.Interfaces
{
    public interface IStateObject
    {
        public int StateId { get; }
        public int NextStateId { get; }
        Task PreInitAsync();
        Task InitAsync();
        Task<bool> ExecAsync(IEntity entity);
        Task DiscardAsync();
        void PostDiscard();
        bool IsReadyForExec(IEntity entity);
        Task RunConditionFailActions(IEntity entity);
    }
}