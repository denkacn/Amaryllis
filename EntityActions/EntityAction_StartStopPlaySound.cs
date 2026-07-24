using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
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
        
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            if (_isPlay)
            {
                _audioSource.Play();
                _audioSource.DOFade(_maxVolume, 2f);
            }
            else
            {
                _audioSource.DOFade(0, 2f).OnComplete(() =>
                {
                    _audioSource.Stop();
                });
            }

            await Task.Yield();
            return true;
        }
    }
}
