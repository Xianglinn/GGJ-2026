using UnityEngine;

/// <summary>
/// UI 面板泛型基类，支持通过泛型 T 接收不同的数据模型
/// 提供标准化的 UI 面板生命周期管理
/// </summary>
/// <typeparam name="T">数据模型类型</typeparam>
/// <summary>
/// Canvas 类型枚举
/// </summary>
public enum CanvasType
{
    Persistent, // 常驻 Canvas (DontDestroyOnLoad)
    SceneLocal  // 场景本地 Canvas (随场景销毁)
}

public class UIBasePanel<T> : MonoBehaviour where T : class
{
    /// <summary>
    /// 面板是否已初始化
    /// </summary>
    protected bool isInitialized = false;

    /// <summary>
    /// 面板是否可见
    /// </summary>
    protected bool isVisible = false;

    /// <summary>
    /// 当前面板的数据模型
    /// </summary>
    protected T currentData;

    /// <summary>
    /// 面板所属的 Canvas 类型
    /// </summary>
    public virtual CanvasType PanelCanvasType => CanvasType.SceneLocal;

    /// <summary>
    /// 初始化面板并传入数据
    /// </summary>
    /// <param name="data">传入的数据模型</param>
    public virtual void OnInitialize(T data)
    {
        if (isInitialized)
        {
            Debug.LogWarning($"[UIBasePanel] {GetType().Name} is already initialized.");
            return;
        }

        currentData = data;
        isInitialized = true;

        Debug.Log($"[UIBasePanel] {GetType().Name} initialized with data: {data?.GetType().Name ?? "null"}");
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    public virtual void Show()
    {
        if (!isInitialized)
        {
            Debug.LogWarning($"[UIBasePanel] {GetType().Name} is not initialized. Call OnInitialize first.");
            return;
        }

        gameObject.SetActive(true);
        isVisible = true;

        OnShowAnimationStart();

        Debug.Log($"[UIBasePanel] {GetType().Name} shown.");
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    public virtual void Hide()
    {
        if (!isVisible)
        {
            Debug.LogWarning($"[UIBasePanel] {GetType().Name} is already hidden.");
            return;
        }

        OnHideAnimationStart();

        gameObject.SetActive(false);
        isVisible = false;

        Debug.Log($"[UIBasePanel] {GetType().Name} hidden.");
    }

    /// <summary>
    /// 更新面板数据（可选）
    /// </summary>
    /// <param name="newData">新的数据模型</param>
    public virtual void UpdateData(T newData)
    {
        currentData = newData;
        OnDataUpdated();

        Debug.Log($"[UIBasePanel] {GetType().Name} data updated.");
    }

    /// <summary>
    /// 显示动画开始时调用（子类可重写）
    /// </summary>
    protected virtual void OnShowAnimationStart()
    {
        // 子类可重写此方法以添加显示动画
    }

    /// <summary>
    /// 隐藏动画开始时调用（子类可重写）
    /// </summary>
    protected virtual void OnHideAnimationStart()
    {
        // 子类可重写此方法以添加隐藏动画
    }

    /// <summary>
    /// 数据更新时调用（子类可重写）
    /// </summary>
    protected virtual void OnDataUpdated()
    {
        // 子类可重写此方法以更新 UI 显示
    }

    /// <summary>
    /// Unity 生命周期：OnDestroy
    /// </summary>
    protected virtual void OnDestroy()
    {
        currentData = null;
        isInitialized = false;
        isVisible = false;
    }
}
