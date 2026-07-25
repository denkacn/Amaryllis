#if UNITY_EDITOR
using System;
using System.Threading;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Entities.Models;
using Amaryllis.States.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Amaryllis.Debugging
{
    public static class AmaryllisDebugCommands
    {
        public static void MoveToState(StatesObjectBase statesObject, int stateId)
        {
            if (statesObject == null)
            {
                return;
            }

            statesObject.MoveToStateByIdAsync(stateId, statesObject.GetCancellationTokenOnDestroy())
                .Forget(Debug.LogException);
        }

        public static void ExecEntity(HasStateEntity entity)
        {
            if (entity == null)
            {
                return;
            }

            entity.Exec(null).Forget(Debug.LogException);
        }

        public static void RunAction(IRunAction action, IEntity entity, ExecTimeType execTime)
        {
            RunActionAsync(action, entity, execTime).Forget(Debug.LogException);
        }

        private static async UniTask RunActionAsync(IRunAction action, IEntity entity, ExecTimeType execTime)
        {
            if (action == null)
            {
                return;
            }

            AmaryllisDebugEvents.RaiseActionStarted(action, execTime);

            var component = action as Component;
            var cancellationToken = component == null
                ? CancellationToken.None
                : component.GetCancellationTokenOnDestroy();

            RunActionResult result;
            try
            {
                result = await action.Run(entity, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                result = RunActionResult.Canceled;
            }
            catch (Exception exception)
            {
                if (component == null)
                {
                    Debug.LogException(exception);
                }
                else
                {
                    Debug.LogException(exception, component);
                }

                result = RunActionResult.Failed;
            }

            AmaryllisDebugEvents.RaiseActionFinished(action, execTime, result);
        }
    }
}
#endif
