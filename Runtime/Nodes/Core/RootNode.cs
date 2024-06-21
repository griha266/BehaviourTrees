namespace Shipico.BehaviourTrees
{
    public class RootNode : OneChildNode<TreeNode>
    {
        public override TreeNode Clone()
        {
            return Instantiate(this);
        }
    }
}