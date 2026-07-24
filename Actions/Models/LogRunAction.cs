using System.Threading.Tasks;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using UnityEngine;

namespace Amaryllis.Actions.Models
{
    public class LogRunAction : BaseRunAction
    {
        [SerializeField] private string _logText;
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            await Task.Yield();

            AmaryllisLog.Log($"[LogRunAction] {_logText}");

            return true;
        }
    }
}