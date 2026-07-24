using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_LightBlink : BaseRunAction
    {
        [ShowInInspector, PropertySpace] 
        
        [SerializeField]
        private Light[] _lights;
        
        [SerializeField] 
        private float _delayTime = 0;
        
        [SerializeField]
        private Renderer[] _renderers;
        
        [SerializeField] 
        private float _blinkTime = 1f;

        [SerializeField] 
        private bool _isEnableInEnd = false;

        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (_delayTime > 0)
            {
                await UniTask.Delay((int)(_delayTime * 1000), cancellationToken: cancellationToken);
            }

            var endTime = Time.time + _blinkTime;
            var isEnable = true;

            while (endTime > Time.time)
            {
                await UniTask.Delay(Random.Range(30, 100), cancellationToken: cancellationToken);

                isEnable = !isEnable;

                SetLight(isEnable);
            }
            
            SetLight(_isEnableInEnd);

            return true;
        }

        private void SetLight(bool isEnable)
        {
            foreach (var light in _lights)
            {
                light.enabled = isEnable;
            }

            foreach (var renderer in _renderers)
            {
                if(isEnable)
                    renderer.material.EnableKeyword("_EMISSION");
                else
                    renderer.material.DisableKeyword("_EMISSION");
            }
        }
    }
}
