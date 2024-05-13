using UnityEngine;

namespace Shipico.BehaviourTrees
{
    public interface IOneChildNode
    {
        public TreeNode ChildAsBase{ get; set; }
    }
    public abstract class OneChildNode<TNodeType> : TreeNode, IOneChildNode
        where TNodeType : TreeNode
    {
        public TreeNode ChildAsBase
        {
            get => Child;
            set
            {
                if (value == null)
                {
                    Child = null;
                    return;
                }
                var castedValue = value as TNodeType;
                if (castedValue)
                {
                    Child = castedValue;
                }
                else
                {
                    Debug.LogWarning($"Cannot cast {value}", value);
                }
            }
        }
        public TNodeType Child;

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
    }
}