#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Concept.Editor;
using Concept.Helpers;
using System.Security.Cryptography;

namespace Concept.UI
{
    [CustomEditor(typeof(ResponsiveRectTransform))]
    public class ResponsiveRectTransformEditor : UnityEditor.Editor
    {
        ResponsiveRectTransform element;
        Vector2Int gameViewResolution;
        bool isLandscape;

        GUIStyle activeStyle;

        private void OnEnable()
        {
            element = (ResponsiveRectTransform)target;
        }

        public override void OnInspectorGUI()
        {
            gameViewResolution = GameViewResolutionMonitor.GetMainGameViewResolution();
            isLandscape = gameViewResolution.x >= gameViewResolution.y;

            if (activeStyle == null)
                activeStyle = new GUIStyle(GUI.skin.label)
                {
                    normal = { textColor = Color.green },
                    alignment = TextAnchor.MiddleRight
                };

            serializedObject.Update();

            // Exibir a propriedade forceLayoutByOrientation

            ScreenMonitorEditor.CheckScreenMonitor();


            EditorGUILayout.Space();


            EditorGUILayout.LabelField("Resolution Rect Presets", EditorStyles.boldLabel);

            SerializedProperty presetsProp = serializedObject.FindProperty("presets");



            DrawPresetProperties(presetsProp);
            if (HasInvalidPreset(presetsProp))
            {
                EditorGUILayout.HelpBox(
              "There are invalid Presets. Please fix it.",
              MessageType.Error
              );

            }


            EditorGUILayout.Space();





            if (presetsProp.arraySize == 0)
            {

                EditorGUILayout.HelpBox(
              "At least one preset must be added to Responsive Rect Transform works.",
              MessageType.Error
              );
            }




            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRectProperties(SerializedProperty property, bool active)
        {
            Color color = active ? Color.green : Color.gray;
            activeStyle.normal.textColor = color;

            GUILayout.BeginHorizontal();       // Início da margem horizontal

            string orientationName = (property.name == "landscapePreset" ? "Landscape" : "Portrait");
            Texture2D icon = LoadIcon(this, $"Icons/ICO_Orientation_{orientationName}.png");
            if (icon) GUILayout.Label(icon, GUILayout.Width(32), GUILayout.Height(32));

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField($"{orientationName} Rect Preset", EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            EditorGUILayout.BeginHorizontal();
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
                Vector2Int res = (property.name == "landscapePreset") ? new Vector2Int(1920, 1080) : new Vector2Int(1080, 1920);
                GameViewUtils.SetGameViewSize(res);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(12);              // Margem direita
            GUILayout.EndHorizontal();        // Fim da margem horizontal

            Rect boxRect = GUILayoutUtility.GetLastRect();
            DrawBoards(boxRect, color);

            EditorGUILayout.Space();
        }


        private void DrawPresetProperties(SerializedProperty property)
        {


            int indexToRemove = -1;

            EditorGUILayout.Space();
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                SerializedProperty resolutionProp = item.FindPropertyRelative("resolution");
                SerializedProperty rectTransformProp = item.FindPropertyRelative("rectPreset");

                bool active = resolutionProp.vector2IntValue == gameViewResolution;
                bool isLandscape = resolutionProp.vector2IntValue.x >= resolutionProp.vector2IntValue.y;
                string orientationName = (isLandscape ? "Landscape" : "Portrait");

                Color color = active ? Color.green : Color.gray;
                activeStyle.normal.textColor = color;
                string aspect = ScreenUtils.GetAspectLabel(resolutionProp.vector2IntValue);
                if (aspect == "Invalid") orientationName = aspect;

                GUILayout.BeginHorizontal(); // Adiciona espaço à esquerda e direita
                Texture2D icon = LoadIcon(this, $"Icons/ICO_Orientation_{orientationName}.png");
                if (icon) GUILayout.Label(icon, GUILayout.Width(32), GUILayout.Height(32));

                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Resolution", GUILayout.Width(70));
                GUILayout.FlexibleSpace();

                resolutionProp.vector2IntValue = new Vector2Int(
                    EditorGUILayout.IntField(resolutionProp.vector2IntValue.x, GUILayout.Width(50)),
                    resolutionProp.vector2IntValue.y
                );

                resolutionProp.vector2IntValue = new Vector2Int(
                    resolutionProp.vector2IntValue.x,
                    EditorGUILayout.IntField(resolutionProp.vector2IntValue.y, GUILayout.Width(50))
                );


                if ((property.name == "landscapePresets" && resolutionProp.vector2IntValue.x < resolutionProp.vector2IntValue.y)
                    || (property.name == "portraitPresets" && resolutionProp.vector2IntValue.x >= resolutionProp.vector2IntValue.y))
                {
                    aspect = "Invalid";
                }

                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    normal = { textColor = (aspect == "Invalid") ? new Color(1f, 0.4f, 0f) : Color.green },
                    alignment = TextAnchor.MiddleLeft
                };

                GUILayout.Label($"({aspect})", style, GUILayout.Width(56));
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    indexToRemove = i;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (active)
                    GUILayout.Label("(Active)", activeStyle);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Save Values", GUILayout.Width(120f)))
                {
                    RectTransform rect = rectTransformProp.objectReferenceValue as RectTransform;
                    if (rect != null)
                        element.ClonePreset(rect);
                }

                if (GUILayout.Button("Preview", GUILayout.Width(70f)))
                {
                    GameViewUtils.SetGameViewSize(resolutionProp.vector2IntValue);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                GUILayout.Space(12); // margem direita
                GUILayout.EndHorizontal();
                Rect boxRect = GUILayoutUtility.GetLastRect();
                DrawBoards(boxRect, color);

                EditorGUILayout.Space();
                string presetName = $"({aspect}) {orientationName} {resolutionProp.vector2IntValue.x}x{resolutionProp.vector2IntValue.y}";

                if (rectTransformProp.objectReferenceValue is RectTransform rt)
                {
                    if (rt.name != presetName)
                        rt.name = presetName;
                }
                else
                {
                    RectTransform newPreset = CreatePreset(presetName, element.transform);
                    rectTransformProp.objectReferenceValue = newPreset;
                }
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

            EditorGUILayout.Space();
            if (GUILayout.Button($"Add Rect Preset"))
            {
                AddPreset(property);
            }
        }


        public static bool HasInvalidPreset(SerializedProperty property)
        {
            bool hasInvalidAspect = false;

            // Verifica landscapePresets
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(i);
                SerializedProperty resolutionProp = element.FindPropertyRelative("resolution");

                if (resolutionProp != null)
                {
                    Vector2Int res = resolutionProp.vector2IntValue;
                    string label = ScreenUtils.GetAspectLabel(res);


                    if ((property.name == "landscapePresets" && resolutionProp.vector2IntValue.x < resolutionProp.vector2IntValue.y)
                                    || (property.name == "portraitPresets" && resolutionProp.vector2IntValue.x >= resolutionProp.vector2IntValue.y))
                        label = "Invalid";

                    if (label == "Invalid")
                    {
                        hasInvalidAspect = true;
                        break; // Já achou um inválido, não precisa continuar
                    }
                }
            }
            return hasInvalidAspect;
        }

        public static void DrawBoards(Rect rect, Color color)
        {
            const float padding = 6f;
            const float horizontalMargin = 4f;

            float x = rect.x - padding - horizontalMargin;
            float width = EditorGUIUtility.currentViewWidth - x - 16f;

            Rect borderRect = new Rect(
                x,
                rect.y - padding,
                width,
                rect.height + padding * 2
            );

            Handles.BeginGUI();
            Handles.color = color;
            Handles.DrawSolidRectangleWithOutline(borderRect, new Color(0, 0, 0, 0), color);
            Handles.EndGUI();
        }

        private RectTransform CreatePreset(string name, Transform parent)
        {
            RectTransform preset = parent.Find(name) as RectTransform;
            if (preset == null)
                preset = new GameObject(name, typeof(RectTransform), typeof(PresetAdvice)).GetComponent<RectTransform>();

            preset.transform.SetParent(parent, false);
            preset.gameObject.SetActive(false);
            return preset;
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
            SerializedProperty rectProp = newElement.FindPropertyRelative("rectPreset");
            string presetName = $"({ScreenUtils.GetAspectLabel(gameViewResolution)}) {(property.name == "landscapePresets" ? "Landscape" : "Portrait")} {resolutionProp.vector2IntValue.x}x{resolutionProp.vector2IntValue.y}";
            rectProp.objectReferenceValue = CreatePreset(presetName, element.transform);
        }

        public static Texture2D LoadIcon(object sender, string relativePathFromScript)
        {
            // Encontra o caminho do script atual
            MonoScript script = MonoScript.FromScriptableObject((ScriptableObject)sender);
            string scriptPath = AssetDatabase.GetAssetPath(script);

            // Resolve o caminho base
            string basePath = System.IO.Path.GetDirectoryName(scriptPath);

            // Junta com o caminho relativo até o ícone
            string iconPath = System.IO.Path.Combine(basePath, relativePathFromScript).Replace("\\", "/");

            return AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
        }

    }
}
#endif
