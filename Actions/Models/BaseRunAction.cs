using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Entities.Interfaces;
using Amaryllis.States.Models;
using UnityEngine;

namespace Amaryllis.Actions.Models
{
    public abstract class BaseRunAction : MonoBehaviour, IRunAction
    {
        [SerializeField] private int _execPriority = 0;
        [SerializeField] private bool _isEnable = true;
        [SerializeField] private int _startDelayMs = 0;
        [SerializeField] private ExecTimeType _execTimeType;
        
        [SerializeReference] private List<IRunActionCondition> _runActionConditions;

        public int ExecPriority => _execPriority;
        public ExecTimeType ExecTime => _execTimeType;

        public virtual async Task<bool> Run(IEntity entity)
        {
            if (!_isEnable) return false;
            if (!IsCanRun(entity)) return false;
            
            await Task.Delay(_startDelayMs);
            
            var result = await RunLogic(entity);
            
            return result;
        }

        protected abstract Task<bool> RunLogic(IEntity entity);

        private bool IsCanRun(IEntity entity)
        {
            return _runActionConditions.All(condition => condition.IsCanRun(entity));
        }
    }
}