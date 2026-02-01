using System.Collections.Generic;
using UnityEngine;

// Scene2/Scene3 物品面板与全局状态同步
public class SceneItemPanelSync : MonoBehaviour
{
    [System.Serializable]
    private class ItemPrefabEntry
    {
        public string itemId;
        public GameObject prefab;
    }

    [SerializeField] private ItemLocation location = ItemLocation.Scene2;
    [SerializeField] private RectTransform container;
    [SerializeField] private List<ItemPrefabEntry> prefabs = new List<ItemPrefabEntry>();
    [SerializeField] private bool enableDebug;

    public void SaveToManager(){
        if(ItemLocationManager.Instance == null)
        {
            return;
        }
        List<ItemLocationState> states = CollectStatesFromChildren();
        ItemLocationManager.Instance.SetItemsForLocation(location, states);
        if(enableDebug)
        {
            Debug.Log($"[SceneItemPanelSync] Saved {location}: {FormatIds(states)}", this);
        }
    }

    public void LoadFromManager(){
        if(ItemLocationManager.Instance == null)
        {
            return;
        }

        List<ItemLocationState> items = ItemLocationManager.Instance.GetItemsForLocation(location);
        if(items == null || items.Count == 0)
        {
            List<ItemLocationState> bootstrap = CollectStatesFromChildren();
            if(bootstrap.Count > 0)
            {
                ItemLocationManager.Instance.SetItemsForLocation(location, bootstrap);
                if(enableDebug)
                {
                    Debug.Log($"[SceneItemPanelSync] Bootstrapped {location}: {FormatIds(bootstrap)}", this);
                }
            }
            return;
        }

        ClearContainer();

        for(int i = 0; i < items.Count; i++)
        {
            ItemLocationState state = items[i];
            if(state == null)
            {
                continue;
            }

            GameObject template = FindTemplate(state.itemId);
            if(template == null)
            {
                if(enableDebug)
                {
                    Debug.LogWarning($"[SceneItemPanelSync] Template not found for itemId: {state.itemId}", this);
                }
                continue;
            }

            GameObject instance = Instantiate(template, GetContainer());
            if(!instance.activeSelf)
            {
                instance.SetActive(true);
            }
            RectTransform rect = instance.GetComponent<RectTransform>();
            if(rect != null)
            {
                rect.anchoredPosition = state.anchoredPosition;
            }

            ItemInfo info = instance.GetComponent<ItemInfo>();
            if(info != null)
            {
                info.ApplyState(state);
            }
        }
    }

    private List<ItemLocationState> CollectStatesFromChildren(){
        List<ItemLocationState> states = new List<ItemLocationState>();
        RectTransform target = GetContainer();
        if(target == null)
        {
            return states;
        }

        for(int i = 0; i < target.childCount; i++)
        {
            Transform child = target.GetChild(i);
            if(child == null)
            {
                continue;
            }

            ItemInfo info = child.GetComponent<ItemInfo>();
            if(info == null)
            {
                info = child.GetComponentInChildren<ItemInfo>();
            }
            if(info == null)
            {
                continue;
            }

            RectTransform rect = child.GetComponent<RectTransform>();
            Vector2 pos = rect != null ? rect.anchoredPosition : Vector2.zero;
            if(ItemTemplateLibrary.Instance != null)
            {
                ItemTemplateLibrary.Instance.RegisterTemplateFromInstance(info);
            }
            states.Add(info.ToLocationState(location, -1, pos));
        }
        return states;
    }

    private RectTransform GetContainer(){
        if(container != null)
        {
            return container;
        }
        return transform as RectTransform;
    }

    private void ClearContainer(){
        RectTransform target = GetContainer();
        if(target == null)
        {
            return;
        }

        List<GameObject> toDestroy = new List<GameObject>();
        for(int i = 0; i < target.childCount; i++)
        {
            Transform child = target.GetChild(i);
            if(child != null)
            {
                toDestroy.Add(child.gameObject);
            }
        }

        for(int i = 0; i < toDestroy.Count; i++)
        {
            Destroy(toDestroy[i]);
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
}
