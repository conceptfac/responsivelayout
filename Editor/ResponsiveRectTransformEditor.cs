#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Concept.Core;
using Concept.Editor;

namespace Concept.UI
{
    [CustomEditor(typeof(ResponsiveRectTransform))]
    public class ResponsiveRectTransformEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ResponsiveRectTransform element = (ResponsiveRectTransform)target;
            float fullWidth = EditorGUIUtility.currentViewWidth;
            float buttonWidth = 70f;
            float fieldWidth = fullWidth - buttonWidth - 32f;
            Vector2 gameViewResolution = GameViewResolutionMonitor.GetMainGameViewResolution();
            bool isLandscape = gameViewResolution.x >= gameViewResolution.y;

            // Verifica dependência
           

            serializedObject.Update();

            // Exibir a propriedade forceLayoutByOrientation
            var landscapePresetProp = serializedObject.FindProperty("landscapePreset");
            var portraitPresetProp = serializedObject.FindProperty("portraitPreset");

            PresetAdviceEditor.CheckResolutionMonitor();

            EditorGUILayout.Space();
            DrawRectProperties(landscapePresetProp, isLandscape);
            EditorGUILayout.Space();
            DrawRectProperties(portraitPresetProp, !isLandscape);



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

        private void DrawRectProperties(SerializedProperty property, bool active)
        {
            ResponsiveRectTransform element = (ResponsiveRectTransform)target;
            Color color = active ? Color.green : Color.gray;
            GUIStyle activeStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = color },
                alignment = TextAnchor.MiddleRight
            };


            // Começa um bloco visual com fundo de box
            EditorGUILayout.BeginVertical(GUI.skin.box);

            // Desenha conteúdo aqui
            EditorGUILayout.LabelField("Orientation Rect Presets", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Landscape Preset", EditorStyles.label);
            GUILayout.FlexibleSpace();
            if (active)
                GUILayout.Label("(Active)", activeStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save Values", GUILayout.Width(100f)))
            {
                RectTransform rectProp = property.objectReferenceValue as RectTransform;
                if (rectProp != null)
                    element.ClonePreset(rectProp);
            }

            if (GUILayout.Button("Preview", GUILayout.Width(100f)))
            {
                RectTransform rectProp = property.objectReferenceValue as RectTransform;
                if (rectProp != null)
                    element.ApplyPreset(rectProp);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // Fecha o bloco antes de capturar a área
            EditorGUILayout.EndVertical();

            // Agora, com o conteúdo desenhado, podemos pegar o retângulo real
            Rect rect = GUILayoutUtility.GetLastRect();

            const float padding = 6f;
            const float horizontalMargin = 4f;
            float x = rect.x - padding - horizontalMargin;
            float width = EditorGUIUtility.currentViewWidth - x - 20f;
            Rect borderRect = new Rect(
                x,
                rect.y - padding,
                width,
                rect.height + padding * 2
            );




            // Desenha o contorno
            Handles.BeginGUI();
            Handles.color = color;
            Handles.DrawSolidRectangleWithOutline(borderRect, new Color(0, 0, 0, 0), color);
            Handles.EndGUI();
            EditorGUILayout.Space();

        }
    }
}
#endif
