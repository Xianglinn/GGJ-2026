using UnityEngine;

/// <summary>
/// 素材栏数据模型：管理 8 个格位的素材数据
/// </summary>
[System.Serializable]
public class InventoryModel
{
    /// <summary>
    /// 素材数组，固定 8 个格位
    /// </summary>
    public ItemData[] items;

    /// <summary>
    /// 构造函数：初始化 8 个空格位
    /// </summary>
    public InventoryModel()
    {
        items = new ItemData[8];
    }

    /// <summary>
    /// 根据素材 ID 查找格位索引
    /// </summary>
    /// <param name="itemId">素材 ID</param>
    /// <returns>格位索引，未找到返回 -1</returns>
    public int FindSlotByItemId(string itemId)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].itemId == itemId)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 添加素材到下一个空格位
    /// </summary>
    /// <param name="item">素材数据</param>
    /// <returns>添加成功的格位索引，失败返回 -1</returns>
    public int AddItem(ItemData item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                item.isCollected = true;
                return i;
            }
        }
        Debug.LogWarning("[InventoryModel] Inventory is full. Cannot add more items.");
        return -1;
    }

    /// <summary>
    /// 获取已收集的素材数量
    /// </summary>
    public int GetCollectedCount()
    {
        int count = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].isCollected)
            {
                count++;
            }
        }
        return count;
    }
}
