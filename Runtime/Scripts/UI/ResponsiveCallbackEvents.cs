using Concept.Helpers;
using UnityEngine;

namespace Concept.UI
{

    [ExecuteAlways]
    public class ResponsiveCallbackEvents : MonoBehaviour
    {
        public OnLandscapeOrientationEvent OnLandscapeOrientation;
        public OnPortraitOrientationEvent OnPortraitOrientation;

        private void OnEnable()
        {
            ScreenUtils.OnResolutionChanged += OnResolutionChanged;
        }

        private void OnDisable()
        {
            ScreenUtils.OnResolutionChanged -= OnResolutionChanged;

        }

        private void Start()
        {
            OnResolutionChanged(Screen.width, Screen.height);
        }

        private void OnResolutionChanged(float width, float height)
        {
            if (width >= height) 
                OnLandscapeOrientation?.Invoke();
            else
                OnPortraitOrientation?.Invoke();

        }
    }

}