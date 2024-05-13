namespace Shipico.BehaviourTrees
{
    public class FixedImportanceStrategy : StrategyNode
    {
        public NormalizedFloat Importance;
        
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
            return Importance;
        }
    }
}