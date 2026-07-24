using UnityEngine;

namespace Amaryllis.Entities.Models
{
    public class CharacterBaseEntity : SimpleEntity
    {
        public virtual void SetEnableControl(bool isEnable){}

        public virtual void LookAtPoint(Vector3 point, float lookTime){}

        public virtual void SetAnimationTrigger(string triggerName, float lockTime){}
    }
}
