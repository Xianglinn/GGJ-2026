using UnityEngine;
using UnityEngine.EventSystems;

// 工作台槽位：接收拖拽物品
public class WorkBenchSlot : MonoBehaviour, IDropHandler
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

    // 清空占位（用于拖出）
    public void ClearItem(DragByInterface item){
        if(currentItem == item)
        {
            currentItem = null;
        }
    }
}
