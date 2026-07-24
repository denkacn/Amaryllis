using System.Collections.Generic;
using System.Linq;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;

namespace Amaryllis.Entities.Managers
{
    public static class EntitiesManager
    {
        private static readonly Dictionary<string, IEntity> _entitiesById = new Dictionary<string, IEntity>();

        public static int Count => _entitiesById.Count;
        public static IReadOnlyCollection<IEntity> Entities => _entitiesById.Values;

        public static bool Add(IEntity entity)
        {
            if (entity == null)
            {
                AmaryllisLog.Log("[EntitiesManager] Add skipped: entity is null");
                return false;
            }

            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                AmaryllisLog.Log("[EntitiesManager] Add skipped: entity id is empty");
                return false;
            }

            if (_entitiesById.ContainsKey(entity.Id))
            {
                AmaryllisLog.Log($"[EntitiesManager] Entity id {entity.Id} already exists. Replacing old entry.");
            }

            _entitiesById[entity.Id] = entity;
            return true;
        }

        public static bool Remove(string entityId)
        {
            if (string.IsNullOrWhiteSpace(entityId))
            {
                return false;
            }

            return _entitiesById.Remove(entityId);
        }

        public static IEntity Get(string entityId)
        {
            return TryGet(entityId, out var entity) ? entity : null;
        }
        
        public static IEntity Get()
        {
            return _entitiesById.Values.FirstOrDefault();
        }

        public static bool TryGet(string entityId, out IEntity entity)
        {
            entity = null;
            
            if (string.IsNullOrWhiteSpace(entityId))
            {
                return false;
            }

            return _entitiesById.TryGetValue(entityId, out entity);
        }

        public static void Clear()
        {
            _entitiesById.Clear();
        }
    }
}
