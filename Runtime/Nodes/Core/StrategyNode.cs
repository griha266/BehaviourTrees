namespace Shipico.BehaviourTrees
{
    public abstract class StrategyNode : OneChildNode<TreeNode>
    {
        public abstract NormalizedFloat Evaluate();
    }
}