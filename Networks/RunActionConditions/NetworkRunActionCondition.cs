using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using UnityEngine;

namespace Amaryllis.Networks.RunActionConditions
{
    public class NetworkRunActionCondition : BaseRunActionCondition
    {
        [SerializeField] private NetworkRunActionConditionType _runActionConditionType;
        
        public override bool IsCanRun(IEntity entity)
        {
            switch (_runActionConditionType)
            {
                case NetworkRunActionConditionType.Both:
                    return true;
                case NetworkRunActionConditionType.Client:
                    return true;
                case NetworkRunActionConditionType.Master:
                    return true;
                case NetworkRunActionConditionType.None:
                    return false;
                default:
                    return false;
            }
        }
    }

    public enum NetworkRunActionConditionType
    {
        Both, Client, Master, None,
    }
}