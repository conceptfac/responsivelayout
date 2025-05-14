using Concept.Core;
using Concept.Helpers;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Concept.UI
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class ResponsiveRectTransform : MonoBehaviour
    {
        private RectTransform _rectTransform;
        public RectPreset[] presets;

#if UNITY_EDITOR
        public int _activePreset;
#endif

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
            OnResolutionChanged(Screen.width, Screen.height);
            _rectTransform.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
        }


        private void OnDestroy()
        {
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
            if (presets == null) return;

            RectPreset preset = null;
            Vector2Int currentRes = new Vector2Int(width, height);


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
                    preset = closestPreset;
                }


            if (preset != null)
            {
                //Debug.Log("Applying preset resolution: " + preset.resolution);
#if UNITY_EDITOR
                _activePreset = Array.IndexOf(presets, preset);
#endif
                ApplyPreset(preset.rectPreset);
            }

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