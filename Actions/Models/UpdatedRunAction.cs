using System.Threading;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;


namespace Amaryllis.Actions.Models
{
    public abstract class UpdatedRunAction : BaseRunAction, IUpdatedRunAction
    {
        protected abstract override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken);

        public abstract void UpdateIt(float deltaTime);
    }
}
