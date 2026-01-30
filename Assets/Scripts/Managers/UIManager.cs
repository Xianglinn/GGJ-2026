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
    [SerializeField]
    private Canvas _uiCanvas;

    /// <summary>
    /// 获取 UI Canvas
    /// </summary>
    public Canvas UICanvas => _uiCanvas;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        // 尝试查找场景中的 Canvas
        if (_uiCanvas == null)
        {
            _uiCanvas = FindObjectOfType<Canvas>();
            if (_uiCanvas == null)
            {
                Debug.LogWarning("[UIManager] No Canvas found in scene. UI panels may not display correctly.");
            }
            else
            {
                // 确保 Canvas 在场景切换时不被销毁
                DontDestroyOnLoad(_uiCanvas.gameObject);
                Debug.Log("[UIManager] Canvas set to DontDestroyOnLoad.");
            }
        }
        else
        {
            // 如果 Canvas 已经赋值，也要确保它不被销毁
            DontDestroyOnLoad(_uiCanvas.gameObject);
            Debug.Log("[UIManager] Canvas set to DontDestroyOnLoad.");
        }

        Debug.Log("[UIManager] Initialized successfully.");
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
            Debug.LogWarning($"[UIManager] Panel {panelType.Name} is already registered. Overwriting.");
        }

        _registeredPanels[panelType] = panel;
        panel.gameObject.SetActive(false); // 默认隐藏面板

        Debug.Log($"[UIManager] Registered panel: {panelType.Name}");
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
        foreach (var panel in _registeredPanels.Values)
        {
            panel.gameObject.SetActive(false);
        }

        _panelStack.Clear();
        Debug.Log("[UIManager] All panels hidden.");
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
}
