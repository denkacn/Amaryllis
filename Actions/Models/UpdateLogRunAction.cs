using System;
using System.Threading;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Amaryllis.Actions.Models
{
    public class UpdateLogRunAction : UpdatedRunAction
    {
        [SerializeField] private string _logText;
        
        protected override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            AmaryllisLog.Log($"[UpdateLogRunAction] {_logText}");

            return UniTask.FromResult(true);
        }

        public override void UpdateIt(float deltaTime)
        {
            AmaryllisLog.Log($"[UpdateLogRunAction] {DateTime.Now}");
        }
    }
}
