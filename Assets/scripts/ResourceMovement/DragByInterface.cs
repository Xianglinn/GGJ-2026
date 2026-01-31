using UnityEngine;
using UnityEngine.EventSystems;

// UI 拖拽物品逻辑（基于 EventSystem）
public class DragByInterface : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform; // 物品的 RectTransform
    private Canvas parentCanvas; // 所在 Canvas
    private RectTransform canvasRect; // Canvas 的 RectTransform
    private CanvasGroup canvasGroup; // 控制射线阻挡
    private Transform startParent; // 拖拽前父节点
    private Vector2 startAnchoredPosition; // 拖拽前位置
    private bool wasDropped; // 是否成功放入槽位
    private Vector3 originalScale; // 物品原始缩放
    private Vector3 startScale; // 拖拽前缩放
    private ItemInfo cachedItemInfo; // 缓存的物品信息

    // 供外部取原始缩放
    public Vector3 OriginalScale => originalScale;
    // 供外部读取物品信息
    public ItemInfo ItemInfo => cachedItemInfo;

    // 初始化组件与缓存
    private void Awake(){
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        if(parentCanvas != null)
        {
            canvasRect = parentCanvas.GetComponent<RectTransform>();
        }
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        if(rectTransform != null)
        {
            originalScale = rectTransform.localScale;
        }
        RefreshItemInfo();
    }

    // 记录初始父节点与位置
    private void Start(){
        startParent = transform.parent;
        if(rectTransform != null)
        {
            startAnchoredPosition = rectTransform.anchoredPosition;
        }
    }

    // 开始拖拽：清理旧槽位、置顶、居中到鼠标
    public void OnBeginDrag(PointerEventData eventData){
        if(rectTransform == null)
        {
            return;
        }

        wasDropped = false;
        RefreshItemInfo();
        startParent = transform.parent;
        startAnchoredPosition = rectTransform.anchoredPosition;
        startScale = rectTransform.localScale;
        rectTransform.localScale = originalScale;
        if(startParent != null)
        {
            InventorySlot inventorySlot = startParent.GetComponent<InventorySlot>();
            if(inventorySlot != null)
            {
                inventorySlot.ClearItem(this);
            }
            WorkBenchSlot workBenchSlot = startParent.GetComponent<WorkBenchSlot>();
            if(workBenchSlot != null)
            {
                workBenchSlot.ClearItem(this);
            }
            BlueprintSlot blueprintSlot = startParent.GetComponent<BlueprintSlot>();
            if(blueprintSlot != null)
            {
                blueprintSlot.ClearItem(this);
            }
        }
        if(parentCanvas != null)
        {
            transform.SetParent(parentCanvas.transform, true);
            transform.SetAsLastSibling();
            CenterOnPointer(eventData);
        }
        canvasGroup.blocksRaycasts = false;
    }

    // 拖拽过程中跟随鼠标
    public void OnDrag(PointerEventData eventData){
        if(parentCanvas == null)
        {
            return;
        }

        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    // 结束拖拽：没放入则回到原位置
    public void OnEndDrag(PointerEventData eventData){
        canvasGroup.blocksRaycasts = true;
        if(!wasDropped)
        {
            transform.SetParent(startParent, false);
            rectTransform.anchoredPosition = startAnchoredPosition;
            rectTransform.localScale = startScale;
        }
    }

    // 放入槽位并触发缩放适配
    public void PlaceInSlot(Transform slotTransform){
        wasDropped = true;
        transform.SetParent(slotTransform, false);
        rectTransform.anchoredPosition = Vector2.zero;
        SlotSizeFitter fitter = slotTransform.GetComponent<SlotSizeFitter>();
        if(fitter != null)
        {
            fitter.Fit(transform);
        }
    }

    // 将物品中心对齐到鼠标位置
    private void CenterOnPointer(PointerEventData eventData){
        if(canvasRect == null)
        {
            return;
        }

        Vector2 localPoint;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            Vector2 size = rectTransform.rect.size;
            Vector2 pivotOffset = (new Vector2(0.5f, 0.5f) - rectTransform.pivot) * size;
            rectTransform.anchoredPosition = localPoint + pivotOffset;
        }
    }

    // 重新查找并缓存 ItemInfo
    private void RefreshItemInfo(){
        cachedItemInfo = GetComponent<ItemInfo>();
        if(cachedItemInfo == null)
        {
            cachedItemInfo = GetComponentInParent<ItemInfo>();
        }
        if(cachedItemInfo == null)
        {
            cachedItemInfo = GetComponentInChildren<ItemInfo>();
        }
    }
}
