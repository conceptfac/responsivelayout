using UnityEditor;
using UnityEngine;

namespace Concept.UI
{


[CustomEditor(typeof(ExpandableText))]
[CanEditMultipleObjects]
public class ExpandableTextEditor : UnityEditor.Editor
{
    SerializedProperty isCollapsed;
    SerializedProperty minHeight;
    SerializedProperty forceReubuild;

    void OnEnable()
    {
        isCollapsed = serializedObject.FindProperty("isCollapsed");
        minHeight = serializedObject.FindProperty("m_minHeight");
        forceReubuild = serializedObject.FindProperty("_forceRebuildTransforms");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Só desenha os campos da ExpandableText, não da base LayoutElement
        EditorGUILayout.PropertyField(isCollapsed);
        EditorGUILayout.PropertyField(minHeight);
        EditorGUILayout.PropertyField(forceReubuild);

        serializedObject.ApplyModifiedProperties();
    }
}

}