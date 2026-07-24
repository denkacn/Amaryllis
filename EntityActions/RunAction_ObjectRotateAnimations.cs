using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_ObjectRotateAnimations : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField]
        private Vector3 _startRotation;
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private Vector3 _openRotation;
        [SerializeField]
        private bool _returnToStart;
        [SerializeField]
        private float _duration = 1f;
        [SerializeField]
        private float _backDuration = 2f;
        
        [SerializeField] 
        private float _delay = 0;
                      
        void Awake()
        {
            _startRotation = _target.localEulerAngles;
        }

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            if (_delay > 0)
            {
                await Task.Delay((int)(_delay * 1000));
            }
            
            var animator = _target.GetComponent<Animator>();
            if (animator != null) animator.enabled = false;

            _target.DOLocalRotate(_openRotation, _duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (_returnToStart)
                {
                    _target.DOLocalRotate(_startRotation, _backDuration).SetDelay(_duration + 2).OnComplete(() =>
                    {
                        if (animator != null) animator.enabled = true;
                    });
                }
                else
                {
                    if (animator != null) animator.enabled = true;
                }
            });
        
            await Task.Yield();
            return true;
        }
    }
}
