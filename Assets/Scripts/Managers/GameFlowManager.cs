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
                
                // 显示序章面板
                if (UIManager.Instance.IsPanelRegistered<UIProloguePanel>())
                {
                    UIManager.Instance.ShowPanel<UIProloguePanel>();
                }
                else
                {
                    Debug.LogWarning("[GameFlowManager] UIProloguePanel not registered. Please ensure it's in the scene.");
                }
            }
        }
    }

    private void HandleGameplayState()
    {
        Debug.Log("[GameFlowManager] Entering Gameplay state");
        // TODO: 加载或激活核心玩法（场景2/3）
    }

    private void HandleEpilogueState()
    {
        Debug.Log("[GameFlowManager] Entering Epilogue state");
        // TODO: 加载或激活尾声剧情场景
    }

    private void HandleLevelMapState()
    {
        Debug.Log("[GameFlowManager] Entering LevelMap state");
        // TODO: 加载或激活关卡地图场景
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
