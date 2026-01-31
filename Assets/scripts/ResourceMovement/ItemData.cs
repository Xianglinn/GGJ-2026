using UnityEngine;

// 物品静态数据（可复用）
[CreateAssetMenu(menuName = "GGJ/Item Data", fileName = "ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;

    [Header("Basic Info")]
    [SerializeField] private string displayNameCN;
    [TextArea(2, 6)]
    [SerializeField] private string descriptionCN;

    [Header("Mask Part")]
    [SerializeField] private MaskPartType partType;
    [SerializeField] private bool canBeProcessed = true;

    [Header("Special Effect")]
    [SerializeField] private SpecialEffectType defaultEffects = SpecialEffectType.None;

    [Header("UI")]
    [SerializeField] private GameObject uiPrefab;

    public string Id => id;
    public string DisplayNameCN => displayNameCN;
    public string DescriptionCN => descriptionCN;
    public MaskPartType PartType => partType;
    public bool CanBeProcessed => canBeProcessed;
    public SpecialEffectType DefaultEffects => defaultEffects;
    public GameObject UiPrefab => uiPrefab;
}
