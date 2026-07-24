using System;

namespace Amaryllis.Entities.Interfaces
{
    public interface IEntity
    {
        event Action<string> OnCreateHandler;
        event Action<string> OnInitHandler;
        event Action<string> OnDiscardHandler;
        
        string Id { get; }
        void Create();
        void Create(string entityId);
        void Init();
        void Discard();
    }
}