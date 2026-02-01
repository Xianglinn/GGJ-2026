using UnityEngine;
using UnityEngine.UI;

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

    [Header("UI")]
    [SerializeField] private Sprite unprocessedSprite;
    [SerializeField] private Sprite processedSprite;

    private Image cachedImage;

    public string ItemId => !string.IsNullOrEmpty(itemId) ? itemId : gameObject.name;
    public string DescriptionCN => descriptionCN;
    public MaskPartType PartType => partType;
    public bool IsProcessed => isProcessed;
    public bool CanBeProcessed => canBeProcessed;
    public SpecialEffectType SpecialEffects => specialEffects;

    private void Awake(){
        if(string.IsNullOrEmpty(itemId))
        {
            itemId = gameObject.name;
        }
        CacheImage();
        if(unprocessedSprite == null && cachedImage != null)
        {
            unprocessedSprite = cachedImage.sprite;
        }
        UpdateVisualState();
    }

    private void Start(){
        if(ItemLocationManager.Instance != null && ItemLocationManager.Instance.HasItemInLocation(ItemId, ItemLocation.Inventory))
        {
            gameObject.SetActive(false);
        }
    }

    // 设置加工状态
    public void SetProcessed(bool processed){
        if(canBeProcessed)
        {
            isProcessed = processed;
            UpdateVisualState();
        }
    }

    public void SetSpecialEffects(SpecialEffectType effects){
        specialEffects = effects;
    }

    public InventoryItemState ToState(){
        return new InventoryItemState(ItemId, isProcessed, specialEffects);
    }

    public ItemLocationState ToLocationState(ItemLocation location, int slotIndex, Vector2 anchoredPosition){
        return new ItemLocationState(ItemId, location, slotIndex, anchoredPosition, isProcessed, specialEffects);
    }

    public void ApplyState(InventoryItemState state){
        if(state == null)
        {
            return;
        }
        isProcessed = state.isProcessed;
        specialEffects = state.specialEffects;
        UpdateVisualState();
    }

    public void ApplyState(ItemLocationState state){
        if(state == null)
        {
            return;
        }
        isProcessed = state.isProcessed;
        specialEffects = state.specialEffects;
        UpdateVisualState();
    }

    private void CacheImage(){
        if(cachedImage != null)
        {
            return;
        }
        cachedImage = GetComponent<Image>();
        if(cachedImage == null)
        {
            cachedImage = GetComponentInChildren<Image>();
        }
    }

    private void UpdateVisualState(){
        CacheImage();
        if(cachedImage == null)
        {
            return;
        }
        if(isProcessed)
        {
            if(processedSprite != null)
            {
                cachedImage.sprite = processedSprite;
            }
        }
        else
        {
            if(unprocessedSprite != null)
            {
                cachedImage.sprite = unprocessedSprite;
            }
        }
    }
}
