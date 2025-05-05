#if UNITY_EDITOR
using Concept.Editor;
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
        SerializedProperty landscapeLayoutProp;
        SerializedProperty portraitLayoutProp;

        private void OnEnable()
        {
            landscapeLayoutProp = serializedObject.FindProperty("landscapePreset");
            portraitLayoutProp = serializedObject.FindProperty("portraitPreset");
        }

        public override void OnInspectorGUI()
        {
            Vector2 gameViewResolution = GameViewResolutionMonitor.GetMainGameViewResolution();
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Responsive Layout Presets", EditorStyles.boldLabel);

            DrawGridLayoutProperties(landscapeLayoutProp, "Landscape Layout", gameViewResolution.x >= gameViewResolution.y);
            EditorGUILayout.Space();
            DrawGridLayoutProperties(portraitLayoutProp, "Portrait Layout", gameViewResolution.x < gameViewResolution.y);



            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGridLayoutProperties(SerializedProperty property, string label, bool active = false)
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

            // Aqui desenhamos as propriedades do GridLayoutProperties manualmente
            EditorGUILayout.PropertyField(property.FindPropertyRelative("padding"), new GUIContent("Padding"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("cellSize"), new GUIContent("Cell Size"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("spacing"), new GUIContent("Spacing"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("startCorner"), new GUIContent("Start Corner"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("startAxis"), new GUIContent("Start Axis"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("childAlignment"), new GUIContent("Child Alignment"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("constraint"), new GUIContent("Constraint"));
            property.FindPropertyRelative("isActive").boolValue = active;

            // Verifica se a constraint não é do tipo Flexible antes de desenhar constraintCount
            SerializedProperty constraintProp = property.FindPropertyRelative("constraint");
            if (constraintProp.enumValueIndex != (int)GridLayoutGroup.Constraint.Flexible) // Verifica se não é Flexible
            {
                EditorGUILayout.PropertyField(property.FindPropertyRelative("constraintCount"), new GUIContent("Constraint Count"));
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Preview", GUILayout.Width(100f)))
            {
                ResponsiveGridLayoutGroup element = (ResponsiveGridLayoutGroup)target;
                if (property.name == "landscapePreset")
                {
                    element.ApplyPreset(element.landscapePreset);
                }
                else if (property.name == "portraitPreset")
                {
                    element.ApplyPreset(element.portraitPreset);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }



    }
}
#endif
