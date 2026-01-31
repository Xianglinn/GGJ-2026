using UnityEngine;
using UnityEngine.EventSystems;

// 蓝图槽位：只接收指定部件类型
public class BlueprintSlot : MonoBehaviour, IDropHandler, ISlot
{
    [SerializeField] private MaskPartType allowedPart; // 允许的部件类型
    private DragByInterface currentItem; // 当前槽位物品
    private BlueprintController controller; // 所属蓝图控制器

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

        ItemInfo info = dragItem.ItemInfo;
        if(info == null)
        {
            info = GetItemInfoFrom(dragItem);
        }
        if(info == null || info.PartType != allowedPart)
        {
            return;
        }

        dragItem.PlaceInSlot(transform);
        currentItem = dragItem;
        NotifySlotChanged();
    }

    // 读取当前物品信息
    public ItemInfo GetItemInfo(){
        if(currentItem == null)
        {
            return null;
        }
        return currentItem.ItemInfo != null ? currentItem.ItemInfo : GetItemInfoFrom(currentItem);
    }

    // 清空占位（用于拖出）
    public void ClearItem(DragByInterface item){
        if(currentItem == item)
        {
            currentItem = null;
            NotifySlotChanged();
        }
    }

    // 从拖拽物体链路获取 ItemInfo
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

    public void SetController(BlueprintController owner){
        controller = owner;
    }

    private void NotifySlotChanged(){
        if(controller != null)
        {
            controller.NotifySlotChanged();
        }
    }
}
