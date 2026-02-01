using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 加工槽位：只接收可加工物品，并立即变成第二形态
public class ProcessingSlot : MonoBehaviour, IDropHandler, ISlot
{
    [System.Serializable]
    private class ProcessedOverride
    {
        public string itemId;
        public SpecialEffectType effects = SpecialEffectType.None;
        public bool replaceEffects = true;
    }

    [SerializeField] private bool onlyUnprocessed = true;
    [SerializeField] private bool autoReturnToInventory = true;
    [SerializeField] private string inventoryCanvasName = "InventoryCanvas";
    [SerializeField] private List<ProcessedOverride> overrides = new List<ProcessedOverride>();
    private DragByInterface currentItem;

    public void OnDrop(PointerEventData eventData){
        if(currentItem != null)
        {
            if(!autoReturnToInventory || !TryReturnToInventory(currentItem))
            {
                return;
            }
            currentItem = null;
        }

        if(eventData.pointerDrag == null)
        {
            return;
        }

        DragByInterface dragItem = eventData.pointerDrag.GetComponent<DragByInterface>();
        if(dragItem == null)
        {
            return;
        }

        ItemInfo info = dragItem.ItemInfo;
        if(info == null || !info.CanBeProcessed)
        {
            return;
        }

        if(onlyUnprocessed && info.IsProcessed)
        {
            return;
        }

        string sourceId = info.ItemId;
        dragItem.PlaceInSlot(transform);
        currentItem = dragItem;
        info.SetProcessed(true);
        ApplyOverrides(info, sourceId);

        if(autoReturnToInventory)
        {
            if(TryReturnToInventory(dragItem))
            {
                currentItem = null;
            }
        }
    }

    public void SetItem(DragByInterface item){
        currentItem = item;
    }

    public void ClearItem(DragByInterface item){
        if(currentItem == item)
        {
            currentItem = null;
        }
    }

    public ItemInfo GetItemInfo(){
        return currentItem != null ? currentItem.ItemInfo : null;
    }

    private void ApplyOverrides(ItemInfo info, string sourceItemId){
        if(info == null || overrides == null || overrides.Count == 0)
        {
            return;
        }

        string itemId = !string.IsNullOrEmpty(sourceItemId) ? sourceItemId : info.ItemId;
        if(string.IsNullOrEmpty(itemId))
        {
            return;
        }

        for(int i = 0; i < overrides.Count; i++)
        {
            ProcessedOverride entry = overrides[i];
            if(entry == null || string.IsNullOrEmpty(entry.itemId))
            {
                continue;
            }
            if(entry.itemId == itemId)
            {
                if(entry.replaceEffects)
                {
                    info.SetSpecialEffects(entry.effects);
                }
                else
                {
                    info.SetSpecialEffects(info.SpecialEffects | entry.effects);
                }
                return;
            }
        }
    }

    private bool TryReturnToInventory(DragByInterface item){
        if(item == null)
        {
            return false;
        }

        InventorySlot originalSlot = GetOriginalInventorySlot(item);
        if(originalSlot != null && originalSlot.GetItemInfo() == null)
        {
            item.PlaceInSlot(originalSlot.transform);
            originalSlot.SetItem(item);
            return true;
        }

        GameObject inventoryCanvas = GameObject.Find(inventoryCanvasName);
        if(inventoryCanvas == null)
        {
            return false;
        }

        InventorySlot[] allSlots = inventoryCanvas.GetComponentsInChildren<InventorySlot>(true);
        if(allSlots == null || allSlots.Length == 0)
        {
            return false;
        }

        for(int i = 0; i < allSlots.Length; i++)
        {
            InventorySlot slot = allSlots[i];
            if(slot == null)
            {
                continue;
            }
            if(slot.GetItemInfo() == null)
            {
                item.PlaceInSlot(slot.transform);
                slot.SetItem(item);
                return true;
            }
        }
        return false;
    }

    private InventorySlot GetOriginalInventorySlot(DragByInterface item){
        if(item == null || item.LastSlotTransform == null)
        {
            return null;
        }
        return item.LastSlotTransform.GetComponent<InventorySlot>();
    }
}
