using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_PlaySound : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField]
        private AudioSource _audioSource;
        [SerializeField] 
        private float _delayTime = 0;
        [SerializeField] 
        private AudioClip _clip;
        [SerializeField] 
        private bool _isBackwards = false;
        [SerializeField] 
        private float _stopTime = 0;
        [SerializeField] 
        private float _fadeTime = 0;

        [SerializeField] private bool _isSetLoop = false;
        [SerializeField] private bool _isLoop = false;

        private float _startVolume = 0;
        
        private void Awake()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }

            _startVolume = _audioSource.volume;
        }

        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (_delayTime > 0)
            {
                await UniTask.Delay((int)(_delayTime * 1000), cancellationToken: cancellationToken);
            }

            if (_isSetLoop)
            {
                _audioSource.loop = _isLoop;
            }

            _audioSource.volume = _startVolume;
            
            Play();

            StopAsync(cancellationToken).Forget();

            Fade();

            return true;
        }

        private void Play()
        {
            if (_clip != null)
            {
                _audioSource.clip = _clip;
            }

            if (_isBackwards)
            {
                _audioSource.timeSamples = _audioSource.clip.samples - 1;
                _audioSource.pitch = -1;
            }
            
            _audioSource.Play();
        }

        private async UniTask StopAsync(CancellationToken cancellationToken)
        {
            if (_stopTime > 0)
            {
                await UniTask.Delay((int)(_stopTime * 1000), cancellationToken: cancellationToken);
                
                _audioSource.Stop();
            }
        }
        
        private void Fade()
        {
            if (_fadeTime > 0)
            {
                DOTween.To(() => _audioSource.volume, value => _audioSource.volume = value, 0, _fadeTime)
                    .SetTarget(_audioSource)
                    .SetEase(Ease.Linear);
            }
        }
    }
}
