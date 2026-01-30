using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 示例测试面板，用于验证 UIManager 功能
/// </summary>
public class TestPanel : UIBasePanel<string>
{
    [Header("UI Elements")]
    [SerializeField]
    private Text titleText;

    [SerializeField]
    private Text contentText;

    [SerializeField]
    private Button closeButton;

    /// <summary>
    /// Unity 生命周期：Awake
    /// </summary>
    private void Awake()
    {
        // 绑定关闭按钮事件
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
    }

    /// <summary>
    /// 初始化面板数据
    /// </summary>
    public override void OnInitialize(string data)
    {
        base.OnInitialize(data);

        // 更新 UI 显示
        if (titleText != null)
        {
            titleText.text = "Test Panel";
        }

        if (contentText != null)
        {
            contentText.text = data ?? "No data provided";
        }
    }

    /// <summary>
    /// 关闭按钮点击事件
    /// </summary>
    private void OnCloseButtonClicked()
    {
        UIManager.Instance.HidePanel<TestPanel>();
    }
}
