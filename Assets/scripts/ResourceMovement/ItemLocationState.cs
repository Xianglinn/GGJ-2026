using System;
using UnityEngine;

public enum ItemLocation
{
    Scene2,
    Inventory,
    Scene3
}

[Serializable]
public class ItemLocationState
{
    public string itemId;
    public ItemLocation location;
    public int slotIndex;
    public Vector2 anchoredPosition;
    public bool isProcessed;
    public SpecialEffectType specialEffects;

    public ItemLocationState(string itemId, ItemLocation location, int slotIndex, Vector2 anchoredPosition, bool isProcessed, SpecialEffectType specialEffects)
    {
        this.itemId = itemId;
        this.location = location;
        this.slotIndex = slotIndex;
        this.anchoredPosition = anchoredPosition;
        this.isProcessed = isProcessed;
        this.specialEffects = specialEffects;
    }
}
