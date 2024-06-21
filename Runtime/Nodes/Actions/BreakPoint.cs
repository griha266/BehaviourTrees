using UnityEngine;

namespace Shipico.BehaviourTrees
{
    public class BreakPoint : ActionNode
    {
        protected override void OnExit(bool cancelled)
        {
        }

        protected override Status OnUpdate(float deltaTime)
        {
            Debug.LogWarning("Break point");
            Debug.Break();
            return Status.Success;
        }

        public override TreeNode Clone()
        {
            return Instantiate(this);
        }
    }
}