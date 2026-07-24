using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Entities.Interfaces;
using Amaryllis.States.Models;
using Cysharp.Threading.Tasks;
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

        public virtual async UniTask<bool> Run(IEntity entity, CancellationToken cancellationToken = default)
        {
            if (!_isEnable) return true;
            if (!IsCanRun(entity)) return false;

            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.GetCancellationTokenOnDestroy());

            try
            {
                if (_startDelayMs > 0)
                {
                    await UniTask.Delay(_startDelayMs, cancellationToken: linkedCancellation.Token);
                }
                
                return await RunLogic(entity, linkedCancellation.Token);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        protected abstract UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken);

        private bool IsCanRun(IEntity entity)
        {
            return _runActionConditions == null || _runActionConditions.All(condition => condition.IsCanRun(entity));
        }
    }
}
