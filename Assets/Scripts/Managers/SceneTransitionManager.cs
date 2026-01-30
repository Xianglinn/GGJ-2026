using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换管理器
/// 负责场景加载、切换和过渡效果
/// </summary>
public class SceneTransitionManager : MonoSingleton<SceneTransitionManager>
{
    [Header("场景切换设置")]
    [Tooltip("切换前延迟时间（秒）")]
    public float transitionDelay = 0.5f;

    [Tooltip("是否在切换时显示加载界面")]
    public bool showLoadingScreen = false;

    private bool _isTransitioning = false;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        Debug.Log("[SceneTransitionManager] Initialized successfully.");
    }

    /// <summary>
    /// 加载场景（通过场景名称）
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    public void LoadScene(string sceneName)
    {
        if (_isTransitioning)
        {
            Debug.LogWarning($"[SceneTransitionManager] Already transitioning to another scene. Ignoring request to load '{sceneName}'.");
            return;
        }

        Debug.Log($"[SceneTransitionManager] Loading scene: {sceneName}");
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    /// <summary>
    /// 加载场景（通过场景索引）
    /// </summary>
    /// <param name="sceneIndex">场景索引</param>
    public void LoadScene(int sceneIndex)
    {
        if (_isTransitioning)
        {
            Debug.LogWarning($"[SceneTransitionManager] Already transitioning to another scene. Ignoring request to load scene index {sceneIndex}.");
            return;
        }

        Debug.Log($"[SceneTransitionManager] Loading scene by index: {sceneIndex}");
        StartCoroutine(LoadSceneByIndexCoroutine(sceneIndex));
    }

    /// <summary>
    /// 重新加载当前场景
    /// </summary>
    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log($"[SceneTransitionManager] Reloading current scene: {currentScene.name}");
        LoadScene(currentScene.name);
    }

    /// <summary>
    /// 加载下一个场景（按 Build Settings 顺序）
    /// </summary>
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning($"[SceneTransitionManager] No next scene available. Current index: {currentSceneIndex}, Total scenes: {SceneManager.sceneCountInBuildSettings}");
            return;
        }

        Debug.Log($"[SceneTransitionManager] Loading next scene: {nextSceneIndex}");
        LoadScene(nextSceneIndex);
    }

    /// <summary>
    /// 加载上一个场景（按 Build Settings 顺序）
    /// </summary>
    public void LoadPreviousScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int previousSceneIndex = currentSceneIndex - 1;

        if (previousSceneIndex < 0)
        {
            Debug.LogWarning($"[SceneTransitionManager] No previous scene available. Current index: {currentSceneIndex}");
            return;
        }

        Debug.Log($"[SceneTransitionManager] Loading previous scene: {previousSceneIndex}");
        LoadScene(previousSceneIndex);
    }

    /// <summary>
    /// 场景加载协程（通过名称）
    /// </summary>
    private System.Collections.IEnumerator LoadSceneCoroutine(string sceneName)
    {
        _isTransitioning = true;

        // 等待过渡延迟
        if (transitionDelay > 0)
        {
            yield return new WaitForSeconds(transitionDelay);
        }

        // 显示加载界面（如果启用）
        if (showLoadingScreen)
        {
            // TODO: 显示加载界面 UI
        }

        // 加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"[SceneTransitionManager] Loading progress: {progress * 100:F0}%");
            yield return null;
        }

        Debug.Log($"[SceneTransitionManager] Scene '{sceneName}' loaded successfully.");
        _isTransitioning = false;
    }

    /// <summary>
    /// 场景加载协程（通过索引）
    /// </summary>
    private System.Collections.IEnumerator LoadSceneByIndexCoroutine(int sceneIndex)
    {
        _isTransitioning = true;

        // 等待过渡延迟
        if (transitionDelay > 0)
        {
            yield return new WaitForSeconds(transitionDelay);
        }

        // 显示加载界面（如果启用）
        if (showLoadingScreen)
        {
            // TODO: 显示加载界面 UI
        }

        // 加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = true;

        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"[SceneTransitionManager] Loading progress: {progress * 100:F0}%");
            yield return null;
        }

        Debug.Log($"[SceneTransitionManager] Scene index {sceneIndex} loaded successfully.");
        _isTransitioning = false;
    }

    /// <summary>
    /// 获取当前场景信息
    /// </summary>
    public void LogCurrentSceneInfo()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log("========== 当前场景信息 ==========");
        Debug.Log($"场景名称: {currentScene.name}");
        Debug.Log($"场景索引: {currentScene.buildIndex}");
        Debug.Log($"场景路径: {currentScene.path}");
        Debug.Log($"场景已加载: {currentScene.isLoaded}");
        Debug.Log($"Build Settings 中的场景总数: {SceneManager.sceneCountInBuildSettings}");
        Debug.Log("================================");
    }
}
