using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class EntityAction_StartStopPlaySound : BaseRunAction
    {
        [ShowInInspector, PropertySpace] 
        
        [SerializeField]
        private AudioSource _audioSource;

        [SerializeField] 
        private bool _isPlay;

        [SerializeField] 
        private float _maxVolume;

        private void Awake()
        {
            if (_audioSource != null)
            {
                _audioSource.volume = 0;
            }
        }
        
        protected override UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (_isPlay)
            {
                _audioSource.Play();
                FadeAudio(_maxVolume, 2f);
            }
            else
            {
                FadeAudio(0, 2f).OnComplete(() =>
                {
                    _audioSource.Stop();
                });
            }

            return UniTask.FromResult(true);
        }

        private Tween FadeAudio(float volume, float duration)
        {
            return DOTween.To(() => _audioSource.volume, value => _audioSource.volume = value, volume, duration)
                .SetTarget(_audioSource)
                .SetEase(Ease.Linear);
        }
    }
}
