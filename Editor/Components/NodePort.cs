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
        private GraphViewChange m_GraphViewChange;
        private List<Edge> m_EdgesToCreate;
        private List<GraphElement> m_EdgesToDelete;

        public DefaultEdgeConnectorListener()
        {
          m_EdgesToCreate = new List<Edge>();
          m_EdgesToDelete = new List<GraphElement>();
          m_GraphViewChange.edgesToCreate = this.m_EdgesToCreate;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
          m_EdgesToCreate.Clear();
          m_EdgesToCreate.Add(edge);
          m_EdgesToDelete.Clear();
          if (edge.input.capacity == Capacity.Single)
          {
            foreach (Edge connection in edge.input.connections)
            {
              if (connection != edge)
                m_EdgesToDelete.Add(connection);
            }
          }

          if (edge.output.capacity == Capacity.Single)
          {
            foreach (Edge connection in edge.output.connections)
            {
              if (connection != edge)
                m_EdgesToDelete.Add(connection);
            }
          }

          if (m_EdgesToDelete.Count > 0)
            graphView.DeleteElements(m_EdgesToDelete);
          List<Edge> edgesToCreate = m_EdgesToCreate;
          if (graphView.graphViewChanged != null)
            edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
          foreach (Edge edge1 in edgesToCreate)
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