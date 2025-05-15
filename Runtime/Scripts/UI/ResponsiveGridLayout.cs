using Concept.Core;
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
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class ResponsiveGridLayoutGroup : GridLayoutGroup
    {
        private RectTransform _rectTransform;
        public GridLayoutProperties[] presets;

#if UNITY_EDITOR
        public int _activePreset;
        private GridLayoutProperties _lastPreset;
        protected override void OnValidate()
        {
            base.OnValidate();

            EditorApplication.delayCall += () =>
            {
                if (this == null) return; // Evita erros se o objeto foi destruído
                if(presets.Length <= _activePreset)
                {
                    //if (presets.Length > 0) ApplyPreset(presets[0]);
                    
                    
                    return;
                       
                }
                string currentLandscape = JsonUtility.ToJson(presets[_activePreset]);
                string lastLandscape = JsonUtility.ToJson(_lastPreset);

                if (presets[_activePreset].isActive && currentLandscape != lastLandscape)
                {
                    ApplyPreset(presets[_activePreset]);
                    _lastPreset = JsonUtility.FromJson<GridLayoutProperties>(currentLandscape);
                }


            };
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            _rectTransform = GetComponent<RectTransform>();
            ScreenMonitor.OnResolutionChanged += OnResolutionChanged;

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ScreenMonitor.OnResolutionChanged -= OnResolutionChanged;

        }

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;


            OnResolutionChanged(Screen.width, Screen.height);
            _rectTransform.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
        }


        private void OnResolutionChanged(int width, int height)
        {
            if(presets == null)
            {
                Debug.LogError($"[ResponsiveGridLayoutGroup] Invalid presets in {gameObject.name}");
                return;
            }

            GridLayoutProperties preset = null;
            Vector2Int currentRes = new Vector2Int(width, height);


            string currentAspect = ScreenUtils.GetAspectLabel(currentRes);
            GridLayoutProperties closestPreset = null;
            float closestDistance = float.MaxValue;

            foreach (var p in presets)
            {


                string presetAspect = ScreenUtils.GetAspectLabel(p.resolution);
                if (presetAspect == "Invalid") continue;

                if (currentAspect != "Invalid" && presetAspect != currentAspect)
                    continue;

                float dist = Vector2.Distance(currentRes, p.resolution);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestPreset = p;
                }
            }

            if (closestPreset == null && currentAspect != "Invalid")
            {
                foreach (var p in presets)
                {
                    string presetAspect = ScreenUtils.GetAspectLabel(p.resolution);
                    if (presetAspect == "Invalid") continue;

                    float dist = Vector2.Distance(currentRes, p.resolution);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestPreset = p;
                    }
                }
            }

            if (closestPreset != null)
            {
                preset = closestPreset;
            }



            if (preset != null)
            {
              //  Debug.Log("Applying preset resolution: " + preset.resolution);
#if UNITY_EDITOR
                _activePreset = Array.IndexOf(presets, preset);
#endif
                ApplyPreset(preset);
            }
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
        public Vector2Int resolution;
        public RectOffset padding;
        public Vector2 cellSize;
        public Vector2 spacing;
        public Corner startCorner;
        public Axis startAxis;
        public TextAnchor childAlignment;
        public Constraint constraint;
        public int constraintCount;


#if UNITY_EDITOR

        [HideInInspector]
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