#if UNITY_EDITOR
using Concept.Core;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PresetAdvice))]
[CanEditMultipleObjects]
public class PresetAdviceEditor : Editor
{

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox(
            "This component is used as Responsive Layout Preset. Do not remove it!",
            MessageType.Warning
        );

    }
}
#endif