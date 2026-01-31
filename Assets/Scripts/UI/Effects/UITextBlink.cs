using UnityEngine;
using UnityEngine.UI;

namespace UIEffects
{
    public class UITextBlink : MonoBehaviour
    {
        [Header("Blink Settings")]
        [SerializeField] private float speed = 3f;
        [SerializeField] private float minAlpha = 0.0f;
        [SerializeField] private float maxAlpha = 1.0f;

        private Graphic _graphic; // 支持 Text 或 Image
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _graphic = GetComponent<Graphic>();
            }
        }

        private void Update()
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * speed) + 1f) / 2f);

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = alpha;
            }
            else if (_graphic != null)
            {
                Color color = _graphic.color;
                color.a = alpha;
                _graphic.color = color;
            }
        }
    }
}
