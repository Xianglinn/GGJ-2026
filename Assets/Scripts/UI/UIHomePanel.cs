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
<<<<<<< Updated upstream
    [SerializeField] private Button achievementButton;
=======
    [SerializeField] private Button achievementBtn;
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
        if (achievementButton != null)
        {
            achievementButton.onClick.AddListener(OnAchievementButtonClicked);
=======
        if (achievementBtn != null)
        {
            achievementBtn.onClick.AddListener(OnAchievementBtnClicked);
        }
        else
        {
            // 尝试查找 UIAchivementBtn (注意拼写: User Request specified "UIAchivementBtn")
            var achBtnObj = GameObject.Find("UIAchivementBtn"); // User typo in request? Keeping "UIAchivementBtn" as per request just in case, but standard is Achievement
            if (achBtnObj == null) achBtnObj = GameObject.Find("UIAchievementBtn"); // Try correct spelling too

            if (achBtnObj != null)
            {
                achievementBtn = achBtnObj.GetComponent<Button>();
                if (achievementBtn != null)
                {
                    achievementBtn.onClick.AddListener(OnAchievementBtnClicked);
                }
            }
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
    private void OnAchievementButtonClicked()
    {
        Debug.Log("[UIHomePanel] Achievement Button Clicked");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPanel<UIAchievementPanel>();
=======
    private void OnAchievementBtnClicked()
    {
        Debug.Log("[UIHomePanel] Achievement Btn Clicked");
        if (UIManager.Instance != null)
        {
            // 如果面板未注册（可能是因为初始是 Disable 的，Awske 没跑），尝试手动查找并修正
            if (!UIManager.Instance.IsPanelRegistered<UIAchievementPanel>())
            {
                // 尝试查找场景中隐藏的面板
                // Note: Resources.FindObjectsOfTypeAll 可以找到隐藏的，但也会找到 Asset 里的 Prefab，所以需要过滤
                var allPanels = Resources.FindObjectsOfTypeAll<UIAchievementPanel>();
                foreach (var p in allPanels)
                {
                    // 确保是场景中的对象 (hideFlags 检查，或 editor 里的 check)
                    // 在运行时，场景对象的 asset 标记通常不同，或者简单检查 gameObject.scene
                    if (p.gameObject.scene.IsValid())
                    {
                        Debug.Log("[UIHomePanel] Found inactive UIAchievementPanel, manually registering and activating.");
                        p.gameObject.SetActive(true); // 激活它，触发 Awake (如果之前没触发)，或者手动 Register
                        if (!UIManager.Instance.IsPanelRegistered<UIAchievementPanel>())
                        {
                            UIManager.Instance.RegisterPanel(p); // 如果 Awake 还没跑或没注册成功，手动注册
                        }
                        break; // 假设只有一个
                    }
                }
            }

            // 显示成就面板，传递 null 数据进行初始化
            UIManager.Instance.ShowPanel<UIAchievementPanel, object>(null);
>>>>>>> Stashed changes
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
