using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 挂载在物品或者背包槽位上，鼠标悬浮时触发 Tooltip
/// 支持 3D/2D 物体 (OnMouseEnter) 和 UI 元素 (OnPointerEnter)
/// 自动支持从 InventorySlot 获取物品信息
/// </summary>
public class ItemTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemInfo itemInfo; // 缓存的 direct reference (用于 World Object)
    private InventorySlot inventorySlot; // 是否挂载在 Slot 上
    private bool isHovering;

    private void Awake()
    {
        // 尝试获取自身 ItemInfo (用于 World Object)
        // 使用 GetComponentInParent 以兼容触发器在子对象的情况 (如 ItemModel)
        itemInfo = GetComponentInParent<ItemInfo>();
        
        // 尝试获取 Slot (用于 Inventory Slot)
        // 使用 GetComponentInParent 以兼容触发器在子对象的情况 (如 Slot Image)
        inventorySlot = GetComponentInParent<InventorySlot>();
    }

    /// <summary>
    /// 获取当前应该显示的 ItemInfo
    /// </summary>
    private ItemInfo GetCurrentItemInfo()
    {
        // 1. 如果是 Slot，优先从 Slot 获取当前物品
        if (inventorySlot != null)
        {
            return inventorySlot.GetItemInfo();
        }
        
        // 2. 如果不是 Slot，或者是 World Object，使用自身的 ItemInfo
        return itemInfo;
    }

    #region Physics Interaction (World Objects)

    private void OnMouseEnter()
    {
        TryShowTooltip();
    }

    private void OnMouseExit()
    {
        TryHideTooltip();
    }

    private void OnMouseDown()
    {
        // 拖拽开始时隐藏 tooltip
        TryHideTooltip();
    }

    #endregion

    #region UI Interaction (Inventory Items)

    public void OnPointerEnter(PointerEventData eventData)
    {
        TryShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TryHideTooltip();
    }

    #endregion
    
    private void OnDisable()
    {
        TryHideTooltip();
    }

    private void TryShowTooltip()
    {
        if (isHovering) return; // 避免重复触发

        var info = GetCurrentItemInfo();

        // 使用 IsNullOrWhiteSpace 以进行更严格的检查
        if (info != null && !string.IsNullOrWhiteSpace(info.DescriptionCN))
        {
            isHovering = true;
            ShowTooltipCore(info.DescriptionCN);
        }
    }

    private void TryHideTooltip()
    {
        if (!isHovering) return;

        isHovering = false;
        HideTooltipCore();
    }

    private void ShowTooltipCore(string text)
    {
        if (UIManager.Instance != null)
        {
            // 确保 TooltipPanel 已注册
            if (!UIManager.Instance.IsPanelRegistered<UITooltipPanel>())
            {
               var panel = FindObjectOfType<UITooltipPanel>(true);
               if (panel != null) UIManager.Instance.RegisterPanel(panel);
            }

            if (UIManager.Instance.IsPanelRegistered<UITooltipPanel>())
            {
                UIManager.Instance.ShowPanel<UITooltipPanel, string>(text);
            }
        }
    }

    private void HideTooltipCore()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsPanelRegistered<UITooltipPanel>())
        {
            UIManager.Instance.HidePanel<UITooltipPanel>();
        }
    }
}
