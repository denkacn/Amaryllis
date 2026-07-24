using System.Threading.Tasks;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Entities.Interfaces;


namespace Amaryllis.Actions.Models
{
    public abstract class UpdatedRunAction : BaseRunAction, IUpdatedRunAction
    {
        protected abstract override Task<bool> RunLogic(IEntity entity);

        public abstract void UpdateIt(float deltaTime);
    }
}