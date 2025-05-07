#if UNITY_EDITOR
using Concept.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Concept.UI;
using UnityEngine.Events;

[CustomEditor(typeof(ResponsiveCallbackEvents))]
    [CanEditMultipleObjects]

    public class ResponsiveCallbackEventsEditor : Editor
    {
    SerializedProperty OnLandscapeOrientation;
    SerializedProperty OnPortraitOrientation;
    private void OnEnable()
    {
        OnLandscapeOrientation = serializedObject.FindProperty("OnLandscapeOrientation");
        OnPortraitOrientation = serializedObject.FindProperty("OnPortraitOrientation");
    }

    public override void OnInspectorGUI()
    {
        Vector2 gameViewResolution = GameViewResolutionMonitor.GetMainGameViewResolution();
        bool isLandscape = gameViewResolution.x >= gameViewResolution.y;
        EditorGUILayout.Space();
        PresetAdviceEditor.CheckResolutionMonitor();
        EditorGUILayout.LabelField("Responsive Callback Events", EditorStyles.boldLabel);

        DrawCallbacksProperties(OnLandscapeOrientation, "Landscape Callbacks", isLandscape);
        EditorGUILayout.Space();
        DrawCallbacksProperties(OnPortraitOrientation, "Portrait Callbacks", !isLandscape);


        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCallbacksProperties(SerializedProperty property, string label, bool active = false)
    {
        Color color = active ? Color.green : Color.gray;
        GUIStyle activeStyle = new GUIStyle(GUI.skin.label);
        activeStyle.normal.textColor = Color.green;
        activeStyle.alignment = TextAnchor.MiddleRight;

        const float padding = 6f;

        EditorGUILayout.Space(5);

        // Começa uma área desenhável
        Rect rect = EditorGUILayout.BeginVertical(GUI.skin.box);

        // Corrige a largura com base na view atual
        float fullWidth = EditorGUIUtility.currentViewWidth - 40f; // Subtrai o espaço reservado para scrollbars/margens

        // Redefine o retângulo com largura manualmente definida
        Rect borderRect = new Rect(
            rect.x - padding,
            rect.y - padding,
            fullWidth + padding * 2,
            rect.height + padding * 2
        );

        // Desenha a borda
        Handles.BeginGUI();
        Handles.color = color;
        Handles.DrawSolidRectangleWithOutline(borderRect, new Color(0, 0, 0, 0), color);
        Handles.EndGUI();

        // Campos internos - desenha cada propriedade individualmente
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        if (active)
            GUILayout.Label("(Active)", activeStyle);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(property, new GUIContent(label));

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Simulate Callbacks", GUILayout.Width(120f)))
        {
            ResponsiveCallbackEvents element = (ResponsiveCallbackEvents)target;

            if (property.name == "OnLandscapeOrientation")
            {
                element.OnLandscapeOrientation?.Invoke();
            }
            else if (property.name == "OnPortraitOrientation")
            {
                element.OnPortraitOrientation?.Invoke();
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }


}

#endif