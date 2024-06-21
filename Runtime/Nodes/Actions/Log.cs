using UnityEngine;

namespace Shipico.BehaviourTrees
{
    public class Log : ActionNode
    {
        public string Message;
        
        protected override void OnExit(bool cancelled)
        {
            if (cancelled)
            {
                Debug.LogWarning("Cancelled");
            }
        }

        protected override Status OnUpdate(float deltaTime)
        {
            Debug.Log(Message);
            return Status.Success;
        }

        public override TreeNode Clone()
        {
            var clone = Instantiate(this);
            clone.Message = Message;
            return clone;
        }
    }
}