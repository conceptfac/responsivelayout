#if UNITY_EDITOR
using Concept.Editor;
using Concept.Helpers;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Concept.UI
{
    [CustomEditor(typeof(ResponsiveGridLayoutGroup))]
    [CanEditMultipleObjects]
    public class ResponsiveGridLayoutGroupEditor : UnityEditor.Editor
    {
        ResponsiveGridLayoutGroup element;
        SerializedProperty presetsProp;
        Vector2Int gameViewResolution;
        private Dictionary<int, bool> foldoutStates = new();

        private void OnEnable()
        {
            element = (ResponsiveGridLayoutGroup)target;
            presetsProp = serializedObject.FindProperty("presets");
        }

        public override void OnInspectorGUI()
        {
            gameViewResolution = GameViewResolutionMonitor.GetMainGameViewResolution();

            serializedObject.Update();

            ScreenMonitorEditor.CheckScreenMonitor();


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Responsive Layout Presets", EditorStyles.boldLabel);
            EditorGUILayout.Space();


            if (presetsProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox(
              "At least one preset must be added to Responsive Grid Layout works.",
              MessageType.Error
              );
            }
            else
            if (ResponsiveRectTransformEditor.HasInvalidPreset(presetsProp))
            {
                EditorGUILayout.HelpBox(
              "There are invalid Presets. Please fix it.",
              MessageType.Error
              );

            }

            DrawGridLayoutPresets(presetsProp);


            EditorGUILayout.Space();



            serializedObject.ApplyModifiedProperties();
        }


        private void DrawGridLayoutPresets(SerializedProperty property)
        {
            int indexToRemove = -1;

            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                SerializedProperty resolutionProp = item.FindPropertyRelative("resolution");
                bool isLandscape = resolutionProp.vector2IntValue.x >= resolutionProp.vector2IntValue.y;
                string orientationName = (isLandscape ? "Landscape" : "Portrait");
                bool active = resolutionProp.vector2IntValue == gameViewResolution;
                Color color = active ? Color.green : Color.gray;
                GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.margin = new RectOffset(0, 0, 4, 4);
                boxStyle.padding = new RectOffset(10, 10, 4, 4);
                string aspect = ScreenUtils.GetAspectLabel(resolutionProp.vector2IntValue);
                if (aspect == "Invalid") orientationName = aspect;
                EditorGUILayout.BeginVertical(boxStyle);

                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                Texture2D icon = ResponsiveRectTransformEditor.LoadIcon(this, $"Icons/ICO_Orientation_{orientationName}.png");
                if (icon) GUILayout.Label(icon, GUILayout.Width(32), GUILayout.Height(32));

                EditorGUILayout.BeginVertical(GUILayout.Height(32));
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
                EditorGUILayout.LabelField("Resolution", GUILayout.Width(80));

                Vector2Int currentRes = resolutionProp.vector2IntValue;
                currentRes.x = EditorGUILayout.IntField(currentRes.x, GUILayout.Width(60));
                currentRes.y = EditorGUILayout.IntField(currentRes.y, GUILayout.Width(60));

                resolutionProp.vector2IntValue = currentRes;




                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    normal = { textColor = (aspect == "Invalid") ? new Color(1f, 0.4f, 0f) : Color.green },
                    alignment = TextAnchor.MiddleLeft
                };

                GUILayout.Label($"({aspect})", style, GUILayout.Width(56));
                EditorGUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();


                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    indexToRemove = i;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                // Garante que o foldout esteja inicializado
                if (!foldoutStates.ContainsKey(i))
                    foldoutStates[i] = false;

                // Foldout para esse item do array
                EditorGUILayout.BeginHorizontal();
                Rect foldoutRect = GUILayoutUtility.GetRect(140, EditorGUIUtility.singleLineHeight);
                foldoutStates[i] = EditorGUI.Foldout(foldoutRect, foldoutStates[i], "Grid Layout Options", true);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                if (foldoutStates[i])
                {
                    EditorGUILayout.PropertyField(item.FindPropertyRelative("padding"));
                    EditorGUILayout.PropertyField(item.FindPropertyRelative("cellSize"));
                    EditorGUILayout.PropertyField(item.FindPropertyRelative("spacing"));
                    EditorGUILayout.PropertyField(item.FindPropertyRelative("startCorner"));
                    EditorGUILayout.PropertyField(item.FindPropertyRelative("startAxis"));
                    EditorGUILayout.PropertyField(item.FindPropertyRelative("childAlignment"));
                    EditorGUILayout.PropertyField(item.FindPropertyRelative("constraint"));

                    SerializedProperty constraintProp = item.FindPropertyRelative("constraint");
                    if (constraintProp.enumValueIndex != (int)GridLayoutGroup.Constraint.Flexible)
                    {
                        EditorGUILayout.PropertyField(item.FindPropertyRelative("constraintCount"));
                    }
                    EditorGUILayout.Space();
                }
                else
                {

                    GUILayout.Space(-GUI.skin.button.CalcSize(new GUIContent("Preview")).y);
                }

                item.FindPropertyRelative("isActive").boolValue = active;

                EditorGUI.indentLevel--;


                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Preview", GUILayout.Width(120f)))
                {
                    GameViewUtils.SetGameViewSize(resolutionProp.vector2IntValue);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                Rect boxRect = GUILayoutUtility.GetLastRect();
                ResponsiveRectTransformEditor.DrawBoards(boxRect, color);


                EditorGUILayout.Space();

            }


            if (indexToRemove >= 0)
            {
                SerializedProperty itemToRemove = property.GetArrayElementAtIndex(indexToRemove);
                SerializedProperty rectPresetProp = itemToRemove.FindPropertyRelative("rectPreset");

                if (rectPresetProp != null && rectPresetProp.objectReferenceValue != null)
                {
                    GameObject go = (rectPresetProp.objectReferenceValue as Component)?.gameObject;
                    if (go != null)
                    {
                        Undo.DestroyObjectImmediate(go);
                    }
                }

                property.DeleteArrayElementAtIndex(indexToRemove);
                if (element._activePreset == indexToRemove && element.presets.Length > 0)
                    GameViewUtils.SetGameViewSize(element.presets[0].resolution);

            }

            if (GUILayout.Button($"Add New Preset"))
            {
                AddPreset(property);
            }
        }

        private void AddPreset(SerializedProperty property)
        {
            int index = property.arraySize;
            property.InsertArrayElementAtIndex(index);

            // Limpa os campos do novo elemento
            SerializedProperty newElement = property.GetArrayElementAtIndex(index);

            // Exemplo: zera o campo "resolution"
            SerializedProperty resolutionProp = newElement.FindPropertyRelative("resolution");
            resolutionProp.vector2IntValue = Vector2Int.zero;
        }
    }

}
#endif
