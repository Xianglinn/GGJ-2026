using UnityEngine;
using UnityEngine.UI;

/// <summary>
<<<<<<< Updated upstream
/// 成就展示面板
/// </summary>
public class UIAchievementPanel : UIBasePanel<object>
{
    // 配置为 SceneLocal (因为只在 Home 场景显示，或者 Persistent 也可以，Persistent 可以在任何地方打开)
    // 既然是从 Scene 1 的 Home 按钮打开，且属于全局系统，Persistent 比较合适，
    // 这样可以在游戏中任意时刻查看(如果有需求的话)。
    public override CanvasType PanelCanvasType => CanvasType.Persistent;

    [Header("UI References")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Text achievementText; // 简单展示，实际可以是 List
    
    // 模拟成就数据
    private bool isAchievementUnlocked = false;

    private void Awake()
    {
        // 自动注册
=======
/// 成就面板
/// </summary>
public class UIAchievementPanel : UIBasePanel<object>
{
    [Header("UI References")]
    [SerializeField] private Button closeBtn;

    private void Awake()
    {
        // 自动注册到 UIManager
>>>>>>> Stashed changes
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UIAchievementPanel>())
        {
            UIManager.Instance.RegisterPanel(this);
        }

<<<<<<< Updated upstream
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
    }

    public override void OnInitialize(object data)
    {
        base.OnInitialize(data);
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (achievementText != null)
        {
            // 这里可以连接真实的成就系统数据
            // 目前使用简单的模拟状态
            string status = isAchievementUnlocked ? "<color=green>UNLOCKED</color>" : "<color=red>LOCKED</color>";
            achievementText.text = $"Adventure Begins: {status}\n\n(More achievements coming soon...)";
        }
    }

    private void OnCloseButtonClicked()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HidePanel<UIAchievementPanel>();
        }
    }

    // 供外部测试改变状态的方法
    public void UnlockAchievement()
    {
        isAchievementUnlocked = true;
        RefreshUI();
=======
        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(OnCloseBtnClicked);
        }
        else
        {
            // 尝试查找 CloseBtn 子物体
            var btnTransform = transform.Find("CloseBtn");
            if (btnTransform != null)
            {
                closeBtn = btnTransform.GetComponent<Button>();
                if (closeBtn != null)
                {
                    closeBtn.onClick.AddListener(OnCloseBtnClicked);
                }
            }
        }
        
        // 确保面板初始时是隐藏的（如果场景中默认开启的话）
        // UIManager RegisterPanel 会调用 SetActive(false)，但如果是手动 active 的话，这里双重保险
        // 实际上 UIManager 一般会管理好
    }

    private void OnCloseBtnClicked()
    {
        Hide();
        // 如果需要通知 UIManager 也可以调用 UIManager.Instance.HidePanel<UIAchievementPanel>();
        // 但 UIBasePanel.Hide() 只负责自身隐藏逻辑，UIManager 状态同步可能需要
        // 查看 UIManager Update/Sync? UIManager 似乎是主动调用的。
        // 为了保持 UIManager 栈同步，最好通过 UIManager 关闭，或者 override Hide 调用 UIManager.HidePanel
    }

    public override void Show()
    {
        base.Show();
        // 这里可以加载成就数据
        RefreshAchievements();
    }
    
    private void RefreshAchievements()
    {
        // TODO: 加载和显示成就列表
        Debug.Log("[UIAchievementPanel] Refreshing achievements...");
>>>>>>> Stashed changes
    }
}
