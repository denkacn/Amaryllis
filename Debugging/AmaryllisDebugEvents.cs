#if UNITY_EDITOR
using System;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Actions.Models;
using Amaryllis.States.Models;
using UnityEngine;

namespace Amaryllis.Debugging
{
    public static class AmaryllisDebugEvents
    {
        public static event Action<StateChangedDebugEvent> StateChanged;
        public static event Action<ActionDebugEvent> ActionStarted;
        public static event Action<ActionDebugEvent> ActionFinished;

        public static void RaiseStateChanged(StatesObjectBase statesObject, int previousStateId, int currentStateId)
        {
            StateChanged?.Invoke(new StateChangedDebugEvent(statesObject, previousStateId, currentStateId, Time.realtimeSinceStartupAsDouble));
        }

        public static void RaiseActionStarted(IRunAction action, ExecTimeType execTime)
        {
            ActionStarted?.Invoke(new ActionDebugEvent(action, execTime, null, Time.realtimeSinceStartupAsDouble));
        }

        public static void RaiseActionFinished(IRunAction action, ExecTimeType execTime, RunActionResult result)
        {
            ActionFinished?.Invoke(new ActionDebugEvent(action, execTime, result, Time.realtimeSinceStartupAsDouble));
        }
    }

    public readonly struct StateChangedDebugEvent
    {
        public StateChangedDebugEvent(StatesObjectBase statesObject, int previousStateId, int currentStateId, double time)
        {
            StatesObject = statesObject;
            PreviousStateId = previousStateId;
            CurrentStateId = currentStateId;
            Time = time;
        }

        public StatesObjectBase StatesObject { get; }
        public int PreviousStateId { get; }
        public int CurrentStateId { get; }
        public double Time { get; }
    }

    public readonly struct ActionDebugEvent
    {
        public ActionDebugEvent(IRunAction action, ExecTimeType execTime, RunActionResult? result, double time)
        {
            Action = action;
            ActionComponent = action as Component;
            ExecTime = execTime;
            Result = result;
            Time = time;
        }

        public IRunAction Action { get; }
        public Component ActionComponent { get; }
        public ExecTimeType ExecTime { get; }
        public RunActionResult? Result { get; }
        public double Time { get; }
    }
}
#endif
