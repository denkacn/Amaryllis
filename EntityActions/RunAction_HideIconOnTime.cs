using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_HideIconOnTime : BaseRunAction
    {
        [ShowInInspector, PropertySpace] 
        [SerializeField]
        private float _hideTime;

        protected override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            AmaryllisLog.Log("!!! [RunAction_HideIconOnTime] Empty !!!");

            return UniTask.FromResult(true);
        }
    }
}
