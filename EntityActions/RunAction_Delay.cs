using System;
using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_Delay : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField] 
        private float _delayTime = 0;
        
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            await Task.Delay(TimeSpan.FromSeconds(_delayTime));
            
            return true;
        }
    }
}
