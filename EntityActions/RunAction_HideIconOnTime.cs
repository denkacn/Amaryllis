using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_HideIconOnTime : BaseRunAction
    {
        [ShowInInspector, PropertySpace] 
        [SerializeField]
        private float _hideTime;

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            AmaryllisLog.Log("!!! [RunAction_HideIconOnTime] Empty !!!");

            await Task.Yield();
            return true;
        }
    }
}
