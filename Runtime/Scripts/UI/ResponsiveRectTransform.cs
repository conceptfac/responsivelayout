using Concept.Core;
using Concept.Helpers;
using System;
using System.Linq;
using UnityEngine;

namespace Concept.UI
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class ResponsiveRectTransform : MonoBehaviour
    {
        private RectTransform _rectTransform;
        public bool forceLayoutByOrientation = true;
        public RectTransform landscapePreset;
        public RectTransform portraitPreset;

        public RectPreset[] landscapePresets;
        public RectPreset[] portraitPresets;


        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();
            ScreenUtils.OnResolutionChanged += OnResolutionChanged;
        }

        private void OnDisable()
        {
            ScreenUtils.OnResolutionChanged -= OnResolutionChanged;

        }

        private void Start()
        {
            if (!Application.isPlaying) return;
            ApplyPreset((Screen.width >= Screen.height) ? landscapePreset : portraitPreset);
        }


        private void OnDestroy()
        {
            if (landscapePreset) DestroyImmediate(landscapePreset.gameObject);
            if (portraitPreset) DestroyImmediate(portraitPreset.gameObject);
        }



        public void ClonePreset(RectTransform target)
        {
            ScreenUtils.CloneRectTransform(_rectTransform, target);
        }

        public void ApplyPreset(RectTransform rectPreset)
        {
            ScreenUtils.CloneRectTransform(rectPreset, _rectTransform);
        }

        #region Callback Methods
        private void OnResolutionChanged(int width, int height)
        {
            RectTransform preset = null;
            Vector2Int currentRes = new Vector2Int(width, height);

            if (!forceLayoutByOrientation)
            {
                RectPreset[] presets = (width >= height) ? landscapePresets : portraitPresets;

                string currentAspect = ScreenUtils.GetAspectLabel(currentRes);
                RectPreset closestPreset = null;
                float closestDistance = float.MaxValue;

                foreach (var p in presets)
                {
                    if (p.rectPreset == null) continue;

                    string presetAspect = ScreenUtils.GetAspectLabel(p.resolution);

                    // Se aspecto é válido, só compara com presets do mesmo aspect ratio
                    if (currentAspect != "Invalid" && presetAspect != currentAspect)
                        continue;

                    float dist = Vector2.Distance(currentRes, p.resolution);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestPreset = p;
                    }
                }

                // Se nenhum preset foi encontrado com o mesmo aspect, faz fallback para qualquer um
                if (closestPreset == null && currentAspect != "Invalid")
                {
                    foreach (var p in presets)
                    {
                        if (p.rectPreset == null) continue;

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
                    preset = closestPreset.rectPreset;
                }
            }

            if (preset == null)
            {
                preset = (width >= height) ? landscapePreset : portraitPreset;
            }

            Debug.Log("Applying preset resolution: " + preset.name);
            ApplyPreset(preset);
        }

        #endregion


    }
    [Serializable]
    public class RectPreset
    {
        [Min(0)] public Vector2Int resolution = Vector2Int.zero;
        [Tooltip("Use a empty Gameobject with RectTransform to copy that properties.")]
        public RectTransform rectPreset;

    }
}