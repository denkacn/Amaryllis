using System.Collections;
using Amaryllis.Entities.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Amaryllis.Utils
{
    [RequireComponent(typeof(HasStateEntity))]
    public class TestEntityController : MonoBehaviour
    {
        private HasStateEntity _hasStateEntity;
        
        private void Start()
        {
            _hasStateEntity = GetComponent<HasStateEntity>();

            //StartCoroutine(IeRunTestScenario());
        }

        private IEnumerator IeRunTestScenario()
        {
            yield return new WaitForSeconds(1);
            
            _hasStateEntity.Exec(null);
            
            yield return new WaitForSeconds(1);
            
            _hasStateEntity.Exec(null);
        }

        [Button]
        private void MoveToNextState()
        {
            _hasStateEntity.Exec(null);
        }
    }
}
