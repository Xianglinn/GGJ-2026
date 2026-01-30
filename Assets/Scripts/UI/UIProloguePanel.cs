using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 序章 UI 面板：对应 Scene2 的开场剧情和寻找原材料
/// </summary>
public class UIProloguePanel : UIBasePanel<object>
{
    [Header("UI References")]
    [SerializeField] private Text titleText;
    [SerializeField] private Button startButton;

    private void Awake()
    {
        // 自动注册到 UIManager
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UIProloguePanel>())
        {
            UIManager.Instance.RegisterPanel(this);
            Debug.Log("[UIProloguePanel] Registered to UIManager");
        }

        // 绑定按钮事件（如果有）
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
    }

    public override void OnInitialize(object data)
    {
        base.OnInitialize(data);

        // 初始化UI显示
        if (titleText != null)
        {
            titleText.text = "序章：寻找原材料";
        }

        Debug.Log("[UIProloguePanel] Initialized");
    }

    protected override void OnShowAnimationStart()
    {
        base.OnShowAnimationStart();
        Debug.Log("[UIProloguePanel] Show animation started");
    }

    protected override void OnHideAnimationStart()
    {
        base.OnHideAnimationStart();
        Debug.Log("[UIProloguePanel] Hide animation started");
    }

    /// <summary>
    /// 开始剧情按钮点击事件
    /// </summary>
    private void OnStartButtonClicked()
    {
        Debug.Log("[UIProloguePanel] Start button clicked");
        // TODO: 开始剧情或其他交互逻辑
    }

    private void OnDestroy()
    {
        // 解绑按钮事件
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        // 从 UIManager 注销
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnregisterPanel<UIProloguePanel>();
        }
    }
}
