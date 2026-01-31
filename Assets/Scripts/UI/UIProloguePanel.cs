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
    [SerializeField] private Button toScene3Btn;

    private void Awake()
    {
        // 自动注册到 UIManager
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UIProloguePanel>())
        {
            UIManager.Instance.RegisterPanel(this);
            Debug.Log("[UIProloguePanel] Auto-registered to UIManager");
        }

        // 绑定按钮事件
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        // 如果 Inspector 没指定，尝试按名称查找
        if (toScene3Btn == null)
        {
            var btnObj = GameObject.Find("ToScene3Btn");
            if (btnObj != null) toScene3Btn = btnObj.GetComponent<Button>();
        }

        if (toScene3Btn != null)
        {
            toScene3Btn.onClick.AddListener(OnToScene3ButtonClicked);
            Debug.Log("[UIProloguePanel] toScene3Btn bound successfully");
        }
        else
        {
            Debug.LogWarning("[UIProloguePanel] toScene3Btn not assigned and 'ToScene3Btn' not found in scene");
        }
    }

    public override void OnInitialize(object data)
    {
        base.OnInitialize(data);

        InventoryPanelSync panelSync = FindObjectOfType<InventoryPanelSync>(true);
        if (panelSync != null)
        {
            panelSync.LoadFromManager();
        }

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

    /// <summary>
    /// 切换到场景3
    /// </summary>
    private void OnToScene3ButtonClicked()
    {
        Debug.Log("[UIProloguePanel] toScene3Btn clicked, switching to Gameplay state");
        if (GameFlowManager.Instance != null)
        {
            InventoryPanelSync panelSync = FindObjectOfType<InventoryPanelSync>(true);
            if(panelSync != null)
            {
                panelSync.SaveToManager();
            }
            GameFlowManager.Instance.SwitchState(GameState.Gameplay);
        }
        else
        {
            Debug.LogError("[UIProloguePanel] GameFlowManager.Instance is null!");
        }
    }

    private void OnDestroy()
    {
        // 解绑按钮事件
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        if (toScene3Btn != null)
        {
            toScene3Btn.onClick.RemoveListener(OnToScene3ButtonClicked);
        }

        // 从 UIManager 注销
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnregisterPanel<UIProloguePanel>();
        }
    }
}
