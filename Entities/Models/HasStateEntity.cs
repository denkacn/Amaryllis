using Amaryllis.Entities.Interfaces;
using Amaryllis.States.Interfaces;
using Cysharp.Threading.Tasks;

namespace Amaryllis.Entities.Models
{
    public class HasStateEntity : SimpleEntity
    {
        private IStatesObject _statesObject;
        
        public override void Create()
        {
            InitStatesObject();
            
            base.Create();
        }

        public override void Create(string entityId)
        {
            InitStatesObject();
            
            base.Create(entityId);
        }

        public UniTask Exec(IEntity entity)
        {
            return _statesObject.Exec(entity);
        }

        private void InitStatesObject()
        {
            _statesObject = GetComponentInChildren<IStatesObject>();
            _statesObject.Init();
        }
    }
}
