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
        if(ItemLocationManager.Instance == null)
        {
            return;
        }
        List<ItemLocationState> states = CollectStates();
        ItemLocationManager.Instance.SetItemsForLocation(ItemLocation.Inventory, states);
        if(enableDebug)
        {
            Debug.Log($"[InventoryPanelSync] Saved items: {FormatIds(states)}", this);
        }
    }

    public void LoadFromManager(){
        if(ItemLocationManager.Instance == null)
        {
            return;
        }

        List<ItemLocationState> items = ItemLocationManager.Instance.GetItemsForLocation(ItemLocation.Inventory);
        ClearSlots();
        if(enableDebug)
        {
            Debug.Log($"[InventoryPanelSync] Loading items: {FormatIds(items)}", this);
        }

        bool[] occupied = new bool[slots.Count];
        List<ItemLocationState> unplaced = new List<ItemLocationState>();

        for(int i = 0; i < items.Count; i++)
        {
            ItemLocationState state = items[i];
            if(state == null)
            {
                continue;
            }

            int targetIndex = state.slotIndex;
            if(targetIndex < 0 || targetIndex >= slots.Count || occupied[targetIndex])
            {
                unplaced.Add(state);
                continue;
            }

            if(SpawnIntoSlot(state, slots[targetIndex]))
            {
                occupied[targetIndex] = true;
            }
            else
            {
                unplaced.Add(state);
            }
        }

        for(int i = 0; i < unplaced.Count; i++)
        {
            ItemLocationState state = unplaced[i];
            int freeIndex = FindFirstFreeSlotIndex(occupied);
            if(freeIndex < 0)
            {
                break;
            }

            if(SpawnIntoSlot(state, slots[freeIndex]))
            {
                occupied[freeIndex] = true;
            }
        }
    }

    private List<ItemLocationState> CollectStates(){
        List<ItemLocationState> states = new List<ItemLocationState>();
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
            states.Add(info.ToLocationState(ItemLocation.Inventory, i, Vector2.zero));
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

    private GameObject FindTemplate(string itemId){
        for(int i = 0; i < prefabs.Count; i++)
        {
            ItemPrefabEntry entry = prefabs[i];
            if(entry != null && entry.prefab != null && entry.itemId == itemId)
            {
                return entry.prefab;
            }
        }
        if(ItemTemplateLibrary.Instance != null)
        {
            return ItemTemplateLibrary.Instance.GetTemplate(itemId);
        }
        return null;
    }

    private string FormatIds(List<ItemLocationState> items){
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

    private bool SpawnIntoSlot(ItemLocationState state, InventorySlot slot){
        if(state == null || slot == null)
        {
            return false;
        }

        GameObject template = FindTemplate(state.itemId);
        if(template == null)
        {
            if(enableDebug)
            {
                Debug.LogWarning($"[InventoryPanelSync] Template not found for itemId: {state.itemId}", this);
            }
            return false;
        }

        GameObject instance = Instantiate(template, slot.transform);
        if(!instance.activeSelf)
        {
            instance.SetActive(true);
        }
        DragByInterface dragItem = instance.GetComponent<DragByInterface>();
        if(dragItem == null)
        {
            Destroy(instance);
            return false;
        }

        ItemInfo info = instance.GetComponent<ItemInfo>();
        if(info != null)
        {
            info.ApplyState(state);
        }

        dragItem.PlaceInSlot(slot.transform);
        slot.SetItem(dragItem);
        return true;
    }

    private int FindFirstFreeSlotIndex(bool[] occupied){
        if(occupied == null)
        {
            return -1;
        }

        for(int i = 0; i < occupied.Length; i++)
        {
            if(!occupied[i])
            {
                return i;
            }
        }
        return -1;
    }
}
