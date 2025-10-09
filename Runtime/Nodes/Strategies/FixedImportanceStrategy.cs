using UnityEngine.Serialization;

namespace Shipico.BehaviourTrees
{
    public class FixedImportanceStrategy : StrategyNode
    {
        public NormalizedFloat importance;
        
        protected override void OnExit(bool cancelled)
        {
            if (cancelled)
            {
                Child.Abort();
            }
        }

        protected override Status OnUpdate(float deltaTime)
        {
            return Child.UpdateNode(Blackboard, Tree, deltaTime);
        }

        public override NormalizedFloat Evaluate()
        {
            return importance;
        }

        protected override StrategyNode CloneInternal()
        {
            var clone = Instantiate(this);
            clone.importance = importance;
            return clone;
        }
    }
}