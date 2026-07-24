using System;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Entities.Managers;
using UnityEngine;

namespace Amaryllis.Entities.Models
{
    public class SimpleEntity : MonoBehaviour, IEntity
    {
        public event Action<string> OnCreateHandler;
        public event Action<string> OnInitHandler;
        public event Action<string> OnDiscardHandler;
        
        public string Id { get; private set; }

        public virtual void Create()
        {
            Add(Guid.NewGuid().ToString());
        }

        public virtual void Create(string entityId)
        {
            Add(entityId);
        }

        private void Add(string entityId)
        {
            if (!string.IsNullOrWhiteSpace(Id))
            {
                EntitiesManager.Remove(Id);
            }
            
            Id = entityId;
            
            OnCreateHandler?.Invoke(Id);
            
            EntitiesManager.Add(this);
        }

        public virtual void Init()
        {
            OnInitHandler?.Invoke(Id);
        }

        public virtual void Discard()
        {
            OnDiscardHandler?.Invoke(Id);
            
            EntitiesManager.Remove(Id);
        }
    }
}
