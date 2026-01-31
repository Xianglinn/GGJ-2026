using System.Collections.Generic;
using UnityEngine;

// Scene2/Scene3 背包 UI 与全局状态同步（最小版本）
public class InventoryPanelSync : MonoBehaviour
{
    [System.Serializable]
    private class ItemPrefabEntry
    {
        public string itemId;
        public GameObject prefab;
    }

    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
    [SerializeField] private List<ItemPrefabEntry> prefabs = new List<ItemPrefabEntry>();
    [SerializeField] private bool enableDebug;

    public void SaveToManager(){
        if(InventoryManager.Instance == null)
        {
            return;
        }
        List<InventoryItemState> states = CollectStates();
        InventoryManager.Instance.SetItems(states);
        if(enableDebug)
        {
            Debug.Log($"[InventoryPanelSync] Saved items: {FormatIds(states)}", this);
        }
    }

    public void LoadFromManager(){
        if(InventoryManager.Instance == null)
        {
            return;
        }

        List<InventoryItemState> items = InventoryManager.Instance.GetItems();
        ClearSlots();
        if(enableDebug)
        {
            Debug.Log($"[InventoryPanelSync] Loading items: {FormatIds(items)}", this);
        }

        int slotIndex = 0;
        for(int i = 0; i < items.Count && slotIndex < slots.Count; i++)
        {
            InventoryItemState state = items[i];
            GameObject prefab = FindPrefab(state.itemId);
            if(prefab == null)
            {
                if(enableDebug)
                {
                    Debug.LogWarning($"[InventoryPanelSync] Prefab not found for itemId: {state.itemId}", this);
                }
                continue;
            }

            InventorySlot slot = slots[slotIndex];
            if(slot == null)
            {
                continue;
            }

            GameObject instance = Instantiate(prefab, slot.transform);
            DragByInterface dragItem = instance.GetComponent<DragByInterface>();
            if(dragItem == null)
            {
                Destroy(instance);
                continue;
            }

            ItemInfo info = instance.GetComponent<ItemInfo>();
            if(info != null)
            {
                info.ApplyState(state);
            }

            dragItem.PlaceInSlot(slot.transform);
            slot.SetItem(dragItem);
            slotIndex++;
        }
    }

    private List<InventoryItemState> CollectStates(){
        List<InventoryItemState> states = new List<InventoryItemState>();
        for(int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            if(slot == null)
            {
                continue;
            }

            ItemInfo info = slot.GetItemInfo();
            if(info == null)
            {
                continue;
            }
            states.Add(info.ToState());
        }
        return states;
    }

    private void ClearSlots(){
        for(int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            if(slot == null)
            {
                continue;
            }
            slot.ClearImmediate();
        }
    }

    private GameObject FindPrefab(string itemId){
        for(int i = 0; i < prefabs.Count; i++)
        {
            ItemPrefabEntry entry = prefabs[i];
            if(entry != null && entry.prefab != null && entry.itemId == itemId)
            {
                return entry.prefab;
            }
        }
        return null;
    }

    private string FormatIds(List<InventoryItemState> items){
        if(items == null || items.Count == 0)
        {
            return "(none)";
        }
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for(int i = 0; i < items.Count; i++)
        {
            if(i > 0) sb.Append(", ");
            sb.Append(items[i].itemId);
        }
        return sb.ToString();
    }
}
