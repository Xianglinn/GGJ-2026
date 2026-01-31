using UnityEngine;
using UnityEngine.EventSystems;

public class BlueprintSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private MaskPartType allowedPart;
    private DragByInterface currentItem;

    private void Awake(){
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

        ItemInfo info = GetItemInfoFrom(dragItem);
        if(info == null || info.PartType != allowedPart)
        {
            return;
        }

        dragItem.PlaceInSlot(transform);
        currentItem = dragItem;
    }

    public ItemInfo GetItemInfo(){
        return currentItem != null ? GetItemInfoFrom(currentItem) : null;
    }

    public void ClearItem(DragByInterface item){
        if(currentItem == item)
        {
            currentItem = null;
        }
    }

    private ItemInfo GetItemInfoFrom(DragByInterface dragItem){
        if(dragItem == null)
        {
            return null;
        }

        ItemInfo info = dragItem.GetComponent<ItemInfo>();
        if(info == null)
        {
            info = dragItem.GetComponentInParent<ItemInfo>();
        }
        if(info == null)
        {
            info = dragItem.GetComponentInChildren<ItemInfo>();
        }
        return info;
    }
}
