using UnityEngine;
using UnityEngine.EventSystems;

public class DragByInterface : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private RectTransform canvasRect;
    private CanvasGroup canvasGroup;
    private Transform startParent;
    private Vector2 startAnchoredPosition;
    private bool wasDropped;
    private Vector3 originalScale;
    private Vector3 startScale;
    private ItemInfo cachedItemInfo;

    public Vector3 OriginalScale => originalScale;
    public ItemInfo ItemInfo => cachedItemInfo;

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

    private void Start(){
        startParent = transform.parent;
        if(rectTransform != null)
        {
            startAnchoredPosition = rectTransform.anchoredPosition;
        }
    }

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

    public void OnDrag(PointerEventData eventData){
        if(parentCanvas == null)
        {
            return;
        }

        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData){
        canvasGroup.blocksRaycasts = true;
        if(!wasDropped)
        {
            transform.SetParent(startParent, false);
            rectTransform.anchoredPosition = startAnchoredPosition;
            rectTransform.localScale = startScale;
        }
    }

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
