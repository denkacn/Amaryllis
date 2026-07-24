using System;
using System.Collections;
using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
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

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            if (_delayTime == 0)
            {
                if (_target != null)
                {
                    _target.gameObject.SetActive(_setState);
                }

                SetItemState();
            }
            else
            {
                StartCoroutine(WaitAndDo());
            }

            await Task.Yield();
            return true;
        }

        private IEnumerator WaitAndDo()
        {
            yield return new WaitForSeconds(_delayTime);
            
            if (_target != null)
            {
                _target.gameObject.SetActive(_setState);
            }
            
            SetItemState();
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
