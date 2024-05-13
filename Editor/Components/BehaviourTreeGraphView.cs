using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace Shipico.BehaviourTrees.Editor
{
    public class BehaviourTreeGraphView : GraphView
    {
        public event Action<NodeView> OnNodeSelected;
        private BehaviourTree _tree;

        public new class UxmlFactory : UxmlFactory<BehaviourTreeGraphView, UxmlTraits> { }

        public BehaviourTreeGraphView()
        {
            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var rootFolder = BehaviourTreeEditorUtils.GetPackageRootPath();
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{rootFolder}Editor/Resources/BehaviourTreeGraphView.uss");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var nodePosition = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            BuildNodeCategoryForContextMenu<ActionNode>("Actions", evt, nodePosition);
            BuildNodeCategoryForContextMenu<ComposeNode>("Composes", evt, nodePosition);
            BuildNodeCategoryForContextMenu<EvaluationNode>("Evaluation", evt, nodePosition);
            BuildNodeCategoryForContextMenu<StrategyNode>("Strategies", evt, nodePosition);
            BuildNodeCategoryForContextMenu<DecoratorNode>("Decorators", evt, nodePosition);
            BuildNodeCategoryForContextMenu<TreeReferenceNode>("References", evt, nodePosition);
        }
        
        public void ClearVisitingStatus()
        {
            foreach (var node in nodes)
            {
                if (node is NodeView nodeView)
                {
                    nodeView.ClearVisitedStatus();
                }
            }
        }
        
        public void UpdateNodeRuntimeState()
        {
            foreach (var node in nodes)
            {
                if (node is NodeView nodeView)
                {
                    nodeView.UpdateCurrentStatePreview();
                }
            }
        }

        private void BuildNodeCategoryForContextMenu<TNodeType>(string categoryName, ContextualMenuPopulateEvent evt, Vector2 nodePosition)
            where TNodeType : TreeNode
        {
            var nodeTypes = TypeCache
                .GetTypesDerivedFrom<TNodeType>()
                .Append(typeof(TNodeType))
                .Where(type => !type.IsAbstract)
                .ToList();
            
            for (int i = 0; i < nodeTypes.Count; i++)
            {
                var nodeType = nodeTypes[i];
                var nodeName = nodeType.Name;
                evt.menu.AppendAction($"{categoryName}/{nodeName}", _ => AddNodeToTreeAsset(nodeType, nodePosition));
            }
        }

        private void AddNodeToTreeAsset(Type nodeType, Vector2 nodePosition)
        {
            _tree.AddNodeAssetToTree(nodeType, nodePosition);
            SetTree(_tree);
        }

        private void OnUndoRedo()
        {
            SetTree(_tree);
            AssetDatabase.SaveAssets();
        }

        public void SetTree(BehaviourTree tree)
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            if (_tree)
            {
                _tree.OnTreeTick -= OnTreeTick;
                _tree.OnTreeStopped -= OnTreeStopped;
            }
            _tree = tree;
            
            if (!tree)
            {
                return;
            }

            _tree.OnTreeTick += OnTreeTick;
            _tree.OnTreeStopped += OnTreeStopped;
            graphViewChanged += OnGraphViewChanged;

            if (tree.RootNode == null)
            {
                tree.RootNode = tree.AddNodeAssetToTree<RootNode>(Vector2.zero);
            }

            // Create nodes
            for (var i = 0; i < tree.Nodes.Count; i++)
            {
                CreateNodeView(tree.Nodes[i]);
            }

            // Create edges
            for (var i = 0; i < tree.Nodes.Count; i++)
            {
                var node = tree.Nodes[i];
                var nodeView = GetNodeView(node);
                switch (node)
                {
                    case IOneChildNode oneChildNode:
                    {
                        if (oneChildNode.ChildAsBase)
                        {
                            var childView = GetNodeView(oneChildNode.ChildAsBase);
                            var edge = nodeView.Output.ConnectTo(childView.Input);
                            AddElement(edge);
                        }
                        break;
                    }
                    case IManyChildrenNode manyChildrenNode:
                    {
                        foreach (var child in manyChildrenNode.ChildrenAsBaseNode)
                        {
                            var childView = GetNodeView(child);
                            var edge = nodeView.Output.ConnectTo(childView.Input);
                            AddElement(edge);
                        }

                        break;
                    }
                }
            }
        }

        private void OnTreeStopped()
        {
            ClearVisitingStatus();
            UpdateNodeRuntimeState();
        }

        private void OnTreeTick()
        {
            UpdateNodeRuntimeState();
        }

        private void CreateNodeView(TreeNode node)
        {
            var nodeView = new NodeView(node);
            nodeView.OnNodeSelected += OnNodeViewSelected;
            AddElement(nodeView);
        }

        private void OnNodeViewSelected(NodeView nodeView)
        {
            OnNodeSelected?.Invoke(nodeView);
        }

        private NodeView GetNodeView(TreeNode node)
        {
            return GetNodeByGuid(node.Id) as NodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                for (int i = 0; i < graphViewChange.elementsToRemove.Count; i++)
                {
                    var elem = graphViewChange.elementsToRemove[i];
                    if (elem is NodeView nodeView)
                    {
                        _tree.DeleteNodeAsset(nodeView.Node);
                    }
                    else if (elem is Edge edge)
                    {
                        var parentNode = (edge.output.node as NodeView)?.Node;
                        switch (parentNode)
                        {
                            case IOneChildNode oneChildNode:
                                Undo.RecordObject(parentNode, "BehaviourTree (RemoveChild)");
                                oneChildNode.ChildAsBase = null;
                                EditorUtility.SetDirty(parentNode);
                                break;
                            case IManyChildrenNode manyChildrenNode:
                            {
                                var child = (edge.input.node as NodeView)?.Node;
                                if (child != null)
                                {
                                    Undo.RecordObject(parentNode, "BehaviourTree (RemoveChild)");
                                    manyChildrenNode.RemoveChild(child);
                                    manyChildrenNode.Sort();
                                    EditorUtility.SetDirty(parentNode);
                                }

                                break;
                            }
                        }
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                for (int i = 0; i < graphViewChange.edgesToCreate.Count; i++)
                {
                    var edge = graphViewChange.edgesToCreate[i];
                    var parentNode = (edge.output.node as NodeView)?.Node;
                    var childNode = (edge.input.node as NodeView)?.Node;
                    if (parentNode is IOneChildNode oneChildNode)
                    {
                        Undo.RecordObject(parentNode, "BehaviourTree: (AddChild)");
                        oneChildNode.ChildAsBase = childNode;
                        EditorUtility.SetDirty(parentNode);
                    }
                    else if (parentNode is IManyChildrenNode manyChildrenNode)
                    {
                        Undo.RecordObject(parentNode, "BehaviourTree: (AddChild)");
                        manyChildrenNode.AddChild(childNode);
                        manyChildrenNode.Sort();
                        EditorUtility.SetDirty(parentNode);
                    }
                }
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var result = new List<Port>();
            foreach (var port in ports)
            {
                if (port.direction != startPort.direction && port.node != startPort.node)
                {
                    var startNode = (startPort.node as NodeView)!;
                    var portNode = (port.node as NodeView)!;
                    var supportStartType = startPort.direction == Direction.Input
                        ? startNode.CompatibleInputTypes
                        : startNode.CompatibleOutputTypes;
                    var supportOtherType = port.direction == Direction.Input
                        ? portNode.CompatibleInputTypes
                        : portNode.CompatibleOutputTypes;

                    var startPortType = startNode.Node.GetType();
                    var otherPortType = portNode.Node.GetType();
                    if (supportStartType.Contains(otherPortType) && supportOtherType.Contains(startPortType))
                    {
                        result.Add(port);
                    }
                }
            }

            return result;
        }
    }
}