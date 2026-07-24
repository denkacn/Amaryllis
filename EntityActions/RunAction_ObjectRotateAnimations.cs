using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
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

        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (_delay > 0)
            {
                await UniTask.Delay((int)(_delay * 1000), cancellationToken: cancellationToken);
            }
            
            var animator = _target.GetComponent<Animator>();
            if (animator != null) animator.enabled = false;

            await AwaitTween(_target.DOLocalRotate(_openRotation, _duration)
                .SetEase(Ease.Linear), cancellationToken);

            if (_returnToStart)
            {
                await UniTask.Delay((int)((_duration + 2f) * 1000), cancellationToken: cancellationToken);
                await AwaitTween(_target.DOLocalRotate(_startRotation, _backDuration), cancellationToken);
            }

            if (animator != null) animator.enabled = true;
        
            return true;
        }

        private static async UniTask AwaitTween(Tween tween, CancellationToken cancellationToken)
        {
            var completion = new UniTaskCompletionSource();

            tween.OnComplete(() => completion.TrySetResult());
            tween.OnKill(() => completion.TrySetResult());

            using (cancellationToken.Register(() =>
                   {
                       if (tween.IsActive())
                       {
                           tween.Kill();
                       }
                   }))
            {
                await completion.Task;
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
