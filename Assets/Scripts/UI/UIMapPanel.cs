using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 地图 UI 面板：负责关卡选择和回到首页
/// </summary>
public class UIMapPanel : UIBasePanel<object>
{
    [Header("按钮引用")]
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button backHomeButton;

    [Header("提示弹窗（可选）")]
    [SerializeField] private string level2ComingSoonText = "敬请期待";

    private void Awake()
    {
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UIMapPanel>())
        {
            UIManager.Instance.RegisterPanel(this);
        }

        if (level1Button != null)
            level1Button.onClick.AddListener(OnLevel1Clicked);

        if (level2Button != null)
            level2Button.onClick.AddListener(OnLevel2Clicked);

        if (backHomeButton != null)
            backHomeButton.onClick.AddListener(OnBackHomeClicked);
    }

    public override void OnInitialize(object data)
    {
        base.OnInitialize(data);
    }

    private void OnLevel1Clicked()
    {
        // 进入核心玩法（场景2/3）
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.SwitchState(GameState.Gameplay);
        }
    }

    private void OnLevel2Clicked()
    {
        // 显示“敬请期待”文本弹窗，这里复用 TestPanel
        if (UIManager.Instance != null)
        {
            if (!UIManager.Instance.IsPanelRegistered<TestPanel>())
            {
                // 试图在场景中查找已放置的 TestPanel
                TestPanel panel = Object.FindObjectOfType<TestPanel>(true);
                if (panel != null)
                {
                    UIManager.Instance.RegisterPanel(panel);
                }
            }

            if (UIManager.Instance.IsPanelRegistered<TestPanel>())
            {
                UIManager.Instance.ShowPanel<TestPanel, string>(level2ComingSoonText);
            }
            else
            {
                Debug.LogWarning("[UIMapPanel] 未找到用于显示提示的 TestPanel，无法弹出 \"敬请期待\" 窗口。");
            }
        }
    }

    private void OnBackHomeClicked()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.SwitchState(GameState.Home);
        }
    }
}
