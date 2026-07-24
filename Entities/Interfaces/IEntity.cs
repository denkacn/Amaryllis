using System;
using UnityEngine;

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

    public interface ICharacterActionTarget
    {
        void SetEnableControl(bool isEnable);
        void LookAtPoint(Vector3 point, float lookTime);
        void SetAnimationTrigger(string triggerName, float lockTime);
    }
}
