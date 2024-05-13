namespace Shipico.BehaviourTrees
{
    public static class BehaviourTreeUtils
    {
        public static int SortNodesByPosition(TreeNode left, TreeNode right) => left.GrahpPosition.x.CompareTo(right.GrahpPosition.x);
    }
}