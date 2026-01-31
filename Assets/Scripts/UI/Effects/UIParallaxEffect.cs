using UnityEngine;

namespace UIEffects
{
    public class UIParallaxEffect : MonoBehaviour
    {
        [Header("Parallax Settings")]
        [SerializeField] private Vector2 parallaxAmount = new Vector2(10f, 10f);
        [SerializeField] private float smoothing = 5f;

        private RectTransform _rectTransform;
        private Vector2 _startPosition;
        private Vector2 _targetPosition;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform != null)
            {
                _startPosition = _rectTransform.anchoredPosition;
            }
        }

        private void Update()
        {
            if (_rectTransform == null) return;

            // 获取鼠标相对于屏幕中心的偏移 (-0.5 ~ 0.5)
            Vector2 mousePos = Input.mousePosition;
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            Vector2 offset = (mousePos - screenCenter);
            
            // 归一化偏移 (-1 ~ 1)
            offset.x /= screenCenter.x;
            offset.y /= screenCenter.y;

            // 计算目标位置
            _targetPosition = _startPosition + new Vector2(offset.x * parallaxAmount.x, offset.y * parallaxAmount.y);

            // 平滑移动
            _rectTransform.anchoredPosition = Vector2.Lerp(_rectTransform.anchoredPosition, _targetPosition, Time.deltaTime * smoothing);
        }
    }
}
