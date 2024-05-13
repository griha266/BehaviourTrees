using UnityEditor;
using UnityEngine;

namespace Shipico.BehaviourTrees.Editor
{
    [CustomPropertyDrawer(typeof(NormalizedFloat))]
    public class NormalizedFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var valueRect = new Rect(position.x, position.y, position.width, position.height);

            var valueProperty = property.FindPropertyRelative("value");
            EditorGUI.Slider(valueRect, valueProperty, 0f, 1f, GUIContent.none);
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}