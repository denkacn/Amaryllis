using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
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

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            if (_delayTime > 0)
            {
                await Task.Delay((int)(_delayTime * 1000));
            }

            var endTime = Time.time + _blinkTime;
            var isEnable = true;

            while (endTime > Time.time)
            {
                await Task.Delay(Random.Range(30, 100));

                isEnable = !isEnable;

                SetLight(isEnable);
            }
            
            SetLight(_isEnableInEnd);

            await Task.Yield();
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
