using System.Collections.Generic;
using UnityEngine;

namespace Shipico.BehaviourTrees
{
    public interface IManyChildrenNode
    {
        IEnumerable<TreeNode> ChildrenAsBaseNode { get; }
        void AddChild(TreeNode node);
        void RemoveChild(TreeNode node);
        void Sort();
    }
    public abstract class ManyChildrenNode<TNodeType> : TreeNode, IManyChildrenNode
        where TNodeType : TreeNode
    {
        public IEnumerable<TreeNode> ChildrenAsBaseNode => Children;
        public void AddChild(TreeNode node)
        {
            var castedChild = node as TNodeType;
            if (castedChild)
            {
                Children.Add(castedChild);
            }
            else
            {
                Debug.LogWarning($"Cannot cast child {node.name} for adding to {name}");
            }
        }

        public void Sort()
        {
            Children.Sort(BehaviourTreeUtils.SortNodesByPosition);
        }

        public void RemoveChild(TreeNode node)
        {
            var castedChild = node as TNodeType;
            if (castedChild)
            {
                Children.Remove(castedChild);
            }
            else
            {
                Debug.LogWarning($"Cannot cast child {node.name} for adding to {name}");
            }
        }

        public List<TNodeType> Children = new();
    }
}