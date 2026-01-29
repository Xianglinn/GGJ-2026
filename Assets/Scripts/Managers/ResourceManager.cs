using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局资源管理器，负责资源加载和缓存
/// 特性：单例模式、泛型加载、资源缓存、内存管理
/// </summary>
public class ResourceManager : MonoSingleton<ResourceManager>
{
    /// <summary>
    /// 资源缓存字典 (路径 -> 资源对象)
    /// </summary>
    private Dictionary<string, Object> _resourceCache = new Dictionary<string, Object>();

    /// <summary>
    /// 是否启用资源缓存
    /// </summary>
    [SerializeField]
    private bool _enableCaching = true;

    /// <summary>
    /// 缓存大小限制（0 = 无限制）
    /// </summary>
    [SerializeField]
    private int _maxCacheSize = 100;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        Debug.Log("[ResourceManager] Initialized successfully.");
    }

    /// <summary>
    /// 加载资源（泛型版本，支持各种资源类型）
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径（相对于 Resources 文件夹）</param>
    /// <returns>加载的资源，失败返回 null</returns>
    public T Load<T>(string path) where T : Object
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("[ResourceManager] Load failed: path is null or empty.");
            return null;
        }

        // 检查缓存
        if (_enableCaching && _resourceCache.TryGetValue(path, out Object cachedResource))
        {
            Debug.Log($"[ResourceManager] Loaded from cache: {path}");
            return cachedResource as T;
        }

        // 从 Resources 文件夹加载
        T resource = Resources.Load<T>(path);

        if (resource == null)
        {
            Debug.LogError($"[ResourceManager] Failed to load resource at path: {path}");
            return null;
        }

        // 添加到缓存
        if (_enableCaching)
        {
            // 检查缓存大小限制
            if (_maxCacheSize > 0 && _resourceCache.Count >= _maxCacheSize)
            {
                Debug.LogWarning($"[ResourceManager] Cache size limit reached ({_maxCacheSize}). Consider clearing cache.");
            }
            else
            {
                _resourceCache[path] = resource;
                Debug.Log($"[ResourceManager] Cached resource: {path}");
            }
        }

        Debug.Log($"[ResourceManager] Loaded resource: {path}");
        return resource;
    }

    /// <summary>
    /// 加载指定路径下的所有资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径（相对于 Resources 文件夹）</param>
    /// <returns>加载的资源数组</returns>
    public T[] LoadAll<T>(string path) where T : Object
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("[ResourceManager] LoadAll failed: path is null or empty.");
            return null;
        }

        T[] resources = Resources.LoadAll<T>(path);

        if (resources == null || resources.Length == 0)
        {
            Debug.LogWarning($"[ResourceManager] No resources found at path: {path}");
            return new T[0];
        }

        // 缓存所有加载的资源
        if (_enableCaching)
        {
            for (int i = 0; i < resources.Length; i++)
            {
                string resourcePath = $"{path}/{resources[i].name}";
                if (!_resourceCache.ContainsKey(resourcePath))
                {
                    _resourceCache[resourcePath] = resources[i];
                }
            }
        }

        Debug.Log($"[ResourceManager] Loaded {resources.Length} resources from: {path}");
        return resources;
    }

    /// <summary>
    /// 卸载指定路径的资源（从缓存中移除）
    /// </summary>
    /// <param name="path">资源路径</param>
    public void Unload(string path)
    {
        if (_resourceCache.ContainsKey(path))
        {
            _resourceCache.Remove(path);
            Debug.Log($"[ResourceManager] Unloaded resource from cache: {path}");
        }
        else
        {
            Debug.LogWarning($"[ResourceManager] Resource not found in cache: {path}");
        }
    }

    /// <summary>
    /// 清空所有资源缓存
    /// </summary>
    public void ClearCache()
    {
        _resourceCache.Clear();
        Debug.Log("[ResourceManager] All resource cache cleared.");
    }

    /// <summary>
    /// 卸载未使用的资源（调用 Unity 的垃圾回收）
    /// </summary>
    public void UnloadUnusedAssets()
    {
        Resources.UnloadUnusedAssets();
        Debug.Log("[ResourceManager] Unloaded unused assets.");
    }

    /// <summary>
    /// 获取当前缓存的资源数量
    /// </summary>
    public int GetCacheCount()
    {
        return _resourceCache.Count;
    }

    /// <summary>
    /// 检查资源是否已缓存
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <returns>是否已缓存</returns>
    public bool IsCached(string path)
    {
        return _resourceCache.ContainsKey(path);
    }

    /// <summary>
    /// 预加载资源列表（用于提前加载常用资源）
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="paths">资源路径列表</param>
    public void PreloadResources<T>(string[] paths) where T : Object
    {
        if (paths == null || paths.Length == 0)
        {
            Debug.LogWarning("[ResourceManager] PreloadResources: paths array is null or empty.");
            return;
        }

        int successCount = 0;
        foreach (string path in paths)
        {
            T resource = Load<T>(path);
            if (resource != null)
            {
                successCount++;
            }
        }

        Debug.Log($"[ResourceManager] Preloaded {successCount}/{paths.Length} resources.");
    }

    /// <summary>
    /// 启用或禁用资源缓存
    /// </summary>
    /// <param name="enable">是否启用缓存</param>
    public void SetCachingEnabled(bool enable)
    {
        _enableCaching = enable;
        Debug.Log($"[ResourceManager] Resource caching {(enable ? "enabled" : "disabled")}.");
    }

    /// <summary>
    /// 设置缓存大小限制
    /// </summary>
    /// <param name="maxSize">最大缓存数量（0 = 无限制）</param>
    public void SetMaxCacheSize(int maxSize)
    {
        _maxCacheSize = maxSize;
        Debug.Log($"[ResourceManager] Max cache size set to: {maxSize}");
    }
}
