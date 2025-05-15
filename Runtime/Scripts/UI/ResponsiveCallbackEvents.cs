using Concept.Core;
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
            ScreenMonitor.OnResolutionChanged += OnResolutionChanged;
        }

        private void OnDisable()
        {
            ScreenMonitor.OnResolutionChanged -= OnResolutionChanged;

        }

        private void Start()
        {
            OnResolutionChanged(Screen.width, Screen.height);
        }

        private void OnResolutionChanged(int width, int height)
        {
            if (width >= height) 
                OnLandscapeOrientation?.Invoke();
            else
                OnPortraitOrientation?.Invoke();

        }
    }

}