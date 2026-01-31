using UnityEngine;

public class SlotSizeFitter : MonoBehaviour
{
    public void Fit(Transform itemTransform){
        RectTransform slotRect = transform as RectTransform;
        RectTransform itemRect = itemTransform as RectTransform;
        if(slotRect == null || itemRect == null)
        {
            return;
        }

        DragByInterface dragItem = itemTransform.GetComponent<DragByInterface>();
        if(dragItem != null)
        {
            itemRect.localScale = dragItem.OriginalScale;
        }

        Vector2 slotSize = slotRect.rect.size;
        Vector2 itemSize = itemRect.rect.size;
        if(itemSize.x <= 0f || itemSize.y <= 0f)
        {
            return;
        }

        float scaleX = slotSize.x / itemSize.x;
        float scaleY = slotSize.y / itemSize.y;
        float scaleFactor = Mathf.Min(scaleX, scaleY);
        Vector3 baseScale = itemRect.localScale;
        if(dragItem != null)
        {
            baseScale = dragItem.OriginalScale;
        }
        itemRect.localScale = baseScale * scaleFactor;
    }
}
