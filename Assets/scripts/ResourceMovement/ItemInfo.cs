using UnityEngine;

// 物品基础信息与玩法数据
public class ItemInfo : MonoBehaviour
{
    [Header("Basic Info")]
    [SerializeField] private string displayNameCN;
    [TextArea(2, 6)]
    [SerializeField] private string descriptionCN;
    [Header("Mask Part")]
    [SerializeField] private MaskPartType partType;
    [SerializeField] private bool isProcessed;
    [SerializeField] private bool canBeProcessed = true;
    [Header("Special Effect")]
    [SerializeField] private SpecialEffectType specialEffects = SpecialEffectType.None;

    public string DisplayNameCN => displayNameCN;
    public string DescriptionCN => descriptionCN;
    public MaskPartType PartType => partType;
    public bool IsProcessed => isProcessed;
    public bool CanBeProcessed => canBeProcessed;
    public SpecialEffectType SpecialEffects => specialEffects;

    // 设置加工状态
    public void SetProcessed(bool processed){
        if(canBeProcessed)
        {
            isProcessed = processed;
        }
    }
}
