using UnityEngine;
using UnityEngine.EventSystems;

// 背包槽位：接收拖拽物品
public class InventorySlot : MonoBehaviour, IDropHandler
{
    private DragByInterface currentItem; // 当前物品
    [SerializeField] private bool destroySourceOnDrop; // 放入后销毁源物体（用于场景拾取）

    private void Awake(){
    }

    // 处理拖拽放入
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

        if(destroySourceOnDrop)
        {
            GameObject clone = Instantiate(dragItem.gameObject, transform);
            DragByInterface cloneDrag = clone.GetComponent<DragByInterface>();
            if(cloneDrag == null)
            {
                Destroy(clone);
                return;
            }
            cloneDrag.PlaceInSlot(transform);
            currentItem = cloneDrag;
            Destroy(dragItem.gameObject);
        }
        else
        {
            dragItem.PlaceInSlot(transform);
            currentItem = dragItem;
        }
    }

    public void SetItem(DragByInterface item){
        currentItem = item;
    }

    public ItemInfo GetItemInfo(){
        return currentItem != null ? currentItem.ItemInfo : null;
    }

    // 清空占位（用于拖出）
    public void ClearItem(DragByInterface item){
        if(currentItem == item)
        {
            currentItem = null;
        }
    }

    public void ClearImmediate(){
        if(currentItem != null)
        {
            Destroy(currentItem.gameObject);
            currentItem = null;
        }
    }
}
