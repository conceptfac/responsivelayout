#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Reflection;

public static class GameViewUtils
{

    public static void SetGameViewSize(int width, int height)
    {
        var sizesInstance = GetGroup();

        var addCustomSize = sizesInstance.GetType().GetMethod("AddCustomSize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gameViewSizeType = sizesInstance.GetType().Assembly.GetType("UnityEditor.GameViewSize");
        var gameViewSizeTypeEnum = sizesInstance.GetType().Assembly.GetType("UnityEditor.GameViewSizeType");

        var ctor = gameViewSizeType.GetConstructor(new Type[] {
            gameViewSizeTypeEnum, typeof(int), typeof(int), typeof(string)
        });

        var fixedResolutionEnum = Enum.Parse(gameViewSizeTypeEnum, "FixedResolution");

        var newSize = ctor.Invoke(new object[] {
            fixedResolutionEnum, width, height, $"{width}x{height}_TEMP"
        });

        addCustomSize.Invoke(sizesInstance, new object[] { newSize });

        // Select the new size
        var getDisplayTexts = sizesInstance.GetType().GetMethod("GetDisplayTexts", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        string[] displayTexts = getDisplayTexts.Invoke(sizesInstance, null) as string[];

        for (int i = 0; i < displayTexts.Length; i++)
        {
            if (displayTexts[i].Contains($"{width}x{height}_TEMP"))
            {
                SetSize(i);
                break;
            }
        }
    }

    private static void SetSize(int index)
    {
        var gameViewWindowType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var window = EditorWindow.GetWindow(gameViewWindowType);
        var selectedSizeIndexProp = gameViewWindowType.GetProperty("selectedSizeIndex",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        selectedSizeIndexProp.SetValue(window, index, null);
    }

    private static object GetGroup()
    {
        var T = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(T);
        var instanceProp = singleType.GetProperty("instance");
        var instance = instanceProp.GetValue(null, null);

        var getGroup = T.GetMethod("GetGroup");
        return getGroup.Invoke(instance, new object[] { 0 });
    }
}
#endif