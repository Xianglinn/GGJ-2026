using UnityEngine;
using UnityEngine.EventSystems;

public class DragByInterface : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 startPosition; // 初始位置
    private bool isDragging; // 是否正在拖拽

    // 缓存初始位置
    private void Start(){
        startPosition = transform.position;
    }

    // 开始拖拽
    public void OnBeginDrag(PointerEventData eventData){
        isDragging = true;
    }

    // 拖拽中更新位置
    public void OnDrag(PointerEventData eventData){
        if(!isDragging)
        {
            return;
        }
        Debug.Log("draggging?");
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        transform.position = new Vector2(worldPos.x, worldPos.y);
    }

    // 结束拖拽
    public void OnEndDrag(PointerEventData eventData){
        isDragging = false;
        transform.position = startPosition;
    }
}
