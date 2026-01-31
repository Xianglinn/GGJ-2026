using UnityEngine;

/// <summary>
/// 尾声 UI 面板：对应 Scene4
/// </summary>
public class UIEpiloguePanel : UIBasePanel<object>
{
    private void Awake()
    {
        // 自动注册到 UIManager
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UIEpiloguePanel>())
        {
            UIManager.Instance.RegisterPanel(this);
            Debug.Log("[UIEpiloguePanel] Auto-registered to UIManager");
        }
    }

    public override void OnInitialize(object data)
    {
        base.OnInitialize(data);
        Debug.Log("[UIEpiloguePanel] Initialized");
    }

    private void OnDestroy()
    {
        // 从 UIManager 注销
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnregisterPanel<UIEpiloguePanel>();
        }
    }
}
