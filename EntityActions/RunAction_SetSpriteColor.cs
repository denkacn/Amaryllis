using System.Threading.Tasks;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.EntityActions
{
    public class RunAction_SetSpriteColor : BaseRunAction
    {
        [ShowInInspector, PropertySpace]
        
        [SerializeField]
        private Renderer _target;
        [SerializeField]
        private Renderer[] _targets;
        [SerializeField]
        private Color _toColor;
        [SerializeField] 
        private bool _setMain = true;
        [SerializeField] 
        private bool _setEmission = true;

        protected override async Task<bool> RunLogic(IEntity entity)
        {
            SetColor(_target, _toColor);

            foreach (var r in _targets)
            {
                SetColor(r, _toColor);
            }
        
            await Task.Yield();
            return true;
        }

        private void SetColor(Renderer r, Color c)
        {
            if (r != null)
            {
                if (_setMain)
                {
                    r.material.color = c;
                }

                if (_setEmission)
                {
                    r.material.SetColor("_EmissionColor", c);
                }
            }
        }
    }
}
