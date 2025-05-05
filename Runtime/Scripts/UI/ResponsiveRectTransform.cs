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

        public RectPreset[] rectPresets; //Desenhar cada classe com o botão preview ao lado da propriedade preset

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (landscapePreset == null)
                landscapePreset = CreatePreset("LandscapePreset");
            if (portraitPreset == null)
                portraitPreset = CreatePreset("PortraitPreset");
        }
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
            ApplyPreset((Screen.width >= Screen.height) ? landscapePreset : portraitPreset);
        }

        public RectTransform CreatePreset(string name)
        {
            RectTransform preset = transform.Find(name) as RectTransform;
            if (preset == null)
            {


                preset = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
                preset.transform.SetParent(transform, false);
                preset.gameObject.SetActive(false);
            }
            return preset;
        }

            public void ClonePreset(RectTransform target)
            {
                ScreenUtils.CloneRectTransform(_rectTransform,target);
            }

            public void ApplyPreset(RectTransform rectPreset)
            {
                ScreenUtils.CloneRectTransform(rectPreset, _rectTransform);
                Debug.LogWarning(gameObject.name + " ApplyPreset: " + rectPreset);
            }

            #region Callback Methods
            private void OnResolutionChanged(float width, float height)
            {
            RectTransform preset = null;
            if (!forceLayoutByOrientation)
            {
                Vector2 res = new Vector2(width, height);
                preset = rectPresets.FirstOrDefault(p => p.resolution == res)?.rectPreset;
            }

                if(preset == null)
                {

                if (width >= height)
                    preset = landscapePreset;
                else
                    preset = portraitPreset;
                }

                ApplyPreset(preset);


        }
        #endregion


    }
        [Serializable]
        public class RectPreset
        {
            public Vector2 resolution = Vector2.zero;
            [Tooltip("Use a empty Gameobject with RectTransform to copy that properties.")]
            public RectTransform rectPreset;

        }
    }