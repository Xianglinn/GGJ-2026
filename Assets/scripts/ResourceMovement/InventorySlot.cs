using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    private DragByInterface currentItem;
    private SlotSizeFitter sizeFitter;

    private void Awake(){
        sizeFitter = GetComponent<SlotSizeFitter>();
    }

    public void OnDrop(PointerEventData eventData){
        if(currentItem != null)
        {
            return;
        }

        if(eventData.pointerDrag == null)
        {
            return;
        }

        DragByInterface dragItem = eventData.pointerDrag.GetComponent<DragByInterface>();
        if(dragItem == null)
        {
            return;
        }

        dragItem.PlaceInSlot(transform);
        currentItem = dragItem;
        if(sizeFitter != null)
        {
            sizeFitter.Fit(dragItem.transform);
        }
    }

    public void ClearItem(DragByInterface item){
        if(currentItem == item)
        {
            currentItem = null;
        }
    }
}
