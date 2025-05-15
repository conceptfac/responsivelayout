using Concept.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Concept.UI
{

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(ContentSizeFitter))]
    public class ExpandableText : LayoutElement
    {
        private RectTransform _rectTransform;

        private TextMeshProUGUI _textMesh;
        private ContentSizeFitter _sizeFitter;
        private Button _button;


        public bool isCollapsed = true;
        public float m_minHeight = 64f;
        [SerializeField] RectTransform[] _forceRebuildTransforms;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            minHeight = m_minHeight;
//            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
//            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponentInParent<RectTransform>());

        }
#endif

        protected override void Awake()
        {
            base.Awake();
            _rectTransform = GetComponent<RectTransform>();
            _textMesh = GetComponent<TextMeshProUGUI>();
            _sizeFitter = GetComponent<ContentSizeFitter>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_button) return; 
            minHeight = m_minHeight;
            _button = transform.Find("BT_Colapse")?.GetComponent<Button>();
            if (!_button)
                _button = new GameObject("BT_Colapse", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button)).GetComponent<Button>();

                RectTransform rect = _button.GetComponent<RectTransform>();
                _button.transform.SetParent(transform, false);
                // Configurar anchoring stretch total
                rect.anchorMin = Vector2.zero;         // (0, 0)
                rect.anchorMax = Vector2.one;          // (1, 1)
                rect.offsetMin = Vector2.zero;         // Left/Bottom padding
                rect.offsetMax = Vector2.zero;
                _button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                _button.onClick.AddListener(OnClick);
        }

        protected override void Start()
        {
            base.Start();
            AsyncOperationExtensions.CallDelayedAction(() => Rebuild(),100);
        }

        private void OnClick()
        {
            if (_sizeFitter.verticalFit == ContentSizeFitter.FitMode.MinSize)
            {
                _sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            else
            {
                _sizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
            }


            Rebuild();
        }

        private void Rebuild()
        {
            
            if (this == null) return;
            _rectTransform.ForceUpdateRectTransforms();
            RectTransform _parentRectTransform = GetComponentInParent<RectTransform>();
            _parentRectTransform.ForceUpdateRectTransforms();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_parentRectTransform);

            foreach (var rect in _forceRebuildTransforms)
            {
                rect.ForceUpdateRectTransforms();
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
        }
    }

}