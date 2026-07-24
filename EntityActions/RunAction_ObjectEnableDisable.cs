using System;
using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_ObjectEnableDisable : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField] 
        private bool _setState;
        [SerializeField] 
        private Transform _target;
        [SerializeField] 
        private float _delayTime = 0;

        [SerializeField] 
        private RunAction_ObjectEnableDisableItem[] _items;

        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (_delayTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_delayTime), cancellationToken: cancellationToken);
            }
            
            if (_target != null)
            {
                _target.gameObject.SetActive(_setState);
            }

            SetItemState();

            return true;
        }

        private void SetItemState()
        {
            foreach (var item in _items)
            {
                item.Set();
            }
        }
    }

    [Serializable]
    public class RunAction_ObjectEnableDisableItem
    {
        [SerializeField] 
        private GameObject _target;
        [SerializeField] 
        private bool _setState;

        public void Set()
        {
            _target.SetActive(_setState);
        }
    }
}
