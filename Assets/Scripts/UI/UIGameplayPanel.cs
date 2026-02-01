using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩法界面面板：对应 Scene3 的核心玩法
/// </summary>
public class UIGameplayPanel : UIBasePanel<object>
{
    public override CanvasType PanelCanvasType => CanvasType.SceneLocal;

    [Header("UI References")]
    [SerializeField] private Button toScene2Btn;

    private void Awake()
    {
        // 自动注册到 UIManager
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UIGameplayPanel>())
        {
            UIManager.Instance.RegisterPanel(this);
            Debug.Log("[UIGameplayPanel] Auto-registered to UIManager");
        }

        // 如果 Inspector 没指定，尝试按名称查找
        if (toScene2Btn == null)
        {
            var btnObj = GameObject.Find("ToScene2Panel");
            if (btnObj != null) toScene2Btn = btnObj.GetComponent<Button>();
        }

        // 绑定按钮事件
        if (toScene2Btn != null)
        {
            toScene2Btn.onClick.AddListener(OnToScene2ButtonClicked);
            Debug.Log("[UIGameplayPanel] toScene2Btn bound successfully");
        }
        else
        {
            Debug.LogWarning("[UIGameplayPanel] toScene2Btn not assigned and 'ToScene2Panel' not found in scene");
        }
    }

    public override void OnInitialize(object data)
    {
        base.OnInitialize(data);
        InventoryPanelSync panelSync = FindObjectOfType<InventoryPanelSync>(true);
        if(panelSync != null)
        {
            panelSync.LoadFromManager();
        }
        Debug.Log("[UIGameplayPanel] Initialized");
    }

    /// <summary>
    /// 切换回场景2
    /// </summary>
    private void OnToScene2ButtonClicked()
    {
        Debug.Log("[UIGameplayPanel] toScene2Btn clicked, switching to Prologue state");
        if (GameFlowManager.Instance != null)
        {
            InventoryPanelSync panelSync = FindObjectOfType<InventoryPanelSync>(true);
            if (panelSync != null)
            {
                panelSync.SaveToManager();
            }
            GameFlowManager.Instance.SwitchState(GameState.Prologue);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // 解绑按钮事件
        if (toScene2Btn != null)
        {
            toScene2Btn.onClick.RemoveListener(OnToScene2ButtonClicked);
        }

        // 从 UIManager 注销
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnregisterPanel<UIGameplayPanel>();
        }
    }
}
