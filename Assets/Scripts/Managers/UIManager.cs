using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局 UI 管理器，负责 UI 面板的生命周期管理
/// 特性：单例模式、面板注册、显示/隐藏管理、面板栈管理
/// </summary>
public class UIManager : MonoSingleton<UIManager>
{
    /// <summary>
    /// 已注册的 UI 面板字典 (类型 -> 面板实例)
    /// </summary>
    private Dictionary<System.Type, MonoBehaviour> _registeredPanels = new Dictionary<System.Type, MonoBehaviour>();

    /// <summary>
    /// 当前显示的面板栈（用于导航和返回）
    /// </summary>
    private Stack<MonoBehaviour> _panelStack = new Stack<MonoBehaviour>();

    /// <summary>
    /// UI 根节点 Canvas
    /// </summary>
    private const string PersistentCanvasName = "[PersistentCanvas]";
    private const string SceneCanvasName = "[SceneCanvas]";

    private Canvas _persistentCanvas;
    private Canvas _sceneCanvas;

    /// <summary>
    /// 获取 Persistent Canvas (常驻)
    /// </summary>
    public Canvas PersistentCanvas
    {
        get
        {
            if (_persistentCanvas == null)
            {
                _persistentCanvas = GetOrCreateCanvas(PersistentCanvasName, 999, true);
            }
            return _persistentCanvas;
        }
    }

    /// <summary>
    /// 获取 Scene Canvas (场景本地)
    /// </summary>
    public Canvas SceneCanvas
    {
        get
        {
            if (_sceneCanvas == null)
            {
                _sceneCanvas = GetOrCreateCanvas(SceneCanvasName, 0, false);
            }
            return _sceneCanvas;
        }
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        // 初始化 Persistent Canvas
        var pCanvas = PersistentCanvas;
        
        // 检查并保护 EventSystem
        CheckAndInitEventSystem();

        // 注册场景加载事件，以便在切换场景时再次检查并清理多余的 EventSystem
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("[UIManager] Initialized successfully. Dual Canvas system ready.");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // 每次场景加载后，检查并清理多余的 EventSystem
        CheckAndInitEventSystem();
    }

    /// <summary>
    /// 获取或创建 Canvas
    /// </summary>
    private Canvas GetOrCreateCanvas(string canvasName, int sortOrder, bool isPersistent)
    {
        GameObject canvasObj = GameObject.Find(canvasName);
        if (canvasObj == null)
        {
            canvasObj = new GameObject(canvasName);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            if (isPersistent)
            {
                DontDestroyOnLoad(canvasObj);
            }
            Debug.Log($"[UIManager] Created new canvas: {canvasName}");
            return canvas;
        }
        else
        {
            Canvas canvas = canvasObj.GetComponent<Canvas>();
            if (canvas == null) canvas = canvasObj.AddComponent<Canvas>();
            
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortOrder;
            
            if (isPersistent)
            {
                DontDestroyOnLoad(canvasObj);
            }
            
            return canvas;
        }
    }

    /// <summary>
    /// 检查并初始化 EventSystem
    /// </summary>
    private void CheckAndInitEventSystem()
    {
        UnityEngine.EventSystems.EventSystem[] eventSystems = FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        
        UnityEngine.EventSystems.EventSystem globalEventSystem = null;

        // 1. 寻找或保留 Persistent Canvas 下的 EventSystem
        if (_persistentCanvas != null)
        {
            globalEventSystem = _persistentCanvas.GetComponentInChildren<UnityEngine.EventSystems.EventSystem>();
        }

        // 2. 如果没有，创建一个并挂载到 PersistentCanvas 下（或者作为独立对象）
        if (globalEventSystem == null)
        {
            // 简单的查找现有的是否有标记为 DontDestroyOnLoad 的
            foreach (var es in eventSystems)
            {
                if (es.gameObject.scene.name == "DontDestroyOnLoad")
                {
                    globalEventSystem = es;
                    break;
                }
            }
        }

        if (globalEventSystem == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            globalEventSystem = esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            
            // 挂载到 PersistentCanvas 下，或者独立 DontDestroyOnLoad
            // 为了整洁，我们可以放 PersistentCanvas 下，但 EventSystem 即使独立也没问题
            // 这里我们遵循 "Event System 唯一性：确保全剧只有一个 EventSystem 并将其置于 Persistent Canvas 所在的层级"
            if (_persistentCanvas != null)
            {
                esGO.transform.SetParent(_persistentCanvas.transform);
            }
            else
            {
                DontDestroyOnLoad(esGO);
            }
             Debug.Log("[UIManager] Created global EventSystem.");
        }
        else
        {
             // 确保它在 PersistentCanvas 层级或者是 DDOL
             if (globalEventSystem.transform.parent != _persistentCanvas.transform)
             {
                 globalEventSystem.transform.SetParent(_persistentCanvas.transform);
             }
        }

        // 3. 销毁所有其他的 EventSystem
        foreach (var es in eventSystems)
        {
            if (es != globalEventSystem)
            {
                Debug.LogWarning($"[UIManager] Destroying redundant EventSystem on {es.gameObject.name}");
                Destroy(es.gameObject);
            }
        }
    }

    /// <summary>
    /// 注册 UI 面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <param name="panel">面板实例</param>
    public void RegisterPanel<T>(T panel) where T : MonoBehaviour
    {
        System.Type panelType = typeof(T);

        if (_registeredPanels.ContainsKey(panelType))
        {
            MonoBehaviour oldPanel = _registeredPanels[panelType];
            if (oldPanel != null && oldPanel != panel)
            {
                Debug.LogWarning($"[UIManager] Duplicate Panel {panelType.Name} detected. Destroying old instance to prevent ghosts.");
                Destroy(oldPanel.gameObject);
            }
        }

        _registeredPanels[panelType] = panel;
        
        // 自动化归位逻辑
        CanvasType targetType = CanvasType.SceneLocal;
        var prop = panelType.GetProperty("PanelCanvasType");
        if (prop != null)
        {
            targetType = (CanvasType)prop.GetValue(panel);
        }
        else
        {
            // 备用：尝试读取字段或默认为 SceneLocal
        }

        Canvas targetCanvas = (targetType == CanvasType.Persistent) ? PersistentCanvas : SceneCanvas;
        if (targetCanvas != null)
        {
            Debug.Log($"[UIManager] Panel {panelType.Name} target canvas: {targetCanvas.name} (InstanceID: {targetCanvas.GetInstanceID()})");
            Debug.Log($"[UIManager] Panel {panelType.Name} current parent: {panel.transform.parent?.name ?? "null"}");

            if (panel.transform.parent != targetCanvas.transform)
            {
                string oldParentName = panel.transform.parent?.name ?? "null";
                panel.transform.SetParent(targetCanvas.transform, false);
                Debug.Log($"[UIManager] Auto-reparented {panelType.Name} from {oldParentName} to {targetCanvas.name}");
            }
            else
            {
                Debug.Log($"[UIManager] Panel {panelType.Name} is already a child of {targetCanvas.name}");
            }
        }
        else
        {
            Debug.LogError($"[UIManager] Target canvas for {panelType.Name} is NULL!");
        }

        panel.gameObject.SetActive(false); // 默认隐藏面板

        Debug.Log($"[UIManager] Registered panel: {panelType.Name} on {targetType} Canvas");
    }

    /// <summary>
    /// 注销 UI 面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public void UnregisterPanel<T>() where T : MonoBehaviour
    {
        System.Type panelType = typeof(T);

        if (_registeredPanels.ContainsKey(panelType))
        {
            _registeredPanels.Remove(panelType);
            Debug.Log($"[UIManager] Unregistered panel: {panelType.Name}");
        }
        else
        {
            Debug.LogWarning($"[UIManager] Panel {panelType.Name} is not registered.");
        }
    }

    /// <summary>
    /// 显示 UI 面板（泛型版本，支持传递数据）
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <typeparam name="TData">数据类型</typeparam>
    /// <param name="data">传递给面板的数据</param>
    public void ShowPanel<T, TData>(TData data) where T : UIBasePanel<TData> where TData : class
    {
        System.Type panelType = typeof(T);

        if (!_registeredPanels.TryGetValue(panelType, out MonoBehaviour panelObj) || panelObj == null)
        {
             // 如果找不到或者对象已销毁（可能是跨场景时引用丢失），尝试清理并报错
            if (_registeredPanels.ContainsKey(panelType))
            {
                 _registeredPanels.Remove(panelType);
            }
            Debug.LogError($"[UIManager] Panel {panelType.Name} is not registered or has been destroyed. Call RegisterPanel first.");
            return;
        }

        T panel = panelObj as T;
        if (panel == null)
        {
            Debug.LogError($"[UIManager] Panel {panelType.Name} could not be cast to type {typeof(T).Name}.");
            return;
        }

        // 先显示面板，再进行初始化
        // 这样可以确保 OnInitialize 中的 StartCoroutine 等逻辑能够正常运行
        panel.gameObject.SetActive(true);

        // 初始化并显示面板
        panel.OnInitialize(data);
        panel.Show();

        // 添加到面板栈
        if (!_panelStack.Contains(panel))
        {
            _panelStack.Push(panel);
        }

        Debug.Log($"[UIManager] Showing panel: {panelType.Name}");
    }

    /// <summary>
    /// 显示 UI 面板（无数据版本）
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public void ShowPanel<T>() where T : MonoBehaviour
    {
        System.Type panelType = typeof(T);

        if (!_registeredPanels.TryGetValue(panelType, out MonoBehaviour panelObj) || panelObj == null)
        {
             // 如果找不到或者对象已销毁（可能是跨场景时引用丢失），尝试清理并报错
            if (_registeredPanels.ContainsKey(panelType))
            {
                 _registeredPanels.Remove(panelType);
            }
            Debug.LogError($"[UIManager] Panel {panelType.Name} is not registered or has been destroyed. Call RegisterPanel first.");
            return;
        }

        panelObj.gameObject.SetActive(true);

        // 如果是 UIBasePanel，调用 Show 方法
        // 这会触发事件订阅和动画
        var basePanel = panelObj.GetComponent<MonoBehaviour>();
        var showMethod = basePanel.GetType().GetMethod("Show");
        if (showMethod != null)
        {
            showMethod.Invoke(basePanel, null);
        }

        // 添加到面板栈
        if (!_panelStack.Contains(panelObj))
        {
            _panelStack.Push(panelObj);
        }

        Debug.Log($"[UIManager] Showing panel: {panelType.Name}");
    }

    /// <summary>
    /// 隐藏 UI 面板
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public void HidePanel<T>() where T : MonoBehaviour
    {
        System.Type panelType = typeof(T);

        if (!_registeredPanels.TryGetValue(panelType, out MonoBehaviour panelObj))
        {
            Debug.LogWarning($"[UIManager] Panel {panelType.Name} is not registered.");
            return;
        }

        panelObj.gameObject.SetActive(false);

        // 从面板栈中移除
        if (_panelStack.Contains(panelObj))
        {
            _panelStack.Pop();
        }

        Debug.Log($"[UIManager] Hiding panel: {panelType.Name}");
    }

    /// <summary>
    /// 切换 UI 面板显示状态
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public void TogglePanel<T>() where T : MonoBehaviour
    {
        System.Type panelType = typeof(T);

        if (!_registeredPanels.TryGetValue(panelType, out MonoBehaviour panelObj))
        {
            Debug.LogWarning($"[UIManager] Panel {panelType.Name} is not registered.");
            return;
        }

        if (panelObj.gameObject.activeSelf)
        {
            HidePanel<T>();
        }
        else
        {
            ShowPanel<T>();
        }
    }

    /// <summary>
    /// 隐藏所有 UI 面板
    /// </summary>
    public void HideAllPanels()
    {
        // 使用 Keys 列表遍历，以便在迭代过程中安全地移除已销毁的面板
        var panelTypes = new List<System.Type>(_registeredPanels.Keys);
        
        foreach (var type in panelTypes)
        {
            var panel = _registeredPanels[type];
            if (panel != null)
            {
                panel.gameObject.SetActive(false);
            }
            else
            {
                // 如果面板已销毁（引用不再有效），则从字典中移除
                _registeredPanels.Remove(type);
                Debug.LogWarning($"[UIManager] Found destroyed panel reference for {type.Name}, removing from registry.");
            }
        }

        _panelStack.Clear();
        Debug.Log("[UIManager] All panels hidden (and registry cleaned).");
    }

    /// <summary>
    /// 返回到上一个面板（面板导航）
    /// </summary>
    public void GoBackToPreviousPanel()
    {
        if (_panelStack.Count > 1)
        {
            MonoBehaviour currentPanel = _panelStack.Pop();
            currentPanel.gameObject.SetActive(false);

            MonoBehaviour previousPanel = _panelStack.Peek();
            previousPanel.gameObject.SetActive(true);

            Debug.Log($"[UIManager] Returned to previous panel: {previousPanel.GetType().Name}");
        }
        else
        {
            Debug.LogWarning("[UIManager] No previous panel to return to.");
        }
    }

    /// <summary>
    /// 获取已注册的面板实例
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns>面板实例，如果未注册则返回 null</returns>
    public T GetPanel<T>() where T : MonoBehaviour
    {
        System.Type panelType = typeof(T);

        if (_registeredPanels.TryGetValue(panelType, out MonoBehaviour panelObj))
        {
             if (panelObj != null)
             {
                return panelObj as T;
             }
             else
             {
                 // 对象已销毁，清理引用
                 _registeredPanels.Remove(panelType);
             }
        }

        Debug.LogWarning($"[UIManager] Panel {panelType.Name} is not registered or has been destroyed.");
        return null;
    }

    /// <summary>
    /// 检查面板是否已注册
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns>是否已注册</returns>
    public bool IsPanelRegistered<T>() where T : MonoBehaviour
    {
        if (_registeredPanels.TryGetValue(typeof(T), out MonoBehaviour panelObj))
        {
            if (panelObj != null)
            {
                return true;
            }
            else
            {
                // 发现引用已销毁，趁机清理
                _registeredPanels.Remove(typeof(T));
                return false;
            }
        }
        return false;
    }


    /// <summary>
    /// 检查面板是否正在显示
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <returns>是否正在显示</returns>
    public bool IsPanelVisible<T>() where T : MonoBehaviour
    {
        System.Type panelType = typeof(T);

        if (_registeredPanels.TryGetValue(panelType, out MonoBehaviour panelObj))
        {
            return panelObj.gameObject.activeSelf;
        }

        return false;
    }
    /// <summary>
    /// 设置所有已注册面板的交互状态
    /// </summary>
    /// <param name="interactable">是否可交互</param>
    /// <param name="excludeType">排除的面板类型（可选）</param>
    public void SetAllPanelsInteraction(bool interactable, System.Type excludeType = null)
    {
        foreach (var kvp in _registeredPanels)
        {
            if (excludeType != null && kvp.Key == excludeType) continue;
            
            if (kvp.Value != null)
            {
                UIBasePanel panel = kvp.Value as UIBasePanel;
                CanvasGroup cg = kvp.Value.GetComponent<CanvasGroup>();
                if (cg == null)
                {
                    cg = kvp.Value.gameObject.AddComponent<CanvasGroup>();
                }
                
                cg.interactable = interactable;
                // 只有在面板本身允许阻挡射线时，才根据状态开启 blocksRaycasts
                cg.blocksRaycasts = interactable && (panel == null || panel.BlocksRaycasts);
            }
        }
        Debug.Log($"[UIManager] All panels interaction set to: {interactable} (Excluded: {excludeType?.Name ?? "None"})");
    }
}
