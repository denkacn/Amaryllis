using System;
using System.Threading.Tasks;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using UnityEngine;

namespace Amaryllis.Actions.Models
{
    public class UpdateLogRunAction : UpdatedRunAction
    {
        [SerializeField] private string _logText;
        
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            await Task.Yield();
            
            AmaryllisLog.Log($"[UpdateLogRunAction] {_logText}");

            return true;
        }

        public override void UpdateIt(float deltaTime)
        {
            AmaryllisLog.Log($"[UpdateLogRunAction] {DateTime.Now}");
        }
    }
}