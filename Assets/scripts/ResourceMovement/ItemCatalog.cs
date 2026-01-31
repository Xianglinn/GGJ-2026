using System.Collections.Generic;
using UnityEngine;

// 物品数据目录（用于通过 ID 查找）
[CreateAssetMenu(menuName = "GGJ/Item Catalog", fileName = "ItemCatalog")]
public class ItemCatalog : ScriptableObject
{
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    public ItemData GetById(string id){
        if(string.IsNullOrEmpty(id))
        {
            return null;
        }

        for(int i = 0; i < items.Count; i++)
        {
            ItemData data = items[i];
            if(data != null && data.Id == id)
            {
                return data;
            }
        }
        return null;
    }
}
