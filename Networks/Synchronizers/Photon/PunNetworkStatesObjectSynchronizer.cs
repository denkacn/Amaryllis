using System;
using Amaryllis.Entities.Managers;
using Amaryllis.Logs;
using Amaryllis.States.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Amaryllis.Networks.Synchronizers.Photon
{
    public interface INetworkStatesObjectTransport
    {
        bool IsWriteAuthority { get; }
        event Action<string> ExecReceived;
        event Action<string> ConditionFailReceived;
        void SendExec(string executorId);
        void SendConditionFail(string executorId);
    }

    [RequireComponent(typeof(IStatesObject))]
    public class PunNetworkStatesObjectSynchronizer : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _transportBehaviour;
        
        private IStatesObject _statesObject;
        private INetworkStatesObjectTransport _transport;

        private void Awake()
        {
            _statesObject = GetComponentInChildren<IStatesObject>();
            _transport = _transportBehaviour as INetworkStatesObjectTransport;
        }

        private void OnEnable()
        {
            if (_statesObject != null)
            {
                _statesObject.OnInitHandler += SubscribeLocalStateEvents;
            }

            if (_transport != null)
            {
                _transport.ExecReceived += OnRemoteExec;
                _transport.ConditionFailReceived += OnRemoteConditionFail;
            }
        }

        private void OnDisable()
        {
            if (_statesObject != null)
            {
                _statesObject.OnInitHandler -= SubscribeLocalStateEvents;
                _statesObject.OnExecHandler -= OnLocalExec;
                _statesObject.OnConditionFailHandler -= OnLocalConditionFail;
            }

            if (_transport != null)
            {
                _transport.ExecReceived -= OnRemoteExec;
                _transport.ConditionFailReceived -= OnRemoteConditionFail;
            }
        }

        private void SubscribeLocalStateEvents()
        {
            if (_statesObject == null || _transport == null || !_transport.IsWriteAuthority)
            {
                return;
            }

            _statesObject.OnExecHandler -= OnLocalExec;
            _statesObject.OnConditionFailHandler -= OnLocalConditionFail;
            _statesObject.OnExecHandler += OnLocalExec;
            _statesObject.OnConditionFailHandler += OnLocalConditionFail;
        }

        private void OnLocalExec(string executorId)
        {
            AmaryllisLog.Log("[NetworkStatesObjectSynchronizer] Local exec: " + executorId);
            _transport?.SendExec(executorId);
        }

        private void OnLocalConditionFail(string executorId)
        {
            AmaryllisLog.Log("[NetworkStatesObjectSynchronizer] Local condition fail: " + executorId);
            _transport?.SendConditionFail(executorId);
        }

        private void OnRemoteExec(string executorId)
        {
            AmaryllisLog.Log("[NetworkStatesObjectSynchronizer] Remote exec: " + executorId);
            
            var entity = EntitiesManager.Get(executorId);
            _statesObject?.Exec(entity, false, this.GetCancellationTokenOnDestroy()).Forget();
        }
        
        private void OnRemoteConditionFail(string executorId)
        {
            AmaryllisLog.Log("[NetworkStatesObjectSynchronizer] Remote condition fail: " + executorId);
            
            var entity = EntitiesManager.Get(executorId);
            _statesObject?.ConditionFailAsync(entity, this.GetCancellationTokenOnDestroy()).Forget();
        }
    }
}
