using Concept.Helpers;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Concept.Helpers.ScreenUtils;
using static UnityEngine.UI.GridLayoutGroup;


namespace Concept.UI
{

    [AddComponentMenu("Layout/Responsive Grid Layout Group")]
    /// <summary>
    /// Layout class to arrange child elements in a grid format.
    /// </summary>
    /// <remarks>
    /// The GridLayoutGroup component is used to layout child layout elements in a uniform grid where all cells have the same size. The size and the spacing between cells is controlled by the GridLayoutGroup itself. The children have no influence on their sizes.
    /// </remarks>
    [ExecuteAlways]
    public class ResponsiveGridLayoutGroup : GridLayoutGroup
    {
        [SerializeField]
        public GridLayoutProperties landscapePreset;
        public GridLayoutProperties portraitPreset;

#if UNITY_EDITOR

        public GridLayoutProperties lastLandscapeLayout;
        public GridLayoutProperties lastPortraitLayout;
        protected override void OnValidate()
        {
            base.OnValidate();

            EditorApplication.delayCall += () =>
            {
                if (this == null) return; // Evita erros se o objeto foi destruído

                string currentLandscape = JsonUtility.ToJson(landscapePreset);
                string lastLandscape = JsonUtility.ToJson(lastLandscapeLayout);

                if (landscapePreset.isActive && currentLandscape != lastLandscape)
                {
                    ApplyPreset(landscapePreset);
                    lastLandscapeLayout = JsonUtility.FromJson<GridLayoutProperties>(currentLandscape);
                }

                string currentPortrait = JsonUtility.ToJson(portraitPreset);
                string lastPortrait = JsonUtility.ToJson(lastPortraitLayout);

                if (portraitPreset.isActive && currentPortrait != lastPortrait)
                {
                    ApplyPreset(portraitPreset);
                    lastPortraitLayout = JsonUtility.FromJson<GridLayoutProperties>(currentPortrait);
                }
            };

        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            ScreenUtils.OnResolutionChanged += OnResolutionChanged;

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ScreenUtils.OnResolutionChanged -= OnResolutionChanged;

        }

        protected override void Start()
        {
            base.Start();
                        if (!Application.isPlaying) return;
            ApplyPreset((Screen.width >= Screen.height) ? landscapePreset : portraitPreset);

        }

        private void OnResolutionChanged(float width, float height)
        {
            ApplyPreset((width >= height) ? landscapePreset : portraitPreset);
        }

        public void ApplyPreset(GridLayoutProperties preset)
        {
            padding = preset.padding;
            cellSize = preset.cellSize;
            spacing = preset.spacing;
            startCorner = preset.startCorner;
            startAxis = preset.startAxis;
            constraint = preset.constraint;
            constraintCount = preset.constraintCount;


        }

    }

    [Serializable]
    public class GridLayoutProperties
    {
        public RectOffset padding;
        public Vector2 cellSize;
        public Vector2 spacing;
        public Corner startCorner;
        public Axis startAxis;
        public TextAnchor childAlignment;
        public Constraint constraint;
        public int constraintCount;


#if UNITY_EDITOR

        public bool isActive;

        public bool IsDifferent(GridLayoutProperties other)
        {
            return !padding.Equals(other.padding)
                || cellSize != other.cellSize
                || spacing != other.spacing
                || startCorner != other.startCorner
                || startAxis != other.startAxis
                || childAlignment != other.childAlignment
                || constraint != other.constraint
                || constraintCount != other.constraintCount;
        }
#endif

    }

}