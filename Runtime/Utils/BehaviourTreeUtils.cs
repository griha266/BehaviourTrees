namespace Shipico.BehaviourTrees
{
    public static class BehaviourTreeUtils
    {
        public static int SortNodesByPosition(TreeNode left, TreeNode right) => left.graphPosition.x.CompareTo(right.graphPosition.x);
    }
}