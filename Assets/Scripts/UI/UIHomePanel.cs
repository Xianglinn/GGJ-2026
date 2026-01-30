using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 首页 UI 面板：监听任意点击，切换到序章状态
/// </summary>
public class UIHomePanel : UIBasePanel<object>, IPointerClickHandler
{
    private void Awake()
    {
        // 自动注册到 UIManager
        if (UIManager.Instance != null && !UIManager.Instance.IsPanelRegistered<UIHomePanel>())
        {
            UIManager.Instance.RegisterPanel(this);
        }
    }

    public override void OnInitialize(object data)
    {
        base.OnInitialize(data);
    }

    /// <summary>
    /// 任意点击首页进入序章（场景2）
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.SwitchState(GameState.Prologue);
        }
    }
}
