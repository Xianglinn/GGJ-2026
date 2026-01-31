using UnityEngine;
using UnityEngine.UI;

public class BackgroundLayers : MonoBehaviour
{
    [Header("Elements")]
    public RectTransform leftLine;
    public RectTransform rightLine;
    public RectTransform flower1;
    public RectTransform flower2;
    public RectTransform blingBling;

    [Header("Settings")]
    public float lineSwaySpeed = 1f;
    public float lineSwayAmount = 5f;
    public float lineTiltAmount = 2f;
    
    [Space(10)]
    public float flowerRotateSpeed = 20f;
    [Tooltip("如果像齿轮一样咬合，建议设为 true")]
    public bool reverseFlower2Direction = true; 
    
    [Space(10)]
    public float blingBlinkSpeed = 2f;
    public float blingBobSpeed = 1.5f;
    public float blingBobAmount = 5f;

    private Vector2 _leftLineStartPos;
    private Vector2 _rightLineStartPos;
    private Vector2 _blingStartPos;
    private CanvasGroup _blingCanvasGroup;
    private Image _blingImage;

    void Start()
    {
        // 自动查找未赋值的引用
        if (leftLine == null) leftLine = FindChildRect("leftline");
        if (rightLine == null) rightLine = FindChildRect("rightline");
        if (flower1 == null) flower1 = FindChildRect("flower1");
        if (flower2 == null) flower2 = FindChildRect("flower2");
        if (blingBling == null) blingBling = FindChildRect("blingbling");

        if (leftLine) _leftLineStartPos = leftLine.anchoredPosition;
        if (rightLine) _rightLineStartPos = rightLine.anchoredPosition;
        
        // 【关键】修正花朵中心点为 (0.5, 0.5) 确保原地自转
        if (flower1) SetPivot(flower1, new Vector2(0.5f, 0.5f));
        if (flower2) SetPivot(flower2, new Vector2(0.5f, 0.5f));
        
        if (blingBling)
        {
            _blingStartPos = blingBling.anchoredPosition;
            _blingCanvasGroup = blingBling.GetComponent<CanvasGroup>();
            if (_blingCanvasGroup == null) _blingImage = blingBling.GetComponent<Image>();
        }
    }

    // 该方法能改变 Pivot 同时保持 UI 视觉位置不变
    private void SetPivot(RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;
        
        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x * rectTransform.localScale.x, deltaPivot.y * size.y * rectTransform.localScale.y);
        
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

    private RectTransform FindChildRect(string name)
    {
        var t = transform.Find(name);
        return t != null ? t.GetComponent<RectTransform>() : null;
    }

    void Update()
    {
        float time = Time.time;

        // 1. 线条摆动逻辑 (保持不变)
        UpdateLines(time);

        // 2. 花朵旋转逻辑 (齿轮式轮转)
        RotateFlowers();

        // 3. 闪烁逻辑 (保持不变)
        UpdateBling(time);
    }

    private void RotateFlowers()
    {
        if (flower1)
        {
            // 向左转 (逆时针)
            flower1.Rotate(0, 0, flowerRotateSpeed * Time.deltaTime, Space.Self);
        }

        if (flower2)
        {
            // 如果 reverse 为 true，则向右转 (顺时针)，模拟齿轮咬合
            float direction = reverseFlower2Direction ? -1f : 1f;
            // 乘以 0.8f 保持你原有的错位感，或者删掉它让转速同步
            flower2.Rotate(0, 0, flowerRotateSpeed * 0.8f * direction * Time.deltaTime, Space.Self);
        }
    }

    private void UpdateLines(float time)
    {
        if (leftLine)
        {
            float sway = Mathf.Sin(time * lineSwaySpeed);
            leftLine.anchoredPosition = _leftLineStartPos + new Vector2(sway * lineSwayAmount - 5f, 0);
            leftLine.localRotation = Quaternion.Euler(0, 0, sway * lineTiltAmount + 2f);
        }
        if (rightLine)
        {
            float sway = Mathf.Sin(time * lineSwaySpeed + 1f);
            rightLine.anchoredPosition = _rightLineStartPos + new Vector2(sway * lineSwayAmount + 5f, 0);
            rightLine.localRotation = Quaternion.Euler(0, 0, sway * (-lineTiltAmount) - 2f);
        }
    }

    private void UpdateBling(float time)
    {
        if (blingBling)
        {
            float bob = Mathf.Sin(time * blingBobSpeed);
            blingBling.anchoredPosition = _blingStartPos + new Vector2(0, bob * blingBobAmount);
            float alpha = (Mathf.Sin(time * blingBlinkSpeed) + 1f) / 2f * 0.5f + 0.5f;
            if (_blingCanvasGroup) _blingCanvasGroup.alpha = alpha;
            else if (_blingImage)
            {
                Color c = _blingImage.color;
                c.a = alpha;
                _blingImage.color = c;
            }
        }
    }
}