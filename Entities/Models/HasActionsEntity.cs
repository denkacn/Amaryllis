using System.Collections.Generic;
using System.Threading;
using Amaryllis.Actions.Helpers;
using Amaryllis.Actions.Interfaces;
using Amaryllis.Actions.Models;
using Amaryllis.Entities.Interfaces;
using Amaryllis.States.Models;
using Cysharp.Threading.Tasks;

namespace Amaryllis.Entities.Models
{
    public class HasActionsEntity : SimpleEntity
    {
        private List<IRunAction> _actions;

        public override void Create()
        {
            InitActions();

            base.Create();
        }

        public override void Create(string entityId)
        {
            InitActions();

            base.Create(entityId);
        }

        public UniTask<bool> RunExecActions(IEntity entity, CancellationToken cancellationToken = default)
        {
            return RunActionLogicHelper.RunActionsAsync(ExecTimeType.Exec, entity, _actions, cancellationToken);
        }

        public UniTask<bool> RunActionsByType(ExecTimeType execTimeType, IEntity entity, CancellationToken cancellationToken = default)
        {
            return RunActionLogicHelper.RunActionsAsync(execTimeType, entity, _actions, cancellationToken);
        }

        private void InitActions()
        {
            _actions = CompositeRunActionUtility.GetRootActions(GetComponentsInChildren<IRunAction>(true));
        }
    }
}
