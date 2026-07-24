using System.Collections;
using Amaryllis.Entities.Models;
using Cysharp.Threading.Tasks;
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
            
            _hasStateEntity.Exec(null).Forget(Debug.LogException);
            
            yield return new WaitForSeconds(1);
            
            _hasStateEntity.Exec(null).Forget(Debug.LogException);
        }

        [Button]
        private void MoveToNextState()
        {
            _hasStateEntity.Exec(null).Forget(Debug.LogException);
        }
    }
}
