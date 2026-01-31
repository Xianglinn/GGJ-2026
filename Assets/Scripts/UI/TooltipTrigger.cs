using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 悬浮提示触发器
/// </summary>
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data Source")]
    [SerializeField] private ItemInfo itemInfo;
    
    [Header("Settings")]
    [Tooltip("如果为 true，则在游戏场景(Scene3)中也允许显示（用于物品栏）")]
    [SerializeField] private bool isInventoryItem = false;

    private void Awake()
    {
        if (itemInfo == null)
        {
            itemInfo = GetComponent<ItemInfo>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ShouldShowTooltip())
        {
            string desc = itemInfo != null ? itemInfo.DescriptionCN : "No Description";
            UIManager.Instance.ShowPanel<UITooltipPanel, string>(desc);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HidePanel<UITooltipPanel>();
    }

    private bool ShouldShowTooltip()
    {
        if (GameFlowManager.Instance == null) return true;

        GameState state = GameFlowManager.Instance.CurrentState;

        // 场景 2 (Prologue): 允许所有挂载了此脚本的物体触发（主要是场景内的 2D 物体）
        if (state == GameState.Prologue)
        {
            return true;
        }
        
        // 场景 3 (Gameplay): 仅允许物品栏物品触发，场景内物体不触发
        if (state == GameState.Gameplay)
        {
            return isInventoryItem;
        }

        return false;
    }
}
