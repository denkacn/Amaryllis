using System.Threading.Tasks;
using Amaryllis.Entities.Interfaces;

namespace Amaryllis.States.Interfaces
{
    public interface IExecObject
    {
        Task Exec(IEntity entity, bool isCheckConditions = true);
    }
}
