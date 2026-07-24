using Amaryllis.Actions.Interfaces;
using Amaryllis.Entities.Interfaces;
using UnityEngine;

namespace Amaryllis.Actions.Models
{
    public class BaseStateCondition : MonoBehaviour, IStateCondition
    {
        [SerializeField] private bool _isCanExec;
        
        public bool IsCanExec(IEntity entity)
        {
            return _isCanExec;
        }
    }
}
