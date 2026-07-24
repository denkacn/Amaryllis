using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using UnityEngine;

namespace Amaryllis.Networks.Synchronizers.Photon
{
    [RequireComponent(typeof(IEntity))]
    public class PunNetworkEntitySynchronizer : MonoBehaviour
    {
        //[SerializeField] private PhotonView _photonView;
        
        private IEntity _entity;

        private void Awake()
        {
            _entity = GetComponent<IEntity>();
            
            /*if (PhotonNetwork.isMasterClient)
            {
                if (_entity != null)
                {
                    _entity.OnCreateHandler += OnEntityCreateHandler;
                }
            }*/
        }

        private void OnEntityCreateHandler(string entityId)
        {
            AmaryllisLog.Log("[PunNetworkEntitySynchronizer] OnEntityCreateHandler: " + entityId);
            
            /*if (_photonView != null)
            {
                _photonView.RPC(nameof(EntityCreateHandlerRpc), PhotonTargets.OthersBuffered, entityId);
            }*/
        }

        /*[PunRPC]
        private void EntityCreateHandlerRpc(string entityId)
        {
            AmaryllisLog.Log("[PunNetworkEntitySynchronizer] EntityCreateHandlerRpc: " + entityId);
            _entity?.Create(entityId);
        }*/
    }
}
