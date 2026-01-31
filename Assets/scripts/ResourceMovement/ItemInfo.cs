using UnityEngine;

// 物品基础信息与玩法数据
public class ItemInfo : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string itemId;

    [Header("Basic Info")]
    [TextArea(2, 6)]
    [SerializeField] private string descriptionCN;
    [Header("Mask Part")]
    [SerializeField] private MaskPartType partType;
    [SerializeField] private bool isProcessed;
    [SerializeField] private bool canBeProcessed = true;
    [Header("Special Effect")]
    [SerializeField] private SpecialEffectType specialEffects = SpecialEffectType.None;

    public string ItemId => !string.IsNullOrEmpty(itemId) ? itemId : gameObject.name;
    public string DescriptionCN => descriptionCN;
    public MaskPartType PartType => partType;
    public bool IsProcessed => isProcessed;
    public bool CanBeProcessed => canBeProcessed;
    public SpecialEffectType SpecialEffects => specialEffects;

    private void Start(){
        if(InventoryManager.Instance != null && InventoryManager.Instance.HasItem(ItemId))
        {
            gameObject.SetActive(false);
        }
    }

    // 设置加工状态
    public void SetProcessed(bool processed){
        if(canBeProcessed)
        {
            isProcessed = processed;
        }
    }

    public InventoryItemState ToState(){
        return new InventoryItemState(ItemId, isProcessed, specialEffects);
    }

    public void ApplyState(InventoryItemState state){
        if(state == null)
        {
            return;
        }
        isProcessed = state.isProcessed;
        specialEffects = state.specialEffects;
    }
}
