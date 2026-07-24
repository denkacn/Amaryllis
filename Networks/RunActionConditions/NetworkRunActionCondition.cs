using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using UnityEngine;

namespace Amaryllis.Networks.RunActionConditions
{
    public interface INetworkAuthorityProvider
    {
        bool IsConnected { get; }
        NetworkAuthorityRole Role { get; }
    }

    public static class NetworkAuthorityProviderRegistry
    {
        private static readonly INetworkAuthorityProvider _offlineProvider = new OfflineNetworkAuthorityProvider();
        
        public static INetworkAuthorityProvider Current { get; private set; } = _offlineProvider;

        public static void Set(INetworkAuthorityProvider provider)
        {
            Current = provider ?? _offlineProvider;
        }

        public static void Reset()
        {
            Current = _offlineProvider;
        }
    }

    public class NetworkRunActionCondition : BaseRunActionCondition
    {
        [SerializeField] private NetworkRunActionConditionType _runActionConditionType;
        
        public override bool IsCanRun(IEntity entity)
        {
            var provider = NetworkAuthorityProviderRegistry.Current;
            
            switch (_runActionConditionType)
            {
                case NetworkRunActionConditionType.Both:
                    return true;
                case NetworkRunActionConditionType.Client:
                    return provider.Role == NetworkAuthorityRole.Client;
                case NetworkRunActionConditionType.Master:
                    return provider.Role == NetworkAuthorityRole.Master || provider.Role == NetworkAuthorityRole.Offline;
                case NetworkRunActionConditionType.None:
                    return false;
                default:
                    return false;
            }
        }
    }

    public enum NetworkRunActionConditionType
    {
        Both, Client, Master, None,
    }

    public enum NetworkAuthorityRole
    {
        Offline,
        Client,
        Master,
    }

    internal class OfflineNetworkAuthorityProvider : INetworkAuthorityProvider
    {
        public bool IsConnected => false;
        public NetworkAuthorityRole Role => NetworkAuthorityRole.Offline;
    }
}
