#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Concept.Core;
using Amazon.Runtime;
using Concept.Editor;

namespace Concept.UI
{
    [CustomEditor(typeof(ResponsiveRectTransform))]
    public class ResponsiveRectTransformEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            float fullWidth = EditorGUIUtility.currentViewWidth;
            float buttonWidth = 70f;
            float fieldWidth = fullWidth - buttonWidth - 32f;
            Vector2 gameViewResolution = GameViewResolutionMonitor.GetMainGameViewResolution();

            ResponsiveRectTransform element = (ResponsiveRectTransform)target;

            // Verifica dependência
            InputMonitor monitor = FindFirstObjectByType<InputMonitor>();
            if (!monitor)
            {
                EditorGUILayout.HelpBox(
                    "This component depends on an 'InputMonitor' MonoBehaviour to function correctly at runtime.",
                    MessageType.Error
                );
            }

            serializedObject.Update();

            // Exibir a propriedade forceLayoutByOrientation
            var landscapePresetProp = serializedObject.FindProperty("landscapePreset");
            var portraitPresetProp = serializedObject.FindProperty("portraitPreset");

            GUIStyle activeStyle = new GUIStyle(GUI.skin.label);
            activeStyle.normal.textColor = Color.green;
            activeStyle.alignment = TextAnchor.MiddleRight;


            EditorGUILayout.LabelField("Orientarion Rect Presets", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(landscapePresetProp, new GUIContent("Landscape Preset"));

            // Adiciona o ícone de alerta ao lado do campo "landscapePreset" se o rectPreset for null
            if (landscapePresetProp.objectReferenceValue == null)
            {
                GUIStyle warningStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.yellow }
                };
                EditorGUILayout.LabelField("⚠", warningStyle, GUILayout.Width(20));
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(gameViewResolution.x >= gameViewResolution.y)
            GUILayout.Label("(Active)", activeStyle);

            if (GUILayout.Button("Save Values", GUILayout.Width(100f)))
            {
                RectTransform rect = landscapePresetProp.objectReferenceValue as RectTransform;
                if (rect != null)
                    element.ClonePreset(rect);
            }
            if (GUILayout.Button("Preview", GUILayout.Width(100f)))
            {
                RectTransform rect = landscapePresetProp.objectReferenceValue as RectTransform;
                if (rect != null)
                    element.ApplyPreset(rect);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(portraitPresetProp, new GUIContent("Portrait Preset"));

            // Adiciona o ícone de alerta ao lado do campo "portraitPreset" se o rectPreset for null
            if (portraitPresetProp.objectReferenceValue == null)
            {
                GUIStyle warningStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.yellow }
                };
                EditorGUILayout.LabelField("⚠", warningStyle, GUILayout.Width(20));
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            if (gameViewResolution.x < gameViewResolution.y)
                GUILayout.Label("(Active)", activeStyle);

            if (GUILayout.Button("Save Values", GUILayout.Width(100f)))
            {
                RectTransform rect = portraitPresetProp.objectReferenceValue as RectTransform;
                if (rect != null)
                    element.ClonePreset(rect);
            }
            if (GUILayout.Button("Preview", GUILayout.Width(100f)))
            {
                RectTransform rect = portraitPresetProp.objectReferenceValue as RectTransform;
                if (rect != null)
                    element.ApplyPreset(rect);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (landscapePresetProp.objectReferenceValue == null || portraitPresetProp.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(
                    "Both Landscape and Portrait presets are required and must have RectTransforms assigned to them.",
                    MessageType.Warning
                );
            }
            SerializedProperty forceLayoutProp = serializedObject.FindProperty("forceLayoutByOrientation");
            EditorGUILayout.PropertyField(forceLayoutProp);
            // Condicionalmente desenha as propriedades relacionadas a forceLayoutByOrientation
            if (!forceLayoutProp.boolValue)
            {


                // Exibe a lista de presets se forceLayoutByOrientation for falso
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Resolution Rect Presets", EditorStyles.boldLabel);

                SerializedProperty presetsProp = serializedObject.FindProperty("rectPresets");
                int indexToRemove = -1;

                // Desenhando cada item da lista de Layout Presets
                for (int i = 0; i < presetsProp.arraySize; i++)
                {
                    SerializedProperty item = presetsProp.GetArrayElementAtIndex(i);
                    SerializedProperty orientationProp = item.FindPropertyRelative("resolution");
                    var rectTransformProp = item.FindPropertyRelative("rectPreset");

                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(orientationProp, new GUIContent("Resolution"));

                    // Botão de remover (X) no canto direito
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        indexToRemove = i;
                    }
                    EditorGUILayout.EndHorizontal();

                    // Campo RectTransform + Botão Preview


                    EditorGUILayout.PropertyField(rectTransformProp, new GUIContent("Rect Preset"), GUILayout.Width(fieldWidth));
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Save Values", GUILayout.Width(120f)))
                    {
                        RectTransform rect = portraitPresetProp.objectReferenceValue as RectTransform;
                        if (rect != null)
                            element.ClonePreset(rect);
                    }
                    if (GUILayout.Button("Preview", GUILayout.Width(buttonWidth)))
                    {
                        RectTransform rect = rectTransformProp.objectReferenceValue as RectTransform;
                        if (rect != null)
                            element.ApplyPreset(rect);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }

                // Remoção segura após o loop
                if (indexToRemove >= 0)
                {
                    presetsProp.DeleteArrayElementAtIndex(indexToRemove);
                }

                // Botão para adicionar mais presets
                EditorGUILayout.Space();
                if (GUILayout.Button("Add Preset"))
                {
                    presetsProp.arraySize++;
                }

                EditorGUILayout.HelpBox(
              "If no compatible resolution is set, it will automatically select based on the Orientation Rect Presets.",
              MessageType.Info
              );

            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
