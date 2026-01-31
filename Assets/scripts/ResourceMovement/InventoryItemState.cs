using System;

// 背包中的物品运行时状态（可序列化）
[Serializable]
public class InventoryItemState
{
    public string itemId;
    public bool isProcessed;
    public SpecialEffectType specialEffects;

    public InventoryItemState(string itemId, bool isProcessed, SpecialEffectType specialEffects)
    {
        this.itemId = itemId;
        this.isProcessed = isProcessed;
        this.specialEffects = specialEffects;
    }
}
