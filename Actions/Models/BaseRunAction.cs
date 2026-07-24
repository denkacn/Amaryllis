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
    public enum RunActionResult
    {
        Success,
        Skipped,
        Failed,
        Canceled
    }

    public abstract class BaseRunAction : MonoBehaviour, IRunAction
    {
        [SerializeField] private int _execPriority = 0;
        [SerializeField] private bool _isEnable = true;
        [SerializeField] private int _startDelayMs = 0;
        [SerializeField] private ExecTimeType _execTimeType;
        
        [SerializeReference] private List<IRunActionCondition> _runActionConditions;

        public int ExecPriority => _execPriority;
        public ExecTimeType ExecTime => _execTimeType;

        public virtual async UniTask<RunActionResult> Run(IEntity entity, CancellationToken cancellationToken = default)
        {
            if (!_isEnable) return RunActionResult.Skipped;
            if (!IsCanRun(entity)) return RunActionResult.Skipped;

            using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.GetCancellationTokenOnDestroy());

            try
            {
                if (_startDelayMs > 0)
                {
                    await UniTask.Delay(_startDelayMs, cancellationToken: linkedCancellation.Token);
                }
                
                var isSuccess = await RunLogic(entity, linkedCancellation.Token);
                return isSuccess ? RunActionResult.Success : RunActionResult.Failed;
            }
            catch (OperationCanceledException)
            {
                return RunActionResult.Canceled;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                return RunActionResult.Failed;
            }
        }

        protected abstract UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken);

        private bool IsCanRun(IEntity entity)
        {
            return _runActionConditions == null || _runActionConditions.All(condition => condition.IsCanRun(entity));
        }
    }
}
