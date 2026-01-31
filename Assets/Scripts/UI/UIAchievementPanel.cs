using UnityEngine;
using UnityEngine.UI;

/// <summary>
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
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UIAchievementPanel>())
        {
            UIManager.Instance.RegisterPanel(this);
        }

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
    }
}
