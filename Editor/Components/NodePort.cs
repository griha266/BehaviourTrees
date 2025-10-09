using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shipico.BehaviourTrees.Editor
{
    // Basic class has wierd logic for determining hover in graph
    public class NodePort : Port
    {
        // Cannot reuse private class from base Port class, so need to do copy-paste
        private class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            private readonly GraphViewChange _graphViewChange;
            private readonly List<Edge> _edgesToCreate;
            private readonly List<GraphElement> _edgesToDelete;

            public DefaultEdgeConnectorListener()
            {
                _edgesToCreate = new List<Edge>();
                _edgesToDelete = new List<GraphElement>();
                _graphViewChange.edgesToCreate = _edgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                _edgesToCreate.Clear();
                _edgesToCreate.Add(edge);
                _edgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                {
                    foreach (var connection in edge.input.connections)
                    {
                        if (connection != edge)
                            _edgesToDelete.Add(connection);
                    }
                }

                if (edge.output.capacity == Capacity.Single)
                {
                    foreach (var connection in edge.output.connections)
                    {
                        if (connection != edge)
                            _edgesToDelete.Add(connection);
                    }
                }

                if (_edgesToDelete.Count > 0)
                    graphView.DeleteElements(_edgesToDelete);
                var edgesToCreate = _edgesToCreate;
                if (graphView.graphViewChanged != null)
                    edgesToCreate = graphView.graphViewChanged(_graphViewChange).edgesToCreate;
                foreach (var edge1 in edgesToCreate)
                {
                    graphView.AddElement(edge1);
                    edge.input.Connect(edge1);
                    edge.output.Connect(edge1);
                }
            }
        }

        public static NodePort Create(Direction direction, Capacity capacity)
        {
            var listener = new DefaultEdgeConnectorListener();
            var edgeConnector = new EdgeConnector<Edge>(listener);
            var port = new NodePort(direction, capacity)
            {
                m_EdgeConnector = edgeConnector
            };
            port.AddManipulator(edgeConnector);
            return port;
        }

        protected NodePort(Direction portDirection, Capacity portCapacity) : base(
            Orientation.Vertical, portDirection, portCapacity, typeof(bool))
        {
            var label = this.Q<Label>();
            label.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            var rect = new Rect(0, 0, layout.width, layout.height);
            return rect.Contains(localPoint);
        }
    }
}