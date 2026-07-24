using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
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

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            if (_delayTime > 0)
            {
                await Task.Delay((int)(_delayTime * 1000));
            }

            if (_isSetLoop)
            {
                _audioSource.loop = _isLoop;
            }

            _audioSource.volume = _startVolume;
            
            Play();

            Stop();

            Fade();

            await Task.Yield();
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

        private async void Stop()
        {
            if (_stopTime > 0)
            {
                await Task.Delay((int)(_stopTime * 1000));
                
                _audioSource.Stop();
            }
        }
        
        private void Fade()
        {
            if (_fadeTime > 0)
            {
                _audioSource.DOFade(0, _fadeTime).SetEase(Ease.Linear);
            }
        }
    }
}
