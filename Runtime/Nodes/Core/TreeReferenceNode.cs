
namespace Shipico.BehaviourTrees
{
    public class TreeReferenceNode : TreeNode
    {
        public BehaviourTree ReferencedTree;

        protected override void OnEnter()
        {
            if (!ReferencedTree.IsInitialized)
            {
                ReferencedTree.Init(Tree, Blackboard);
            }
        }

        protected override void OnExit(bool cancelled)
        {
            if (cancelled)
            {
                ReferencedTree.Interrupt();
            }
        }

        protected override Status OnUpdate(float deltaTime)
        {
            return ReferencedTree.UpdateTree(deltaTime);
        }
    }
}