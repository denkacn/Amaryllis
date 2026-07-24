using System;
using UnityEngine;

namespace Amaryllis.Entities.Managers
{
    public class SceneEntitiesManager : MonoBehaviour
    {
        public static event Action SceneEntitiesManagerInitHandler;
        public static bool IsInit;

        [SerializeField] private bool _isInitOnPlay = false;

        private void Awake()
        {
            if (_isInitOnPlay)
            {
                Init();
            }
        }

        public static void Init()
        {
            IsInit = true;
            
            SceneEntitiesManagerInitHandler?.Invoke();
        }
    }
}