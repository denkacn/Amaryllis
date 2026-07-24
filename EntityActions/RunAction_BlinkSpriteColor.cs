using System.Collections;
using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_BlinkSpriteColor : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField]
        private Renderer _target;
        [SerializeField]
        private Color _toColor;
        [SerializeField] 
        private bool _setMain = true;
        [SerializeField] 
        private bool _setEmission = true;
        [SerializeField] 
        private int _blinkCount = 1;
        [SerializeField] 
        private float _blinkTime = 1f;
        [SerializeField] 
        private int _waitDelay = 1000;
        
        private Color _startColor;
        private Color _startColorEmission;
        
        protected override async Task<bool> RunLogic(IEntity entity)
        {
            if (_target != null)
            {
                _startColor = _target.material.color;
                _startColorEmission = _target.material.GetColor("_EmissionColor");
                
                StartCoroutine(BlinkLogic());
                
                await Task.Delay(_waitDelay);
            }
        
            await Task.Yield();
            return true;
        }

        private IEnumerator BlinkLogic()
        {
            var count = 0;
            while (_blinkCount > count)
            {
                SetColor(_toColor,_toColor);
                yield return new WaitForSeconds(_blinkTime);
                
                SetColor(_startColor, _startColorEmission);
                yield return new WaitForSeconds(_blinkTime);
                
                count++;
            }   
        }

        private void SetColor(Color color, Color emission)
        {
            if (_setMain)
            {
                _target.material.color = color;
            }

            if (_setEmission)
            {
                _target.material.SetColor("_EmissionColor", emission);
            }    
        }
    }
}
