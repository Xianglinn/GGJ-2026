using UnityEngine;

// 将物品缩放到槽位尺寸
public class SlotSizeFitter : MonoBehaviour
{
    // 根据槽位大小适配物品
    public void Fit(Transform itemTransform){
        RectTransform slotRect = transform as RectTransform;
        RectTransform itemRect = itemTransform as RectTransform;
        if(slotRect == null || itemRect == null)
        {
            return;
        }

        DragByInterface dragItem = itemTransform.GetComponent<DragByInterface>();
        Vector3 baseScale = Vector3.one;
        if(dragItem != null)
        {
            baseScale = dragItem.OriginalScale;
        }
        else
        {
            baseScale = itemRect.localScale;
        }

        Vector2 slotSize = slotRect.rect.size;
        Vector2 itemSize = itemRect.rect.size;
        
        if(itemSize.x <= 0f || itemSize.y <= 0f)
        {
            return;
        }

        // 计算适配比例
        float scaleX = slotSize.x / itemSize.x;
        float scaleY = slotSize.y / itemSize.y;
        float scaleFactor = Mathf.Min(scaleX, scaleY);

        // 设置本地缩放
        itemRect.localScale = baseScale * scaleFactor;
        
        Debug.Log($"[SlotSizeFitter] Fitting {itemTransform.name} into {transform.name}. SlotSize: {slotSize}, ItemSize: {itemSize}, ScaleFactor: {scaleFactor}");
    }
}
