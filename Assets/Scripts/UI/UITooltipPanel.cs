using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 悬浮提示面板
/// 显示物品描述，跟随鼠标移动
/// </summary>
public class UITooltipPanel : UIBasePanel<string>
{
    [Header("UI Components")]
    [SerializeField] private Text descriptionText;
    [SerializeField] private RectTransform backgroundRect;
    [SerializeField] private RectTransform panelRect;

    [Header("Settings")]
    [SerializeField] private Vector2 offset = new Vector2(15f, -15f);
    [SerializeField] private float padding = 10f;

    // 确保显示在 Persistent Canvas 最顶层
    public override CanvasType PanelCanvasType => CanvasType.Persistent;

    public override void OnInitialize(string data)
    {
        base.OnInitialize(data);
        UpdateText(data);
    }

    private void Awake()
    {
        // 自动注册
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RegisterPanel(this);
        }

        // 自动获取组件
        if (backgroundRect == null)
            backgroundRect = GetComponent<RectTransform>(); // 自身或者是子对象，这里假设背景就是面板自身或者需要手动指定。
            // 修正：通常背景是 Image 组件所在的节点。假如面板本身就有 Image，那就是自身。
            // 让我们宽容一点：
            if (GetComponent<Image>() != null) backgroundRect = GetComponent<RectTransform>();
        
        if (descriptionText == null)
            descriptionText = GetComponentInChildren<Text>();

        // 确保 Sort Order
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 999;
        }

        // 防止阻挡射线导致 Flicker
        var cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }

    private void Update()
    {
        if (isVisible)
        {
            FollowMouse();
        }
    }

    private void FollowMouse()
    {
        Vector2 mousePosition = Input.mousePosition;
        
        // 简单的跟随，加上偏移量
        // 注意：如果在 Overlay 模式下，transform.position 直接对应屏幕坐标
        transform.position = mousePosition + offset;
        
        // 进阶：屏幕边缘检测防止超出（可选，暂不实现以保持简洁）
    }

    private void UpdateText(string text)
    {
        if (descriptionText != null)
        {
            descriptionText.text = text;
            
            // 自动调整背景大小（假设使用了 ContentSizeFitter 还是手动控制？）
            // 如果使用 Unity 原生 Layout 组件 (ContentSizeFitter + VerticalLayoutGroup)，这里其实不需要手动调整
            // 但为了保险，我们可以强制刷新一下 Layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundRect);
        }
    }
    
    protected override void OnShowAnimationStart()
    {
        // 确保打开时位置正确，不闪烁
        FollowMouse();
    }
}
