using UnityEngine;

/// <summary>
/// 泛型单例基类，用于创建全局唯一的 MonoBehaviour 管理器
/// 特性：线程安全、DontDestroyOnLoad、自动实例化
/// </summary>
/// <typeparam name="T">继承自 MonoSingleton 的类型</typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _applicationIsQuitting = false;

    /// <summary>
    /// 获取单例实例（线程安全）
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[MonoSingleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // 尝试在场景中查找现有实例
                    _instance = FindObjectOfType<T>();

                    // 如果场景中不存在，则创建新的 GameObject
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"[Singleton] {typeof(T).Name}";

                        Debug.Log($"[MonoSingleton] Created new instance of {typeof(T).Name}");
                    }
                    else
                    {
                        Debug.Log($"[MonoSingleton] Using existing instance of {typeof(T).Name}");
                    }
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// Unity 生命周期：Awake
    /// 确保单例唯一性，并设置 DontDestroyOnLoad
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            OnInitialize();
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[MonoSingleton] Duplicate instance of {typeof(T).Name} detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化方法，子类可重写以实现自定义初始化逻辑
    /// </summary>
    protected virtual void OnInitialize()
    {
        // 子类可重写此方法
    }

    /// <summary>
    /// Unity 生命周期：OnApplicationQuit
    /// 标记应用程序正在退出，防止在退出时访问已销毁的实例
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    /// <summary>
    /// Unity 生命周期：OnDestroy
    /// 清理实例引用
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            // 注释掉这行，避免在场景切换时错误设置退出标志
            // _applicationIsQuitting = true;
        }
    }

    /// <summary>
    /// 重置单例状态（用于调试和修复）
    /// </summary>
    public static void ResetSingletonState()
    {
        _applicationIsQuitting = false;
        Debug.Log($"[MonoSingleton] Reset singleton state for {typeof(T).Name}");
    }
}
