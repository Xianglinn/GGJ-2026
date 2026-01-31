using System.Collections.Generic;
using UnityEngine;

// 跨场景背包状态管理器（仅运行时）
public class InventoryManager : MonoSingleton<InventoryManager>
{
    private readonly List<InventoryItemState> items = new List<InventoryItemState>();

    public void SetItems(List<InventoryItemState> newItems){
        items.Clear();
        if(newItems == null)
        {
            return;
        }
        items.AddRange(newItems);
    }

    public List<InventoryItemState> GetItems(){
        return new List<InventoryItemState>(items);
    }

    public bool HasItem(string itemId){
        if(string.IsNullOrEmpty(itemId))
        {
            return false;
        }
        for(int i = 0; i < items.Count; i++)
        {
            if(items[i].itemId == itemId)
            {
                return true;
            }
        }
        return false;
    }

    public void Clear(){
        items.Clear();
    }
}
