using System;
using Amaryllis.Entities.Interfaces;
using Amaryllis.Logs;
using UnityEngine;

namespace Amaryllis.Networks.Synchronizers.Photon
{
    public interface INetworkEntityTransport
    {
        bool IsWriteAuthority { get; }
        event Action<string> CreateReceived;
        void SendCreate(string entityId);
    }

    [RequireComponent(typeof(IEntity))]
    public class PunNetworkEntitySynchronizer : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _transportBehaviour;
        
        private IEntity _entity;
        private INetworkEntityTransport _transport;

        private void Awake()
        {
            _entity = GetComponent<IEntity>();
            _transport = _transportBehaviour as INetworkEntityTransport;
        }

        private void OnEnable()
        {
            if (_entity != null)
            {
                _entity.OnCreateHandler += OnLocalEntityCreate;
            }

            if (_transport != null)
            {
                _transport.CreateReceived += OnRemoteEntityCreate;
            }
        }

        private void OnDisable()
        {
            if (_entity != null)
            {
                _entity.OnCreateHandler -= OnLocalEntityCreate;
            }

            if (_transport != null)
            {
                _transport.CreateReceived -= OnRemoteEntityCreate;
            }
        }

        private void OnLocalEntityCreate(string entityId)
        {
            if (_transport == null || !_transport.IsWriteAuthority)
            {
                return;
            }
            
            AmaryllisLog.Log("[NetworkEntitySynchronizer] Local create: " + entityId);
            _transport.SendCreate(entityId);
        }

        private void OnRemoteEntityCreate(string entityId)
        {
            AmaryllisLog.Log("[NetworkEntitySynchronizer] Remote create: " + entityId);
            _entity?.Create(entityId);
        }
    }
}
