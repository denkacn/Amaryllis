using Amaryllis.Entities.Interfaces;

namespace Amaryllis.Actions.Interfaces
{
    public interface IStateCondition
    {
        bool IsCanExec(IEntity entity);
    }
}