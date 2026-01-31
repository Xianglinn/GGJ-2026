using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡地图 UI 面板
/// </summary>
public class UILevelMapPanel : UIBasePanel<object>
{
    [Header("UI References")]
    [SerializeField] private Button toScene1Btn;

    private void Awake()
    {
        // 自动注册
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UILevelMapPanel>())
        {
            UIManager.Instance.RegisterPanel(this);
        }

        if (toScene1Btn != null)
        {
            toScene1Btn.onClick.AddListener(OnToScene1BtnClicked);
        }
        else
        {
            // 尝试查找
            var btnObj = GameObject.Find("ToScene1Btn");
            if (btnObj != null) 
            {
                toScene1Btn = btnObj.GetComponent<Button>();
                toScene1Btn.onClick.AddListener(OnToScene1BtnClicked);
            }
        }
    }

    private void OnToScene1BtnClicked()
    {
        Debug.Log("[UILevelMapPanel] To Scene1 Clicked");
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.SwitchState(GameState.Home);
        }
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (toScene1Btn != null) toScene1Btn.onClick.RemoveListener(OnToScene1BtnClicked);
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UnregisterPanel<UILevelMapPanel>();
        }
    }
}
