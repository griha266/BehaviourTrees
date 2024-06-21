namespace Shipico.BehaviourTrees
{
    public class EvaluationNode : ManyChildrenNode<StrategyNode>
    {
        private StrategyNode CalculateCurrentNode()
        {
            var maxEvaluation = 0f;
            StrategyNode resultNode = null;
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Evaluate().Value > maxEvaluation)
                {
                    resultNode = Children[i];
                }
            }

            return resultNode;
        }

        private StrategyNode _currentNode;

        protected override void OnEnter()
        {
            _currentNode = CalculateCurrentNode();
        }

        protected override void OnExit(bool cancelled)
        {
            if (cancelled)
            {
                _currentNode.Abort();
            }

            _currentNode = null;
        }

        protected override Status OnUpdate(float deltaTime)
        {
            return _currentNode.UpdateNode(Blackboard, Tree, deltaTime);
        }

        protected override ManyChildrenNode<StrategyNode> CloneInternal()
        {
            return Instantiate(this);
        }
    }
}