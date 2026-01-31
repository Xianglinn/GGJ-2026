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

    // 悬浮提示不阻挡射线，防止闪烁
    public override bool BlocksRaycasts => false;

    public override void OnInitialize(string data)
    {
        base.OnInitialize(data);
        UpdateText(data);
    }

    private void Awake()
    {
        // 自动注册逻辑
        if (UIManager.Instance != null)
        {
            // 如果已经注册过（说明 Persistent Canvas 下已经有一个了）
            if (UIManager.Instance.IsPanelRegistered<UITooltipPanel>())
            {
                // 获取已注册的实例
                var registeredPanel = UIManager.Instance.GetPanel<UITooltipPanel>();
                
                // 如果已注册的不是自己，说明自己是重复的（场景重载产生的）
                if (registeredPanel != this)
                {
                    Debug.Log($"[UITooltipPanel] Duplicate detected. Destroying self. (InstanceID: {GetInstanceID()})");
                    Destroy(gameObject);
                    return; // 立即返回，不执行后续逻辑
                }
            }
            else
            {
                // 还没注册，说明我是第一个
                UIManager.Instance.RegisterPanel(this);
            }
        }

        // 自动获取组件
        if (backgroundRect == null)
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
        
        // 初始隐藏
        gameObject.SetActive(false);
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
