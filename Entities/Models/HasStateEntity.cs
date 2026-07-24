using Amaryllis.Entities.Interfaces;
using Amaryllis.States.Interfaces;

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

        public void Exec(IEntity entity)
        {
            _statesObject.Exec(entity);
        }

        private void InitStatesObject()
        {
            _statesObject = GetComponentInChildren<IStatesObject>();
            _statesObject.Init();
        }
    }
}