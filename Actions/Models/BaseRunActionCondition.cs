using Amaryllis.Actions.Interfaces;
using Amaryllis.Entities.Interfaces;

namespace Amaryllis.Actions.Models
{
    public class BaseRunActionCondition : IRunActionCondition
    {
        public virtual bool IsCanRun(IEntity entity)
        {
            return true;
        }
    }
}