namespace Shipico.BehaviourTrees
{
    public abstract class StrategyNode : OneChildNode<TreeNode>, ICloneable<StrategyNode>
    {
        public abstract NormalizedFloat Evaluate();

        StrategyNode ICloneable<StrategyNode>.Clone()
        {
            return CloneInternal();
        }

        public sealed override TreeNode Clone()
        {
            return CloneInternal();
        }

        protected abstract StrategyNode CloneInternal();
    }
}