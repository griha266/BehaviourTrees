using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Shipico.BehaviourTrees.Editor
{
    public class NodeInspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<NodeInspectorView, UxmlTraits> { }

        private InspectorElement _inspector;
        public NodeInspectorView()
        {
            _inspector = new InspectorElement();
            Add(_inspector);
        }

        public void UpdateSelection(NodeView nodeView)
        {
            if (_inspector.binding != null)
            {
                _inspector.Unbind();
            }

            if (nodeView != null)
            {
                _inspector.Bind(new SerializedObject(nodeView.Node));
            }
        }
    }
}