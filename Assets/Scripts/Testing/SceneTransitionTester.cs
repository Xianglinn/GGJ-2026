using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换测试工具
/// 提供运行时场景切换的 GUI 测试界面
/// </summary>
public class SceneTransitionTester : MonoBehaviour
{
    [Header("测试设置")]
    [Tooltip("是否显示测试 GUI")]
    public bool showTestGUI = true;

    [Tooltip("GUI 显示位置")]
    public Vector2 guiPosition = new Vector2(220, 10);

    [Header("快捷键设置")]
    [Tooltip("加载下一个场景的快捷键")]
    public KeyCode nextSceneKey = KeyCode.N;

    [Tooltip("加载上一个场景的快捷键")]
    public KeyCode previousSceneKey = KeyCode.P;

    [Tooltip("重新加载当前场景的快捷键")]
    public KeyCode reloadSceneKey = KeyCode.R;

    [Tooltip("显示场景信息的快捷键")]
    public KeyCode sceneInfoKey = KeyCode.I;

    private void Update()
    {
        // 快捷键处理
        if (Input.GetKeyDown(nextSceneKey))
        {
            LoadNextScene();
        }

        if (Input.GetKeyDown(previousSceneKey))
        {
            LoadPreviousScene();
        }

        if (Input.GetKeyDown(reloadSceneKey))
        {
            ReloadCurrentScene();
        }

        if (Input.GetKeyDown(sceneInfoKey))
        {
            ShowSceneInfo();
        }
    }

    private void OnGUI()
    {
        if (!showTestGUI) return;

        // 创建测试 GUI 区域
        GUILayout.BeginArea(new Rect(guiPosition.x, guiPosition.y, 250, 300));
        GUILayout.BeginVertical("box");

        // 标题
        GUILayout.Label("场景切换测试", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, fontSize = 14 });
        GUILayout.Space(5);

        // 当前场景信息
        Scene currentScene = SceneManager.GetActiveScene();
        GUILayout.Label($"当前场景: {currentScene.name}");
        GUILayout.Label($"场景索引: {currentScene.buildIndex}");
        GUILayout.Label($"总场景数: {SceneManager.sceneCountInBuildSettings}");
        GUILayout.Space(10);

        // 管理器状态
        GUILayout.Label("管理器状态:", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        DrawManagerStatus("UIManager", UIManager.Instance != null);
        DrawManagerStatus("DialogueManager", DialogueManager.Instance != null);
        DrawManagerStatus("ResourceManager", ResourceManager.Instance != null);
        DrawManagerStatus("AudioManager", AudioManager.Instance != null);
        DrawManagerStatus("DataManager", DataManager.Instance != null);
        DrawManagerStatus("GameFlowManager", GameFlowManager.Instance != null);
        DrawManagerStatus("SceneTransitionManager", SceneTransitionManager.Instance != null);
        GUILayout.Space(10);

        // 场景切换按钮
        GUILayout.Label("场景切换:", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });

        if (GUILayout.Button($"下一个场景 [{nextSceneKey}]"))
        {
            LoadNextScene();
        }

        if (GUILayout.Button($"上一个场景 [{previousSceneKey}]"))
        {
            LoadPreviousScene();
        }

        if (GUILayout.Button($"重新加载 [{reloadSceneKey}]"))
        {
            ReloadCurrentScene();
        }

        if (GUILayout.Button($"场景信息 [{sceneInfoKey}]"))
        {
            ShowSceneInfo();
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    /// <summary>
    /// 绘制管理器状态
    /// </summary>
    private void DrawManagerStatus(string managerName, bool isActive)
    {
        string status = isActive ? "✓" : "✗";
        Color color = isActive ? Color.green : Color.red;
        
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = color;
        
        GUILayout.Label($"  {status} {managerName}", style);
    }

    /// <summary>
    /// 加载下一个场景
    /// </summary>
    public void LoadNextScene()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadNextScene();
        }
        else
        {
            Debug.LogError("[SceneTransitionTester] SceneTransitionManager not found!");
        }
    }

    /// <summary>
    /// 加载上一个场景
    /// </summary>
    public void LoadPreviousScene()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadPreviousScene();
        }
        else
        {
            Debug.LogError("[SceneTransitionTester] SceneTransitionManager not found!");
        }
    }

    /// <summary>
    /// 重新加载当前场景
    /// </summary>
    public void ReloadCurrentScene()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.ReloadCurrentScene();
        }
        else
        {
            Debug.LogError("[SceneTransitionTester] SceneTransitionManager not found!");
        }
    }

    /// <summary>
    /// 显示场景信息
    /// </summary>
    public void ShowSceneInfo()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LogCurrentSceneInfo();
        }
        else
        {
            Scene currentScene = SceneManager.GetActiveScene();
            Debug.Log("========== 当前场景信息 ==========");
            Debug.Log($"场景名称: {currentScene.name}");
            Debug.Log($"场景索引: {currentScene.buildIndex}");
            Debug.Log($"场景路径: {currentScene.path}");
            Debug.Log("================================");
        }

        // 同时显示管理器状态
        ShowManagersStatus();
    }

    /// <summary>
    /// 显示所有管理器的状态
    /// </summary>
    public void ShowManagersStatus()
    {
        Debug.Log("========== 管理器状态 ==========");
        Debug.Log($"UIManager: {(UIManager.Instance != null ? "✓ 活跃" : "✗ 未找到")}");
        Debug.Log($"DialogueManager: {(DialogueManager.Instance != null ? "✓ 活跃" : "✗ 未找到")}");
        Debug.Log($"ResourceManager: {(ResourceManager.Instance != null ? "✓ 活跃" : "✗ 未找到")}");
        Debug.Log($"AudioManager: {(AudioManager.Instance != null ? "✓ 活跃" : "✗ 未找到")}");
        Debug.Log($"DataManager: {(DataManager.Instance != null ? "✓ 活跃" : "✗ 未找到")}");
        Debug.Log($"GameFlowManager: {(GameFlowManager.Instance != null ? "✓ 活跃" : "✗ 未找到")}");
        Debug.Log($"SceneTransitionManager: {(SceneTransitionManager.Instance != null ? "✓ 活跃" : "✗ 未找到")}");
        Debug.Log("==============================");
    }

    /// <summary>
    /// 加载指定场景（用于外部调用）
    /// </summary>
    [ContextMenu("加载 Scene2")]
    public void LoadScene2()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("Scene2");
        }
    }

    /// <summary>
    /// 加载指定场景（用于外部调用）
    /// </summary>
    [ContextMenu("加载 Scene1")]
    public void LoadScene1()
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene("Scene1");
        }
    }
}
