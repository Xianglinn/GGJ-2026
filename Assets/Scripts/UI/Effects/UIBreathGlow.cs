using UnityEngine;
using UnityEngine.UI;

namespace UIEffects
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIBreathGlow : MonoBehaviour
    {
        [Header("Breathing Settings")]
        [SerializeField] private float speed = 2f;
        [SerializeField] private float minAlpha = 0.5f;
        [SerializeField] private float maxAlpha = 1.0f;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            if (_canvasGroup == null) return;

            // 使用 Sin 函数计算 Alpha 值
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
            _canvasGroup.alpha = alpha;
        }
    }
}
