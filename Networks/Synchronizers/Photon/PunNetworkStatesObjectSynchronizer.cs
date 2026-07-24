using Amaryllis.Entities.Managers;
using Amaryllis.Logs;
using Amaryllis.States.Interfaces;
using UnityEngine;

namespace Amaryllis.Networks.Synchronizers.Photon
{
    [RequireComponent(typeof(IStatesObject))]
    public class PunNetworkStatesObjectSynchronizer : MonoBehaviour
    {
        //[SerializeField] private PhotonView _photonView;
        
        private IStatesObject _statesObject;

        private void Awake()
        {
            _statesObject = GetComponentInChildren<IStatesObject>();
            _statesObject.OnInitHandler += () =>
            {
                /*if (PhotonNetwork.isMasterClient)
                {
                    if (_statesObject != null)
                    {
                        _statesObject.OnExecHandler += OnStatesObjectExec;
                        _statesObject.OnConditionFailHandler += OnStatesObjectConditionFail;
                    }
                }*/
            };
        }

        /*private void OnStatesObjectExec(string executorId)
        {
            if (_photonView != null)
            {
                AmaryllisLog.Log("[PunNetworkStatesObjectSynchronizer] OnStatesObjectExec: " + executorId);

                _photonView.RPC(nameof(StatesObjectExecRpc), PhotonTargets.OthersBuffered, executorId);
            }
        }*/

        /*[PunRPC]
        private void StatesObjectExecRpc(string executorId)
        {
            AmaryllisLog.Log("[PunNetworkStatesObjectSynchronizer] StatesObjectExecRpc: _statesObject = " + _statesObject);
            
            var entity = EntitiesManager.Get(executorId);
            _statesObject?.Exec(entity, false);
        }
        
        private void OnStatesObjectConditionFail(string executorId)
        {
            AmaryllisLog.Log("[PunNetworkStatesObjectSynchronizer] OnStatesObjectConditionFail: " + executorId);

            _photonView.RPC(nameof(StatesObjectConditionFail), PhotonTargets.OthersBuffered, executorId);
        }
        
        [PunRPC]
        private void StatesObjectConditionFail(string executorId)
        {
            AmaryllisLog.Log("[PunNetworkStatesObjectSynchronizer] StatesObjectExecRpc: _statesObject = " + _statesObject);
            
            var entity = EntitiesManager.Get(executorId);
            _statesObject?.ConditionFailAsync(entity);
        }*/
    }
}
