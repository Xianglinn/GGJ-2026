using UnityEngine;
using UnityEngine.EventSystems;

// 工作台槽位：接收拖拽物品
public class WorkBenchSlot : MonoBehaviour, IDropHandler, ISlot
{
    private DragByInterface currentItem; // 当前物品

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

        dragItem.PlaceInSlot(transform);
        currentItem = dragItem;
    }

    public void SetItem(DragByInterface item){
        currentItem = item;
    }

    // 清空占位（用于拖出）
    public void ClearItem(DragByInterface item){
        if(currentItem == item)
        {
            currentItem = null;
        }
    }

    // 实现 ISlot 接口
    public ItemInfo GetItemInfo()
    {
        return currentItem != null ? currentItem.ItemInfo : null;
    }
}
