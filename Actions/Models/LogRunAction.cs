using System.Threading;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Amaryllis.Actions.Models
{
    public class LogRunAction : BaseRunAction
    {
        [SerializeField] private string _logText;
        protected override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            AmaryllisLog.Log($"[LogRunAction] {_logText}");

            return UniTask.FromResult(true);
        }
    }
}
