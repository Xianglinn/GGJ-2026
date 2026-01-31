using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 首页 UI 面板：监听任意点击或按键，切换到序章状态
/// </summary>
public class UIHomePanel : UIBasePanel<object>, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private Text hintText;
    [SerializeField] private Button toScene5Btn;
    [SerializeField] private Button achievementButton;

    private void Awake()
    {
        // 自动注册到 UIManager
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UIHomePanel>())
        {
            UIManager.Instance.RegisterPanel(this);
            // UIManager.RegisterPanel 会调用 SetActive(false)，我们需要重新激活
            // 因为 UIHomePanel 需要一直激活来监听输入
            gameObject.SetActive(true);
        }

        if (toScene5Btn != null)
        {
            toScene5Btn.onClick.AddListener(OnToScene5BtnClicked);
        }
        else
        {
             // 尝试查找
            var btnObj = GameObject.Find("ToScene5Btn");
            if (btnObj != null) 
            {
                toScene5Btn = btnObj.GetComponent<Button>();
                toScene5Btn.onClick.AddListener(OnToScene5BtnClicked);
            }
        }

        if (achievementButton != null)
        {
            achievementButton.onClick.AddListener(OnAchievementButtonClicked);
        }

        Debug.Log("[UIHomePanel] Awake called");
    }
    
    private void Start()
    {
        Debug.Log($"[UIHomePanel] Start called - GameObject active: {gameObject.activeInHierarchy}");
    }

    public override void OnInitialize(object data)
    {
        base.OnInitialize(data);
        
        // 设置提示文本
        if (hintText != null)
        {
            hintText.text = "按任意键继续 / Press Any Key to Continue";
        }
    }
    
    private void Update()
    {
        // 检测任意键盘按键
        // 检测任意键盘按键，但排除鼠标点击（防止点击按钮时误触发）
        if (Input.anyKeyDown)
        {
            // 如果是鼠标按键，则忽略（交给 OnPointerClick 或 按钮自身的事件处理）
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                return;
            }

            Debug.Log("[UIHomePanel] Key detected! Triggering transition...");
            TriggerSceneTransition();
        }
    }

    private void OnToScene5BtnClicked()
    {
        Debug.Log("[UIHomePanel] To Scene5 Clicked");
         if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.SwitchState(GameState.LevelMap);
        }
    }

    private void OnAchievementButtonClicked()
    {
        Debug.Log("[UIHomePanel] Achievement Button Clicked");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPanel<UIAchievementPanel>();
        }
    }

    /// <summary>
    /// 任意点击首页进入序章（场景2）
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        TriggerSceneTransition();
    }
    
    /// <summary>
    /// 统一场景切换逻辑，避免重复调用
    /// </summary>
    private void TriggerSceneTransition()
    {
        // 检查 GameFlowManager 是否存在且当前面板是激活的
        if (GameFlowManager.Instance != null && gameObject.activeInHierarchy)
        {
            Debug.Log("[UIHomePanel] Triggering scene transition to Prologue");
            GameFlowManager.Instance.SwitchState(GameState.Prologue);
        }
        else
        {
            Debug.LogWarning("[UIHomePanel] Cannot trigger transition - GameFlowManager null or panel inactive");
        }
    }
}
