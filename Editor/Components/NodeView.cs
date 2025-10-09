using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shipico.BehaviourTrees.Editor
{
    public sealed class NodeView : Node
    {
        private bool _isVisited;
        
        public event Action<NodeView> OnNodeSelected;
        
        public TreeNode Node { get; }
        public Port Input { get; private set; }
        public Port Output { get; private set; }
        public List<Type> CompatibleInputTypes { get; private set; }
        public List<Type> CompatibleOutputTypes { get; private set; }

        public NodeView(TreeNode node) : base($"{BehaviourTreeEditorUtils.GetPackageRootPath()}/Editor/Resources/NodeView.uxml")
        {
            Node = node;
            title = Node.GetType().Name;
            viewDataKey = Node.id;

            style.left = Node.graphPosition.x;
            style.top = Node.graphPosition.y;
            
            CreateInputPorts();
            CreateOutputPorts();
            SetStyleClasses();
            SetCompatibleTypes();
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(Node, "BehaviourTree (Set Position)");
            Node.graphPosition.x = newPos.xMin;
            Node.graphPosition.y = newPos.yMin;
            EditorUtility.SetDirty(Node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        public void UpdateCurrentStatePreview()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            if (Application.isPlaying)
            {
                if (!_isVisited && Node.CurrentStatus == TreeNode.Status.Running)
                {
                    _isVisited = true;
                }
            }
            else
            {
                _isVisited = false;
            }
            string className = null;
            if (Node.CurrentStatus == TreeNode.Status.Running)
            {
                className = "running";
            }
            else if(_isVisited)
            {
                className = Node.CurrentStatus == TreeNode.Status.Failure ? "failure" : "success";
            }

            AddToClassList(className);
        }

        public void ClearVisitedStatus()
        {
            _isVisited = false;
        }
        
        // TODO this is basically a color, we could extract it to attribute also
        private void SetStyleClasses()
        {
            var className = Node switch
            {
                ActionNode => "action-node",
                ComposeNode => "compose-node",
                DecoratorNode => "decorator-node",
                EvaluationNode => "evaluation-node",
                RootNode => "root-node",
                StrategyNode => "strategy-node",
                TreeReferenceNode => "tree-reference-node",
                _ => "unknown-node"
            };
            
            AddToClassList(className);
        }

        // TODO extract this info into attributes on the nodes
        private void SetCompatibleTypes()
        {
            var currentType = Node.GetType();
            var allNodes = TypeCache.GetTypesDerivedFrom<TreeNode>().ToList();
            if (currentType == typeof(EvaluationNode))
            {
                CompatibleInputTypes = allNodes;
                CompatibleOutputTypes = TypeCache.GetTypesDerivedFrom<StrategyNode>().ToList();
            } else if (currentType.IsSubclassOf(typeof(StrategyNode)))
            {
                CompatibleInputTypes = new List<Type>() { typeof(EvaluationNode) };
                CompatibleOutputTypes = allNodes;
            }
            else
            {
                CompatibleOutputTypes = allNodes;
                CompatibleInputTypes = allNodes;
            }
        }

        private void CreateInputPorts()
        {
            // do not make input for root
            if (Node is RootNode) 
                return;
            
            // Anything else should have input
            Input = NodePort.Create(Direction.Input, Port.Capacity.Single);
            Input.portName = "";
            Input.style.flexDirection = FlexDirection.Column;
            Input.style.width = 100;
            inputContainer.Add(Input);
        }

        private void CreateOutputPorts()
        {
            Output = Node switch
            {
                IManyChildrenNode => NodePort.Create(Direction.Output, Port.Capacity.Multi),
                IOneChildNode => NodePort.Create(Direction.Output, Port.Capacity.Single),
                _ => null
            };

            if (Output == null) 
                return;
            
            Output.portName = "";
            Output.style.flexDirection = FlexDirection.ColumnReverse;
            Output.style.width = 100;
            outputContainer.Add(Output);
        }
    }
}