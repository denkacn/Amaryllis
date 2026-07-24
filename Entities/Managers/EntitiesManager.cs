using System.Collections.Generic;
using Amaryllis.Entities.Interfaces;

namespace Amaryllis.Entities.Managers
{
    public static class EntitiesManager
    {
        private static readonly List<IEntity> _entities = new List<IEntity>();

        public static void Add(IEntity entity)
        {
            _entities.Add(entity);
        }

        public static void Remove(string entityId)
        {
            _entities.RemoveAll(e => e.Id == entityId);
        }

        public static IEntity Get(string entityId)
        {
            return _entities.Find(e => e.Id == entityId);
        }
        
        public static IEntity Get()
        {
            return _entities[0];
        }
    }
}