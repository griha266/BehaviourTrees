using System.Collections.Generic;
using UnityEngine;

namespace Shipico.BehaviourTrees
{
    public class ParallelNode : ComposeNode
    {
        private readonly HashSet<TreeNode> _finishedNodes = new ();

        protected override void OnEnter()
        {
            _finishedNodes.Clear();
        }

        protected override void OnExit(bool cancelled)
        {
            if (cancelled)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    var node = Children[i];
                    if (node.CurrentStatus == Status.Running)
                    {
                        Children[i].Abort();
                    }
                }
            }
        }

        protected override Status OnUpdate(float deltaTime)
        {
            var allNodesCount = Children.Count;
            for (var i = 0; i < allNodesCount; i++)
            {
                if (_finishedNodes.Contains(Children[i]))
                {
                    continue;
                }
                var nodeResult = Children[i].UpdateNode(Blackboard, Tree, deltaTime);
                if (nodeResult != Status.Running)
                {
                    _finishedNodes.Add(Children[i]);
                }
            }

            return _finishedNodes.Count == allNodesCount ? Status.Success : Status.Running;
        }
    }
}