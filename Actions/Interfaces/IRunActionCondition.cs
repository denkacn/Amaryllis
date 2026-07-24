using Amaryllis.Entities.Interfaces;

namespace Amaryllis.Actions.Interfaces
{
    public interface IRunActionCondition
    {
        bool IsCanRun(IEntity entity);
    }
}