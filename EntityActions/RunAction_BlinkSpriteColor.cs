using System.Threading;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Cysharp.Threading.Tasks;
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
        
        protected override async UniTask<bool> RunLogic(IEntity entity, CancellationToken cancellationToken)
        {
            if (_target != null)
            {
                _startColor = _target.material.color;
                _startColorEmission = _target.material.GetColor("_EmissionColor");
                
                await BlinkLogic(cancellationToken);
                
                if (_waitDelay > 0)
                {
                    await UniTask.Delay(_waitDelay, cancellationToken: cancellationToken);
                }
            }
        
            return true;
        }

        private async UniTask BlinkLogic(CancellationToken cancellationToken)
        {
            var count = 0;
            while (_blinkCount > count)
            {
                SetColor(_toColor,_toColor);
                await UniTask.Delay((int)(_blinkTime * 1000), cancellationToken: cancellationToken);
                
                SetColor(_startColor, _startColorEmission);
                await UniTask.Delay((int)(_blinkTime * 1000), cancellationToken: cancellationToken);
                
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
