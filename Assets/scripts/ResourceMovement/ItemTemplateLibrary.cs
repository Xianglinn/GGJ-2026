using System.Collections.Generic;
using UnityEngine;

// 物品模板库（运行时 / 可持久化）
public class ItemTemplateLibrary : MonoSingleton<ItemTemplateLibrary>
{
    [System.Serializable]
    private class ItemTemplateEntry
    {
        public string itemId;
        public GameObject template;
    }

    [SerializeField] private List<ItemTemplateEntry> templates = new List<ItemTemplateEntry>();

    private readonly Dictionary<string, GameObject> templateMap = new Dictionary<string, GameObject>();

    protected override void OnInitialize()
    {
        base.OnInitialize();
        RebuildMap();
    }

    public GameObject GetTemplate(string itemId){
        if(string.IsNullOrEmpty(itemId))
        {
            return null;
        }
        if(templateMap.Count == 0 && templates.Count > 0)
        {
            RebuildMap();
        }
        GameObject template;
        if(templateMap.TryGetValue(itemId, out template))
        {
            return template;
        }
        return null;
    }

    public void RegisterTemplateFromInstance(ItemInfo info){
        if(info == null)
        {
            return;
        }
        string itemId = info.ItemId;
        if(string.IsNullOrEmpty(itemId))
        {
            return;
        }
        if(templateMap.ContainsKey(itemId))
        {
            return;
        }

        GameObject clone = Instantiate(info.gameObject, transform);
        clone.name = $"{itemId}_Template";
        if(clone.activeSelf)
        {
            clone.SetActive(false);
        }

        templateMap[itemId] = clone;
        templates.Add(new ItemTemplateEntry
        {
            itemId = itemId,
            template = clone
        });
    }

    private void RebuildMap(){
        templateMap.Clear();
        for(int i = 0; i < templates.Count; i++)
        {
            ItemTemplateEntry entry = templates[i];
            if(entry == null || string.IsNullOrEmpty(entry.itemId) || entry.template == null)
            {
                continue;
            }
            templateMap[entry.itemId] = entry.template;
        }
    }
}
