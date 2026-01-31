using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏状态枚举
/// </summary>
public enum GameState
{
    Home,       // 场景1：首页
    Prologue,   // 场景2：开场剧情+寻找原材料 
    Gameplay,   // 场景3：面具拼图
    Epilogue,   // 场景4：尾声剧情
    LevelMap    // 场景5：关卡地图
}


/// <summary>
/// 全局游戏流程管理器
/// 负责游戏状态切换、场景加载和 UI 管理
/// </summary>
public class GameFlowManager : MonoSingleton<GameFlowManager>
{
    /// <summary>
    /// 当前游戏状态
    /// </summary>
    private GameState _currentState;

    /// <summary>
    /// 上一个游戏状态
    /// </summary>
    private GameState _previousState;

    /// <summary>
    /// 状态切换事件委托
    /// </summary>
    public delegate void OnStateChanged(GameState previousState, GameState newState);

    /// <summary>
    /// 状态切换事件
    /// </summary>
    public event OnStateChanged StateChanged;

    /// <summary>
    /// 获取当前游戏状态
    /// </summary>
    public GameState CurrentState => _currentState;

    /// <summary>
    /// 获取上一个游戏状态
    /// </summary>
    public GameState PreviousState => _previousState;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        
        // 初始化默认状态为 Home
        _currentState = GameState.Home;
        _previousState = GameState.Home;

        Debug.Log("[GameFlowManager] Initialized with default state: Home");
    }

    /// <summary>
    /// 切换游戏状态
    /// </summary>
    /// <param name="newState">新的游戏状态</param>
    public void SwitchState(GameState newState)
    {
        if (_currentState == newState)
        {
            Debug.LogWarning($"[GameFlowManager] Already in state: {newState}. Ignoring switch request.");
            return;
        }

        _previousState = _currentState;
        _currentState = newState;

        Debug.Log($"[GameFlowManager] Switching state: {_previousState} -> {_currentState}");

        // 触发状态切换事件
        StateChanged?.Invoke(_previousState, _currentState);

        // 执行状态切换逻辑
        OnStateEnter(newState);
    }

    /// <summary>
    /// 进入新状态时的处理逻辑
    /// </summary>
    /// <param name="state">新状态</param>
    private void OnStateEnter(GameState state)
    {
        switch (state)
        {
            case GameState.Home:
                HandleHomeState();
                break;
            case GameState.Prologue:
                HandlePrologueState();
                break;
            case GameState.Gameplay:
                HandleGameplayState();
                break;
            case GameState.Epilogue:
                HandleEpilogueState();
                break;
            case GameState.LevelMap:
                HandleLevelMapState();
                break;
            default:
                Debug.LogError($"[GameFlowManager] Unknown state: {state}");
                break;
        }
    }

    #region 状态处理方法

    private void HandleHomeState()
    {
        Debug.Log("[GameFlowManager] Entering Home state");
        
        // 加载 Scene1
        LoadScene("Scene1");
        
        // 场景加载完成后显示 UIHomePanel
        SceneManager.sceneLoaded += OnScene1Loaded;
    }
    
    private void OnScene1Loaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene1")
        {
            SceneManager.sceneLoaded -= OnScene1Loaded;
            
            // 隐藏所有其他面板
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideAllPanels();

                // 尝试查找并注册（如果尚未注册，可能是因为物体默认隐藏导致 Awake 未执行）
                if (!UIManager.Instance.IsPanelRegistered<UIHomePanel>())
                {
                    var panel = FindObjectOfType<UIHomePanel>(true); // true = include inactive
                    if (panel != null)
                    {
                        UIManager.Instance.RegisterPanel(panel);
                    }
                }
                
                // 显示首页面板
                if (UIManager.Instance.IsPanelRegistered<UIHomePanel>())
                {
                    UIManager.Instance.ShowPanel<UIHomePanel>();
                }
                else
                {
                    Debug.LogWarning("[GameFlowManager] UIHomePanel not registered. Please ensure it's in the scene.");
                }
            }
        }
    }

    private void HandlePrologueState()
    {
        Debug.Log("[GameFlowManager] Entering Prologue state");
        
        // 加载 Scene2
        LoadScene("Scene2");
        
        // 场景加载完成后显示 UIProloguePanel
        SceneManager.sceneLoaded += OnScene2Loaded;
    }
    
    private void OnScene2Loaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene2")
        {
            SceneManager.sceneLoaded -= OnScene2Loaded;
            
            // 隐藏所有其他面板
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideAllPanels();
                
                // 尝试查找并注册（如果尚未注册，可能是因为物体默认隐藏导致 Awake 未执行）
                if (!UIManager.Instance.IsPanelRegistered<UIProloguePanel>())
                {
                    var panel = FindObjectOfType<UIProloguePanel>(true); // true = include inactive
                    if (panel != null)
                    {
                        UIManager.Instance.RegisterPanel(panel);
                    }
                }

                // 显示序章面板
                if (UIManager.Instance.IsPanelRegistered<UIProloguePanel>())
                {
                    UIManager.Instance.ShowPanel<UIProloguePanel>();
                    
                    // 触发新手教程对话
                    CheckAndStartScene2Tutorial();
                }
            }
        }
    }

    /// <summary>
    /// 检查并开始 Scene2 新手教程
    /// </summary>
    private void CheckAndStartScene2Tutorial()
    {
        if (DataManager.Instance == null || DialogueManager.Instance == null) return;

        bool isTutorialCompleted = DataManager.Instance.GetStoryFlag("Scene2_Tutorial_Completed");
        if (!isTutorialCompleted)
        {
            Debug.Log("[GameFlowManager] Starting Scene2 Tutorial...");
            // 延迟启动对话，确保所有系统初始化完成
            StartCoroutine(StartTutorialDialogueDelayed());
        }
    }

    /// <summary>
    /// 延迟启动教程对话
    /// </summary>
    private System.Collections.IEnumerator StartTutorialDialogueDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        DialogueManager.Instance.LoadAndStartDialogue("Data/Dialogues/Dialogue_101");
        DataManager.Instance.SetStoryFlag("Scene2_Tutorial_Completed", true);
    }

    private void HandleGameplayState()
    {
        Debug.Log("[GameFlowManager] Entering Gameplay state");
        
        // 加载 Scene3
        LoadScene("Scene3");
        
        // 场景加载完成后处理逻辑
        SceneManager.sceneLoaded += OnScene3Loaded;
    }

  
    
    private void OnScene3Loaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene3")
        {
            SceneManager.sceneLoaded -= OnScene3Loaded;
            
            // 隐藏所有其他面板
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideAllPanels();
                
                // 尝试查找并注册（如果尚未注册，可能是因为物体默认隐藏导致 Awake 未执行）
                if (!UIManager.Instance.IsPanelRegistered<UIGameplayPanel>())
                {
                    var panel = FindObjectOfType<UIGameplayPanel>(true); // true = include inactive
                    if (panel != null)
                    {
                        UIManager.Instance.RegisterPanel(panel);
                    }
                }

                // 显示玩法面板
                if (UIManager.Instance.IsPanelRegistered<UIGameplayPanel>())
                {
                    UIManager.Instance.ShowPanel<UIGameplayPanel>();
                    
                }
            }
        }
    }

    private void HandleEpilogueState()
    {
        Debug.Log("[GameFlowManager] Entering Epilogue state");
        // TODO: 加载或激活尾声剧情场景
    }

    private void HandleLevelMapState()
    {
        Debug.Log("[GameFlowManager] Entering LevelMap state");
        LoadScene("Scene5");
        SceneManager.sceneLoaded += OnScene5Loaded;
    }

    private void OnScene5Loaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Scene5")
        {
            SceneManager.sceneLoaded -= OnScene5Loaded;
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideAllPanels();
                
                 if (!UIManager.Instance.IsPanelRegistered<UILevelMapPanel>())
                {
                    var panel = FindObjectOfType<UILevelMapPanel>(true);
                    if (panel != null)
                    {
                        UIManager.Instance.RegisterPanel(panel);
                    }
                }

                if (UIManager.Instance.IsPanelRegistered<UILevelMapPanel>())
                {
                    UIManager.Instance.ShowPanel<UILevelMapPanel>();
                }
            }
        }
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 加载场景（示例方法，可根据项目需求扩展）
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    private void LoadScene(string sceneName)
    {
        Debug.Log($"[GameFlowManager] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 异步加载场景（示例方法，可根据项目需求扩展）
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    private void LoadSceneAsync(string sceneName)
    {
        Debug.Log($"[GameFlowManager] Loading scene asynchronously: {sceneName}");
        SceneManager.LoadSceneAsync(sceneName);
    }

    #endregion
}
