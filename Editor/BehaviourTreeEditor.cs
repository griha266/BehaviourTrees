using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Shipico.BehaviourTrees.Editor
{
    public class BehaviourTreeEditor : EditorWindow
    {
        
        [MenuItem("Window/BehaviourTrees/Editor")]
        public static void OpenEditor()
        {
            GetEditorWindow();
        }
        
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line) {
            if (Selection.activeObject is BehaviourTree tree)
            {
                var editor = GetEditorWindow();
                editor.SetTree(tree);
                return true;
            }
            return false;
        }
        
        private static BehaviourTreeEditor GetEditorWindow()
        {
            var window = GetWindow<BehaviourTreeEditor>();
            window.titleContent = new GUIContent("BehaviourTreeEditor");
            return window;
        }
        
        private BehaviourTree _currentTree;
        private BehaviourTreeGraphView _treeView;
        private NodeInspectorView _nodeInspector;

        public void CreateGUI()
        {
            var root = BehaviourTreeEditorUtils.GetPackageRootPath();
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{root}/Editor/Resources/BehaviourTreeEditor.uxml");
            visualTreeAsset.CloneTree(rootVisualElement);
            _treeView = rootVisualElement.Q<BehaviourTreeGraphView>();
            _treeView.OnNodeSelected += OnNodeSelectedInTree;
            _nodeInspector = rootVisualElement.Q<NodeInspectorView>();
            Selection.selectionChanged += OnUnitySelectionChanged;
            SetTree(null);
        }

        private void OnUnitySelectionChanged()
        {
            var selectedObject = Selection.activeObject;
            if (selectedObject is BehaviourTree tree)
            {
                SetTree(tree);
                _nodeInspector.UpdateSelection(null);
            } else if (selectedObject is GameObject gameObject)
            {
                if (gameObject.TryGetComponent(out BehaviourTreeRunner treeRunner) && treeRunner.CurrentTree)
                {
                    SetTree(treeRunner.CurrentTree);
                    _nodeInspector.UpdateSelection(null);
                }
            }
            else
            {
                SetTree(null);
                _nodeInspector.UpdateSelection(null);
            }
        }

        public void SetTree(BehaviourTree tree)
        {
            _treeView.SetTree(tree);
            _treeView.FrameAll();
        }

        private void OnNodeSelectedInTree(NodeView view)
        {
            _nodeInspector.UpdateSelection(view);
        }
    }
}
