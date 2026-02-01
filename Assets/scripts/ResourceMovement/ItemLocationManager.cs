using System.Collections.Generic;
using UnityEngine;

// 物品跨场景位置管理（运行时）
public class ItemLocationManager : MonoSingleton<ItemLocationManager>
{
    private readonly Dictionary<string, ItemLocationState> items = new Dictionary<string, ItemLocationState>();

    public void SetItemsForLocation(ItemLocation location, List<ItemLocationState> newStates){
        if(items.Count > 0)
        {
            List<string> toRemove = new List<string>();
            foreach(var kvp in items)
            {
                if(kvp.Value != null && kvp.Value.location == location)
                {
                    toRemove.Add(kvp.Key);
                }
            }
            for(int i = 0; i < toRemove.Count; i++)
            {
                items.Remove(toRemove[i]);
            }
        }

        if(newStates == null)
        {
            return;
        }

        for(int i = 0; i < newStates.Count; i++)
        {
            UpsertInternal(newStates[i], location);
        }
    }

    public void Upsert(ItemLocationState state){
        if(state == null)
        {
            return;
        }
        UpsertInternal(state, state.location);
    }

    public List<ItemLocationState> GetItemsForLocation(ItemLocation location){
        List<ItemLocationState> result = new List<ItemLocationState>();
        foreach(var kvp in items)
        {
            if(kvp.Value != null && kvp.Value.location == location)
            {
                result.Add(kvp.Value);
            }
        }
        return result;
    }

    public ItemLocationState GetItem(string itemId){
        if(string.IsNullOrEmpty(itemId))
        {
            return null;
        }
        ItemLocationState state;
        if(items.TryGetValue(itemId, out state))
        {
            return state;
        }
        return null;
    }

    public bool HasItemInLocation(string itemId, ItemLocation location){
        ItemLocationState state = GetItem(itemId);
        return state != null && state.location == location;
    }

    public void Clear(){
        items.Clear();
    }

    private void UpsertInternal(ItemLocationState state, ItemLocation location){
        if(state == null || string.IsNullOrEmpty(state.itemId))
        {
            return;
        }
        state.location = location;
        items[state.itemId] = state;
    }
}
